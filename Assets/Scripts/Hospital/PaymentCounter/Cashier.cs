using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Hospital
{
    public class CashierEntity : Management.core.Entity
    {
        public int getCashFromAtATime = 1;//Range(0.1 - 10)
    }

    public class Cashier : Management.Hospital.Core.BaseEntity, Management.Hospital.IHospitalStaffEntity
    {
        CashierEntity cashierEntity = new CashierEntity();

        [Header("UI")]
        [SerializeField] protected Management.UI.UIUpdator _uiUpdator;
        public Management.UI.UIUpdator UiUpdator { get => _uiUpdator; }

        Management.Hospital.PaymentDepartment paymentDepartment;

        PaymentDepartment.PaymentProcessStatus paymentProsessStatus;
        public PaymentDepartment.PaymentProcessStatus PaymentProcessStatus { get => paymentProsessStatus; }

        [SerializeField] Management.Hospital.Scriptable.PaymentGlobalScriptable cashierGlobalScriptable;

        /// <summary>
        /// Initilize the department
        /// </summary>
        /// <param name="pDepartment"></param>
        public void Initilize(Management.Hospital.PaymentDepartment pDepartment)
        {
            paymentDepartment = pDepartment;
        }

        /// <summary>
        /// Get payment work time
        /// </summary>
        /// <returns></returns>
        public float GetWorkTime()
        {
            SaveLoad.HospitalDepartentSave hospitalDepartment = refStateController.GameManager.MasterLoader.HospitalDepartmentSaveModel.GetData(paymentDepartment.DepartmentType);
            if (hospitalDepartment != null)
                return paymentDepartment.GetWorkTime(hospitalDepartment.SalaryIncreaseIndexCount);
            else
                return paymentDepartment.GetWorkTime(0);//Load default time
        }

        public void InitilizePayment(Management.Patient.PatientBase patientBase, System.Action<Patient.PatientBase> cbOnPaymentComplete)
        {
            StartCoroutine(InitilizePaymentCoroutine(patientBase, cbOnPaymentComplete));
        }

        IEnumerator InitilizePaymentCoroutine(Management.Patient.PatientBase _patientBase, System.Action<Patient.PatientBase> cbOnPaymentComplete)
        {
            float workTime = refStateController.GameManager.IsCashierTimeBoosterRunning ? 1 : GetWorkTime();
            paymentProsessStatus = PaymentDepartment.PaymentProcessStatus.PAYMENT_IN_PROGESS;
            yield return StartCoroutine(InitilizeTiming(workTime, UiUpdator));
            UiUpdator.ResetUI();
            OnPaymentCompleted(_patientBase, cbOnPaymentComplete);
            paymentProsessStatus = PaymentDepartment.PaymentProcessStatus.PAYMENT_FINISHED;
        }

        protected virtual void OnPaymentCompleted(Management.Patient.PatientBase _patientBase, System.Action<Patient.PatientBase> cbOnPaymentComplete)
        {
            if(paymentDepartment.DepartmentType == Core.DepartmentType.PHARMASY)
                refStateController.GameManager.OnPaymentComplate(_patientBase.AmountNeedToPayInPharmacy);
            else //This is for consulation fees dpartment
                refStateController.GameManager.OnPaymentComplate(100);

            if (cbOnPaymentComplete != null)
                cbOnPaymentComplete(_patientBase);
        }
    }
}