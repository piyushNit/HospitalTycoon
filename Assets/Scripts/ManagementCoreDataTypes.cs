
namespace Management.Doctor.Core
{
    public enum DoctorType
    {
        ENT = 0,
        DENTIST,
        SURGEON,
        GYNAECOLOGIST,
        PAEDIATRICS,
        EYE_SPECILIST,
        ORTHOPAEDICS,
        NEUROLOGIST,
        CARDIOLOGIST,
        PSYCHIATRIST,
        SKIN_SPECIALIST,
        V_D,
        PLASTIC_SURGEON,

        NONE
    }
}

namespace Management.Hospital.Core
{
    public enum DepartmentType
    {
        ENT = 0,
        DENTIST,
        SURGEON,
        GYNAECOLOGIST,
        PAEDIATRICS,
        EYE_SPECILIST,
        ORTHOPAEDICS,
        NEUROLOGIST,
        CARDIOLOGIST,
        PSYCHIATRIST,
        SKIN_SPECIALIST,
        V_D,
        PLASTIC_SURGEON,

        PHARMASY,
        CONSULTATION_FEES,
        ADMIN,

        None
    }
}

namespace Management.Hospital.Scriptable
{
    [System.Serializable]
    public class GlobalEntityValue
    {
        [UnityEngine.Tooltip("Maximum experience over the game of doctors")]
        public float maxExperienceValue = 10;
        [UnityEngine.Tooltip("Maximum Skills over the game of doctors")]
        public float maxSkillValue = 10;
        [UnityEngine.Tooltip("Maximum Salary over the game of doctors")]
        public float maxSalaryValue = 10;
    }

    [System.Serializable]
    public class GlobalEntityValueUpgradeBY
    {
        [UnityEngine.Tooltip("experience upgrade value per upgrade")]
        public float experienceUpgradeValue = 1;
        [UnityEngine.Tooltip("skills upgrade value per upgrade")]
        public float skillUpgradeValue = 1;
        [UnityEngine.Tooltip("salary upgrade value per upgrade")]
        public float salaryUpgradeValue = 1;
    }

}
