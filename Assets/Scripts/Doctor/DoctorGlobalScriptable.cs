using UnityEngine;

namespace Management.Hospital.Scriptable
{
    //This is a global value for doctors
    [CreateAssetMenu(menuName = "Doctor/Global Value Container")]
    public class DoctorGlobalScriptable : ScriptableObject
    {
        [System.Serializable]
        public class DataGlobalValues : Management.Hospital.Scriptable.GlobalEntityValue
        {
            [Tooltip("Make sure that the department positions are equal to (diagnosisAtTime x maxDoctor)")]
            public int diagnosisAtTime = 5;
            //Max doctors each department
            [Tooltip("Make sure that the department positions are equal to (diagnosisAtTime x maxDoctor)")]
            public int maxDoctor = 5;
        }

        [System.Serializable]
        public class DataGlobalUpgradeValue : Management.Hospital.Scriptable.GlobalEntityValueUpgradeBY
        {
        }

        [Header("Global max values")]
        [SerializeField] DataGlobalValues globalValue;
        public DataGlobalValues GlobalValue { get => globalValue; }

        [Header("Per Upgrade values")]
        [SerializeField] DataGlobalUpgradeValue upgradeValueBy;
        public DataGlobalUpgradeValue DataUppgradeBy {get=> upgradeValueBy; }

        /// <summary>
        /// Experience + Skill + Salary
        /// Get Max value
        /// </summary>
        /// <returns></returns>
        public float GetMax() {
            return globalValue.maxExperienceValue
                + globalValue.maxSalaryValue
                + globalValue.maxSkillValue;
        }
    }
}
