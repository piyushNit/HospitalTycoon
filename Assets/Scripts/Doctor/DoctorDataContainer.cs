using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Doctor
{
    [System.Serializable]
    public class DoctorData
    {
        public string name;
        public Management.Doctor.Core.DoctorType doctorType;
        public Doctor doctorPrefab;
    }

    [CreateAssetMenu(menuName = "HospitalManagement/Doctor/DoctorContainer")]
    public class DoctorDataContainer : ScriptableObject
    {
        public List<DoctorData> doctorDataList;
        public DoctorData GetDoctorData(Management.Doctor.Core.DoctorType _doctorType)
        {
            return doctorDataList.Find(obj => obj.doctorType == _doctorType);
        }
    }
}