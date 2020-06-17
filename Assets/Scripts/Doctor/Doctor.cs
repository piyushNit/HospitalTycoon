using System.Collections;
using UnityEngine;

namespace Management.Doctor
{
    //Use this to save into binary and also use this to perform entity
    [System.Serializable]
    public class DoctorEntity : Management.core.Entity, SaveLoad.SaveCore
    {
        //public int diagnosisAtATime = 1;//range upto 100%
        public string ToJsonFormat()
        {
            return "";
        }
    }

    public class Doctor : Management.Hospital.Core.BaseEntity, Management.Hospital.IHospitalStaffEntity
    {
        protected const string IDLE = "Idle";
        protected const string WAVE_HAND = "Wave_Hand";

        [SerializeField] Management.Doctor.Core.DoctorType doctorType;
        public Management.Doctor.Core.DoctorType DoctorType { get => doctorType; }

        protected DoctorEntity doctorEntity = new DoctorEntity();
        public DoctorEntity DoctorEntity { get => doctorEntity; }
        [SerializeField] Management.Hospital.Scriptable.DoctorGlobalScriptable globalValue;
        [SerializeField] Animator anim;

        [Header("UI")]
        [SerializeField] protected Management.UI.UIUpdator _uiUpdator;
        public Management.UI.UIUpdator UiUpdator { get => _uiUpdator; }

        protected Management.Patient.PatientBase patientBase = null;

        public static System.Action<Management.Doctor.Core.DoctorType> Cb_OnDiagnosisOver;

        int currentPatientCount = 0;

        Management.Hospital.Department department;

        #region FOR_DEBUG && UNITY_EDITOR
        #if UNITY_EDITOR
        [ContextMenu("Log Entity")]
        public void PrintDoctorValues()
        {
            string printStr = "| Salary : ";
            Debug.Log(printStr);
        }
        #endif
        #endregion
        /// <summary>
        /// This will override the default setted doctor type
        /// </summary>
        /// <param name="_doctorType"></param>
        public void Initilize(Management.Hospital.Department _department, Management.Doctor.Core.DoctorType _doctorType)
        {
            department = _department;
            doctorType = _doctorType;
        }

        /// <summary>
        /// Get doctor diagnosis time
        /// </summary>
        /// <returns></returns>
        public virtual float GetWorkTime()
        {
            float maxValue = globalValue.GetMax();
            currentPatientCount++;
            return department.GetTreatmentTime();
        }

        /// <summary>
        /// If this function gets called means the treatment is over and doctor is free to treat next patient
        /// </summary>
        public virtual void OnDiagnosisOver()
        {
            anim.SetTrigger(IDLE);
            currentPatientCount--;
            currentPatientCount = Mathf.Clamp(currentPatientCount, 0, currentPatientCount);

            if (Cb_OnDiagnosisOver != null)
                Cb_OnDiagnosisOver(DoctorType);
        }

        /// <summary>
        /// Load entity from externally like saved in the memory.
        /// This function can be used while loading the game.
        /// </summary>
        /// <param name="entity"></param>
        public void LoadEntityExternally(DoctorEntity entity)
        {
            if (entity == null)
                return;
            doctorEntity = entity;
        }

        /// <summary>
        /// Is doctor is busy to diagnosis other patients
        /// </summary>
        /// <returns></returns>
        public bool IsDoctorBusy()
        {
            return patientBase != null;
            //return currentPatientCount >= doctorEntity.diagnosisAtATime;
        }

        /// <summary>
        /// initilize the patient when patient comes for treatment
        /// </summary>
        /// <param name="_patientBase"></param>
        public void InitPatient(Management.Patient.PatientBase _patientBase)
        {
            patientBase = _patientBase;
        }

        /// <summary>
        /// Start Treatment
        /// </summary>
        public void ReadyAndStartDiagonis()
        {
            anim.SetTrigger(WAVE_HAND);
            StartCoroutine(StartTreatment());
        }

        protected IEnumerator StartTreatment()
        {
            float treatmentTime = refStateController.GameManager.IsDoctorTimeBoosterRunning ? 1 : GetWorkTime();

            yield return StartCoroutine(InitilizeTiming(treatmentTime, UiUpdator));
            UiUpdator.ResetUI();
            if(patientBase != null)
                patientBase.CompleteTreatment();
            patientBase = null;
            OnDiagnosisOver();
        }

        protected override void UpdateImageAmount(float time)
        {
            base.UpdateImageAmount(time);
            _uiUpdator.UpdateImageAmount(time);
        }

        protected override void ResetUI()
        {
            base.ResetUI();
            _uiUpdator.ResetUI();
        }

        //public bool IsUpgradeAvailable()
        //{
        //    return doctorEntity.DoesSalaryCanUpgrade(globalValue.GlobalValue.maxSalaryValue);
        //}

        //public void UpdateSalaryByOne()
        //{
        //    doctorEntity.UpgradeSalary(globalValue.DataUppgradeBy.salaryUpgradeValue, globalValue.GlobalValue.maxSalaryValue);
        //}
    }
}