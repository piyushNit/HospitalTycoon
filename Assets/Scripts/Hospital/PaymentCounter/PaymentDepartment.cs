using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Hospital
{
    public enum PaymentCallbackKey
    {
        PHARMACY_CALLBACK,
        CONSULTATION_CALLKBACK
    }

    //This is a Pharmasy Department
    public class PaymentDepartment : Management.Hospital.BaseDepartment
    {
        public enum PaymentProcessStatus
        {
            NONE = 0,
            PAYMENT_IN_PROGESS,
            PAYMENT_FINISHED
        }

        private class CashierPositions : Management.Hospital.StaffPosition
        {
            public CashierPositions()
            {
                isPositionAvailable = true;
                transform = null;
            }
        }

        private class PatientQueuePositions : Management.Hospital.StaffPosition
        {
            public PatientQueuePositions()
            {
                isPositionAvailable = true;
                transform = null;
            }
        }

        [Header("Payment Department")]
        [SerializeField] protected HospitalManager hospitalManager;
        [SerializeField] protected Transform patientPosTrans;
        [SerializeField] protected Transform cashierPosTrans;
        [SerializeField] protected float queueDistance = 10;
        [SerializeField] protected Cashier cashierPrefab;
        [SerializeField] protected int queueLength = 10;
        public Cashier CashierPrefab { get => cashierPrefab; }

        PaymentProcessStatus paymentProsessStatus;

        protected Dictionary<Cashier, Management.Hospital.QueueHelper> cashierqueueHelperDict;
        public Dictionary<Cashier, Management.Hospital.QueueHelper> CashierQueueHelperDict { get => cashierqueueHelperDict; }

        CashierPositions[] cashierPositions;
        PatientQueuePositions[] patientQueuePositions;


        #if UNITY_EDITOR
        [ContextMenu("Print Log")]
        public void PrintLog()
        {
            string logmsg = "Payment status : " + paymentProsessStatus.ToString() + "\n";
            int index = 1;
            foreach (KeyValuePair<Cashier, Management.Hospital.QueueHelper> keyValue in cashierqueueHelperDict)
            {
                logmsg += ("\n Queue-" + index.ToString());
                logmsg += keyValue.Value.GetLog();
                index++;
            }
            
            Debug.Log(logmsg);
        }
        #endif

        /// <summary>
        /// Get the count of total slots of cashier
        /// </summary>
        /// <returns></returns>
        public int GetTotalCashierSlots()
        {
            return cashierPositions.Length;
        }

        public virtual void Initilize(int paymentCallbackKey = 0)
        {
            cashierqueueHelperDict = new Dictionary<Cashier, QueueHelper>();

            departmentUpgradeJson = refStateController.DepartmentUpgradeScriptable.GetJsonModel<Management.Hospital.Json.DepartmentUpgrades>(DepartmentType);
            if (DeaprtmentUpgradeJson == null)
            {
                Debug.LogError("Game Error!! Json for department type :" + DepartmentType.ToString() + " is not set");
            }

            cashierPositions = new CashierPositions[cashierPosTrans.childCount];
            for (int i = 0; i < cashierPositions.Length; i++)
            {
                cashierPositions[i] = new CashierPositions();
                cashierPositions[i].transform = cashierPosTrans.GetChild(i);
            }

            patientQueuePositions = new PatientQueuePositions[patientPosTrans.childCount];
            for (int i = 0; i < patientQueuePositions.Length; i++)
            {
                patientQueuePositions[i] = new PatientQueuePositions();
                patientQueuePositions[i].transform = patientPosTrans.GetChild(i);
            }
        }

        public virtual void AddCashier(Cashier _cashier, int paymentCallbackKey = 0)
        {
            for (int i = 0; i < patientQueuePositions.Length; i++)
            {
                if (patientQueuePositions[i].isPositionAvailable)
                {
                    QueueHelper queueHelperTemp = new QueueHelper(patientQueuePositions[i].transform, patientQueuePositions[i].transform.forward, _paymentCallbackKey: (PaymentCallbackKey)paymentCallbackKey, _maxQueueLength:queueLength, _queueDistance: queueDistance);
                    queueHelperTemp.CbOnProceedPayment = PrcoeedPayment;
                    queueHelperTemp.CbOnProceedFurtherPayment = ProceedFurtherPayment;
                    queueHelperTemp.CbOnQueueRearrange = QueueRearrange;

                    cashierqueueHelperDict.Add(_cashier, queueHelperTemp);

                    patientQueuePositions[i].isPositionAvailable = false;
                    break;
                }
            }
        }

        public virtual Management.Hospital.QueueHelper.AddQueueResult AddToQueue(Management.Patient.PatientBase patientBase)
        {
            QueueHelper queueHelper = GetLowestQueueHelper();
            if (queueHelper == null)
                return null;
            return queueHelper.AddToQueue(patientBase);
        }

        QueueHelper GetLowestQueueHelper()
        {
            QueueHelper queueHelper = null;
            int lowestQueue = 10000;
            foreach (KeyValuePair<Cashier, Management.Hospital.QueueHelper> keyValue in cashierqueueHelperDict)
            {
                if (keyValue.Value.GetPatientWaitCount() < lowestQueue)
                {
                    lowestQueue = keyValue.Value.GetPatientWaitCount();
                    queueHelper = keyValue.Value;
                }
            }
            return queueHelper;
        }

        /// <summary>
        /// Get percent 0% - 100%
        /// </summary>
        /// <returns></returns>
        public float GetStaffPercentage()
        {
            int totalDoctor = cashierPositions.Length;
            int currDoctorCount = cashierqueueHelperDict.Count;

            float per = ((float)currDoctorCount / (float)totalDoctor) * 100;
            return per;
        }

        /// <summary>
        /// Loads department content from json
        /// </summary>
        /// <param name="index"></param>
        new public void LoadUpgradeContent(Json.UpgradableContent content)
        {
            upgradeContent = content;
        }

        /// <summary>
        /// Grabs the position of the empty slot to instantiate new doctor in the position
        /// If return value is (0, 0, 0) then there is no place for instantiate new doctor
        /// </summary>
        /// <param name="patientBase"></param>
        /// <returns></returns>
        public Transform GrabThePosition()
        {
            for (int i = 0; i < cashierPositions.Length; i++)
            {
                if (cashierPositions[i].isPositionAvailable)
                {
                    cashierPositions[i].isPositionAvailable = false;
                    return cashierPositions[i].transform;
                }
            }
            return null;
        }

        public virtual bool CanAddStaff()
        {
            return cashierqueueHelperDict.Count < cashierPositions.Length;
        }

        public virtual bool CanAddToWaitQueue()
        {
            bool result = false;

            foreach (KeyValuePair<Cashier, Management.Hospital.QueueHelper> keyValue in cashierqueueHelperDict)
            {
                if (keyValue.Value.CanAddToWaitQueue())
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        protected virtual void PrcoeedPayment(Management.Patient.PatientBase _patientBase)
        {
            foreach (KeyValuePair<Cashier, QueueHelper> keyValue in CashierQueueHelperDict)
            {
                if (keyValue.Value.IsContains(_patientBase))
                {
                    keyValue.Key.InitilizePayment(_patientBase, CbOnPaymentComplated);
                    break;
                }
            }
            
        }

        protected virtual void CbOnPaymentComplated(Management.Patient.PatientBase _patientBase)
        {

        }

        void ProceedFurtherPayment(Management.Hospital.QueueHelper _queueHelper, Management.Patient.PatientBase patientBase)
        {
            _queueHelper.AnalylizeThePayment(patientBase, paymentProsessStatus);
        }

        void QueueRearrange(Management.Hospital.QueueHelper _queueHelper)
        {
            _queueHelper.ProceedForPaymentAfterReaarageTheQueue(paymentProsessStatus);
        }

    }
}
