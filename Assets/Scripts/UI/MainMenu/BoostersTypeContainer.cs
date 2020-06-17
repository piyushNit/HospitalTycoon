using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Hospital.Scriptable
{
    [CreateAssetMenu(menuName = "HospitalManagement/BoosterContainer")]
    public class BoostersTypeContainer : ScriptableObject
    {
        [System.Serializable]
        public class BoosterContainer
        {
            public Management.Hospital.BoosterType boosterType;
            public Sprite boosterSpr;
        }

        [SerializeField] private List<BoosterContainer> boosterContainerList;

        public Sprite GetBoosterSpr(Management.Hospital.BoosterType boosterType)
        {
            return boosterContainerList.Find(obj => obj.boosterType == boosterType).boosterSpr;
        }
    }
}