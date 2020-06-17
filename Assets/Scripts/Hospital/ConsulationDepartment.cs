using System.Collections;
using System.Collections.Generic;
using Management.Patient;
using UnityEngine;

namespace Management.Hospital
{
    public class ConsulationDepartment : PaymentDepartment
    {
        [SerializeField] GameObject secondConsulationTable;
        [SerializeField] BoxCollider secondConsulationTableCollider;

        protected static PaymentCallbackKey paymentCallbackKey = 0;
        public static PaymentCallbackKey PaymentCBKey { get => paymentCallbackKey; }

        public override void Initilize(int callbackKey = 0)
        {
            paymentCallbackKey = PaymentCallbackKey.CONSULTATION_CALLKBACK;
            base.Initilize((int)paymentCallbackKey);

            refStateController.GameManager.SpawnCashierInConsulationDepartment(refStateController);
            refStateController.HospitalManager.HospitalBuilding.UnlockPaymentDepartment(departmentType);
        }

        public override void AddCashier(Cashier _cashier, int paymentCallbackKey = 0)
        {
            base.AddCashier(_cashier, paymentCallbackKey);
            if (CashierQueueHelperDict.Count > 2) //Unlocking new model when cashier count is greater than 2
            {
                secondConsulationTable.SetActive(true);
                secondConsulationTableCollider.enabled = true;
            }
        }

        protected override void CbOnPaymentComplated(PatientBase _patientBase)
        {
            base.CbOnPaymentComplated(_patientBase);
            _patientBase.GotowardsDoctor();
        }

        public override void LoadDepartment(Arch.Core.StateController stateController)
        {
            base.LoadDepartment(stateController);
        }

        public void CheckForPartTwoFTUE()
        {
            foreach (KeyValuePair<Cashier, QueueHelper> keyValue in cashierqueueHelperDict)
            {
                if (keyValue.Value.GetPatientWaitCount() > 7)
                {
                    refStateController.FTUEManager.ShowFTUE(FTUE.FTUEType.FTUE_PATINETS_WAIT_TOO_LONG);
                    break;
                }
            }
        }
    }
}