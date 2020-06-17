using System.Collections;
using System.Collections.Generic;
using Management.Patient;
using UnityEngine;

namespace Management.Hospital
{
    public class PharmacyDepartment : PaymentDepartment
    {
        protected static PaymentCallbackKey paymentCallbackKey = 0;
        public static PaymentCallbackKey PaymentCBKey { get => paymentCallbackKey; }

        public override void Initilize(int callbackKey = 0)
        {
            paymentCallbackKey = PaymentCallbackKey.PHARMACY_CALLBACK;
            base.Initilize((int)paymentCallbackKey);

            refStateController.GameManager.SpawnCashierInPharmacyDepartment(refStateController);
            refStateController.HospitalManager.HospitalBuilding.UnlockPaymentDepartment(departmentType);
        }

        protected override void CbOnPaymentComplated(PatientBase _patientBase)
        {
            base.CbOnPaymentComplated(_patientBase);
            _patientBase.PharmacyPaymentFinished();
        }

        public override void LoadDepartment(Arch.Core.StateController stateController)
        {
            base.LoadDepartment(stateController);
        }
    }
}