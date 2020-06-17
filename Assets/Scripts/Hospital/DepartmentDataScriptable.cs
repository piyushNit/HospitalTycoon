using UnityEngine;
using System.Collections.Generic;

namespace Management.Hospital
{
    /// <summary>
    /// This class is use to map the data of next upgrade of department
    /// All upgrade conditions will be here
    /// </summary>
    [System.Serializable]
    public class DepartmentUpgradeDataScriptable
    {
        public string name;

        public int levelUpgradeRequirement;
        //add next upgrade department data
    }

    [CreateAssetMenu(menuName = "Hospital/DepartmentDataContainer")]
    public class DepartmentDataScriptable : ScriptableObject
    {
        public Management.Hospital.Core.DepartmentType departmentType;
        public List<DepartmentUpgradeDataScriptable> departmentLevelDataList;

        public DepartmentUpgradeDataScriptable GetDepartmentData(int levelIndex)
        {
            if (levelIndex > departmentLevelDataList.Count)
                return null;
            return departmentLevelDataList[levelIndex];
        }
    }
}