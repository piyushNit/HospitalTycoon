using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Hospital.Revealer
{
    public class HospitalBuildingRevealer : MonoBehaviour
    {
        [System.Serializable]
        public class BuildingElements
        {
            [SerializeField] string name;
            [SerializeField] Management.Hospital.Core.DepartmentType departmentType;
            public Management.Hospital.Core.DepartmentType DepartmentType { get => departmentType; }
            [SerializeField] GameObject[] objectsToUnhide;
            public GameObject[] ObjectsToUnhide { get => objectsToUnhide; }
            [SerializeField] GameObject[] objectsToHide;
            public GameObject[] ObjectsToHide { get => objectsToHide; }
            [SerializeField] GameObject[] objectRevealers;
            public GameObject[] ObjectRevealers { get => objectRevealers; }
        }

        [SerializeField] List<BuildingElements> buildingElementList;
        public List<BuildingElements> BuildingElementList { get => buildingElementList; }

        /// <summary>
        /// Hide and unhides the department
        /// </summary>
        /// <param name="departmentType"></param>
        public void RevealBuilding(Management.Hospital.Core.DepartmentType departmentType, List<Department> departments)
        {
            BuildingElements buildingElement = buildingElementList.Find(obj => obj.DepartmentType == departmentType);
            if (buildingElement == null)
                return;

            ToggleObjectArray(buildingElement.ObjectsToHide, false);
            ToggleObjectArray(buildingElement.ObjectsToUnhide, true);
            AnalyzeConditionalRevealer(buildingElement.ObjectsToUnhide, departments);
            AnalyzeConditionalRevealer(buildingElement.ObjectRevealers, departments);
        }

        void ToggleObjectArray(GameObject[] objects, bool flag)
        {
            for (int i = 0; i < objects.Length; ++i)
            {
                objects[i].SetActive(flag);
            }
        }

        void AnalyzeConditionalRevealer(GameObject[] objects, List<Department> departments)
        {
            for (int i = 0; i < objects.Length; ++i)
            {
                ConditionalRevealer[] conditionalRevealers = objects[i].GetComponentsInChildren<ConditionalRevealer>();
                for (int j = 0; j < conditionalRevealers.Length; ++j)
                {
                    Debug.Log(conditionalRevealers[j].Condition);
                    conditionalRevealers[j].AnalyzeConditionAndUpdateResult(departments);
                }
            }
        }

        
    }
}
