using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Hospital.Scriptable
{
    [System.Serializable]
    public class CashierGlobalValues : Management.Hospital.Scriptable.GlobalEntityValue
    {
    }

    [System.Serializable]
    public class CashierGlobalUpgradeValues : Management.Hospital.Scriptable.GlobalEntityValueUpgradeBY
    {
    }

    [CreateAssetMenu(menuName = "Hospital/Cashier Scriptable")]
    public class PaymentGlobalScriptable : ScriptableObject
    {
        [Header("Global max values")]
        [SerializeField] CashierGlobalValues globalValue;
        public CashierGlobalValues GlobalValue { get => globalValue; }

        [Header("Per upgrade values")]
        [SerializeField] CashierGlobalUpgradeValues cashierUpgradeValues;
        public CashierGlobalUpgradeValues CashierUpgradeValues { get => cashierUpgradeValues; }

        /// <summary>
        /// Experience + Skill + Salary
        /// Get Max value
        /// </summary>
        /// <returns></returns>
        public float GetMax()
        {
            return globalValue.maxExperienceValue
                + globalValue.maxSalaryValue
                + globalValue.maxSkillValue;
        }
    }
}