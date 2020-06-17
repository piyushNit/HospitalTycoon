using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Hospital.Scriptable
{
    [CreateAssetMenu(menuName = "Hospital/HospitalJsonData")]
    public class HospitalJsonDataScriptable : ScriptableObject
    {
        [SerializeField] TextAsset parkingJson;
        [SerializeField] TextAsset departmentStaffAndSalaryJson;
        [SerializeField] TextAsset waitingLobbyJson;

        public TextAsset ParkingJson { get => parkingJson; }
        public TextAsset DepartmentStaffAndSalaryJson { get => departmentStaffAndSalaryJson; }
        public TextAsset WaitingLobbyJson { get => waitingLobbyJson; }
    }
}