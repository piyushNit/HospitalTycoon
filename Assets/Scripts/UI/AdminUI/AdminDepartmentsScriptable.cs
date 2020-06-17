using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.UI.Admin
{
    [CreateAssetMenu(menuName = "UI/AdminDepartmentsPanelInfo")]
    public class AdminDepartmentsScriptable : ScriptableObject
    {
        [System.Serializable]
        public class DepartmentsDataForUI
        {
            public string title;
            public Management.Hospital.Core.DepartmentType departmentType;
            public Sprite sprIcon;
            public string description;
            public int buyValue;
        }

        [SerializeField] List<DepartmentsDataForUI> departmentDataList;
        public List<DepartmentsDataForUI> DepartmentDataList { get => departmentDataList; }

        public DepartmentsDataForUI GetInfo(Management.Hospital.Core.DepartmentType departmentType)
        {
            return departmentDataList.Find(obj => obj.departmentType == departmentType);
        }
    }
}