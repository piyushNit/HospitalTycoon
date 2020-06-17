using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Management.Hospital
{
    public class QueueHelper
    {
        public class AddQueueResult
        {
            public Vector3 position;
            public System.Action<Management.Patient.PatientBase> funcCallback;

            public AddQueueResult(Vector3 pos, System.Action<Management.Patient.PatientBase> callback)
            {
                position = pos;
                funcCallback = callback;
            }

            public static AddQueueResult Empty()
            {
                return null;
            }
        }
        Transform queueStartTransform;
        Queue<Management.Patient.PatientBase> patientTransformList;
        public Queue<Management.Patient.PatientBase> PatientTransformList { get =>patientTransformList; }

        Queue<Management.Patient.PatientBase> patientWaitingList = new Queue<Patient.PatientBase>();
        public Queue<Management.Patient.PatientBase> PatientWaitingList { get => patientTransformList; }

        public int maxQueueLength = 10;
        Vector3 queueDirection;
        float queueDistance = 10;

        PaymentCallbackKey paymentCallbackKey;

        System.Action<Management.Patient.PatientBase> OnProceedPayment;
        public System.Action<Management.Patient.PatientBase> CbOnProceedPayment { get => OnProceedPayment; set => OnProceedPayment = value; }

        System.Action<QueueHelper, Management.Patient.PatientBase> OnProceedFurtherPayment;
        public System.Action<QueueHelper, Management.Patient.PatientBase> CbOnProceedFurtherPayment { get => OnProceedFurtherPayment; set => OnProceedFurtherPayment = value; }

        System.Action<QueueHelper> OnQueueRearrange;
        public System.Action<QueueHelper> CbOnQueueRearrange { get => OnQueueRearrange; set => OnQueueRearrange = value; }

        /// <summary>
        /// Keep queue Direction to forward to the _queueStartTransform
        /// </summary>
        /// <param name="_queueStartTrans"></param>
        /// <param name="_queueDirection"></param>
        public QueueHelper(Transform _queueStartTrans, Vector3 _queueDirection, PaymentCallbackKey _paymentCallbackKey, int _maxQueueLength = 10, float _queueDistance = 10)
        {
            queueStartTransform = _queueStartTrans;
            maxQueueLength = _maxQueueLength;
            queueDistance = _queueDistance;
            queueDirection = _queueDirection;
            paymentCallbackKey = _paymentCallbackKey;
            patientTransformList = new Queue<Management.Patient.PatientBase>();
        }

        /// <summary>
        /// Checks if queue maximum lenth is over
        /// </summary>
        /// <returns></returns>
        public bool CanAddToWaitQueue()
        {
            return patientTransformList.Count < maxQueueLength;
        }

        /// <summary>
        /// Get patients count who are standing in the queue
        /// </summary>
        /// <returns></returns>
        public int GetPatientWaitCount()
        {
            return PatientTransformList.Count;
        }

        /// <summary>
        /// Before adding to the queue before check with CanAddToWaitQueue()
        /// </summary>
        /// <param name="_patient"></param>
        /// <returns></returns>
        public AddQueueResult AddToQueue(Management.Patient.PatientBase _patient)
        {
            Vector3 position = queueStartTransform.position;
            position += queueDirection * (queueDistance * patientTransformList.Count);
            patientTransformList.Enqueue(_patient);

            AnalyzeForPaymentTimerBooster();

            if(!_patient.CbOnPaymentCompleteDictonary.ContainsKey(paymentCallbackKey))
                _patient.CbOnPaymentCompleteDictonary[paymentCallbackKey] = CbOnRemoveFromQueue;
            return new AddQueueResult(position, FuncCallbackFromPatient);
        }

        void AnalyzeForPaymentTimerBooster()
        {
            if (maxQueueLength / 2 < patientTransformList.Count && Management.UI.UiHUD.AddBooster != null)
                Management.UI.UiHUD.AddBooster(BoosterType.Cashier_Time_Booster);
        }

        /// <summary>
        /// Checks if the patient is contains into the list
        /// </summary>
        /// <param name="patientBase"></param>
        /// <returns></returns>
        public bool IsContains(Management.Patient.PatientBase patientBase)
        {
            return PatientTransformList.Contains(patientBase);
        }

        void FuncCallbackFromPatient(Management.Patient.PatientBase patientBase)
        {
            if (CbOnProceedFurtherPayment != null)
                CbOnProceedFurtherPayment(this, patientBase);
        }

        public void AnalylizeThePayment(Management.Patient.PatientBase patientBase, PaymentDepartment.PaymentProcessStatus paymentStatus)
        {
            if (patientTransformList.Count > 0 && patientTransformList.ElementAt(0) == patientBase && paymentStatus != PaymentDepartment.PaymentProcessStatus.PAYMENT_IN_PROGESS)
            {
                if (OnProceedPayment != null)
                    OnProceedPayment(patientTransformList.Peek());
            }
        }

        void ArrangeQueueMembers()
        {
            Vector3 newPosition = queueStartTransform.position;
            for (int i = 0; i < patientTransformList.Count; i++)
            {
                int index = i;
                newPosition = queueStartTransform.position;
                newPosition += queueDirection * (queueDistance * i);
                patientTransformList.ElementAt(i).MoveTowards(newPosition, ()=> {
                    if (index == 0)
                    {
                        if (CbOnQueueRearrange != null)
                            CbOnQueueRearrange(this);
                    }
                });
            }
        }

        public void CbOnRemoveFromQueue()
        {
            patientTransformList.Dequeue();
            ArrangeQueueMembers();
        }

        public void ProceedForPaymentAfterReaarageTheQueue(PaymentDepartment.PaymentProcessStatus paymentStatue)
        {
            if (patientTransformList.Count > 0 && paymentStatue != PaymentDepartment.PaymentProcessStatus.PAYMENT_IN_PROGESS)
            {
                if (OnProceedPayment != null)
                    OnProceedPayment(patientTransformList.Peek());
            }

            StandIntoQueueWhoWereWaiting();
        }

        void StandIntoQueueWhoWereWaiting()
        {
            if (patientWaitingList.Count <= 0)
                return;
            Management.Patient.PatientBase patientBase = patientWaitingList.Dequeue();
            patientBase.PayConsulationFees();
        }

        /// <summary>
        /// add into waiting queue, patients who does not able to stand into queue
        /// </summary>
        /// <param name="_patient"></param>
        public void AddIntoWaitingQueueList(Management.Patient.PatientBase _patient)
        {
            patientWaitingList.Enqueue(_patient);
        }

        #if UNITY_EDITOR
        public string GetLog()
        {
            string logmsg = "Queue Helper Patient count:" + PatientTransformList.Count + "\n";
            for (int i = 0; i < PatientTransformList.Count; i++)
            {
                logmsg += "i : " + i + "->";
                if (PatientTransformList.ElementAt(i) == null)
                    logmsg += "null";
                else
                    logmsg += PatientTransformList.ElementAt(i).GetLog();
                logmsg += "\n";
            }
            logmsg += "Patient waiting outide queue:" + patientWaitingList.Count + "\n";
            for (int i = 0; i < patientWaitingList.Count; i++)
            {
                logmsg += "i : " + i + "->";
                if (patientWaitingList.ElementAt(i) == null)
                    logmsg += "null";
                else
                    logmsg += patientWaitingList.ElementAt(i).GetLog();
                logmsg += "\n";
            }
            return logmsg;
        }
        #endif
    }
}