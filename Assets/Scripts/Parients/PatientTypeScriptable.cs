using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Patient {
    public enum PatientType {
        HUMAN = 0
    }

    [System.Serializable]
    public class PatientDataHolder {
        public string name;
        public PatientType patientType;
        public PatientBase[] patientPrefabs;

        public PatientBase GetRandomPatient()
        {
            if (patientPrefabs.Length == 0)
                return null;
            PatientBase patientBase = patientPrefabs[Random.Range(0, patientPrefabs.Length)];
            return patientBase;
        }
    }

    [System.Serializable]
    public class HumanExpressionData
    {
        public Management.Hospital.Core.BaseEntity.MoodState moodState;
        public Sprite sprite;
    }

    [CreateAssetMenu(menuName = "HospitalManagement/Patient/PatientDataContainer")]
    public class PatientTypeScriptable : ScriptableObject {
        public List<PatientDataHolder> patientDataHolders;
        public List<HumanExpressionData> humanExpressionDataList;

        public PatientDataHolder GetPatientData(PatientType _type) {
            return patientDataHolders.Find(obj => obj.patientType == _type);
        }

        public PatientBase GetRandomPatient(PatientType _type)
        {
            PatientDataHolder dataHolder = GetPatientData(_type);
            if (dataHolder == null)
                return null;
            return dataHolder.GetRandomPatient();
        }

        public Sprite GetExpressionSprite(Management.Hospital.Core.BaseEntity.MoodState mood)
        {
            HumanExpressionData humanExpression = humanExpressionDataList.Find(obj => obj.moodState == mood);
            if(humanExpression == null)
                return null;
            return humanExpression.sprite;
        }
    }
}
