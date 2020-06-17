using UnityEngine;
using System.Collections.Generic;

namespace Management.Hospital
{
    public class BaseDepartment : MonoBehaviour
    {
        [SerializeField] protected Management.Hospital.Core.DepartmentType departmentType;
        public Management.Hospital.Core.DepartmentType DepartmentType { get => departmentType; }

        protected Management.Hospital.Json.UpgradableContent upgradeContent = null;
        public Management.Hospital.Json.UpgradableContent UpgradeContent { get => upgradeContent; set => upgradeContent = value; }

        protected Management.Hospital.Json.DepartmentUpgrades departmentUpgradeJson = null;
        public Management.Hospital.Json.DepartmentUpgrades DeaprtmentUpgradeJson { get => departmentUpgradeJson; }

        [SerializeField] protected GameObject[] upgradeUnlockList;

        [SerializeField] protected Management.UI.UiType uiType;
        public Management.UI.UiType UiType { get => uiType; }

        [Header("Staff Work time")]
        [SerializeField] protected float initialWorkTime = 10;
        [SerializeField] protected float reduceWorkTimeByPercent = 2;
        [SerializeField] protected float maxCanReduceWorkTimeByPercent = 500;
        public float MaxCanReduceWorkTimeByPercent { get => maxCanReduceWorkTimeByPercent; }
        [SerializeField] protected float minWorkTime = 0.1f;

        protected Arch.Core.StateController refStateController;
        /// <summary>
        /// a base class methos which can be used at the time pf unlocking the department and this method will ensure to load their references
        /// </summary>
        public virtual void LoadDepartment(Arch.Core.StateController stateController)
        {
            refStateController = stateController;
        }

        /// <summary>
        /// Loads department content from json
        /// </summary>
        /// <param name="level"></param>
        public void LoadUpgradeContent(int level = 1)
        {
            LoadUpgradeContent(GetUpgradeContentFromLevel(level));
        }

        /// <summary>
        /// Get the upgrade Contnet read from json
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public Management.Hospital.Json.UpgradableContent GetUpgradeContentFromLevel(int level)
        {
            if (departmentUpgradeJson == null)
                return null;
            int index = 0;
            for (int i = 0; i < departmentUpgradeJson.upgradable_contents.Count; i++)
            {
                if (level >= departmentUpgradeJson.upgradable_contents[i].level)
                    index = i;
            }
            return departmentUpgradeJson.upgradable_contents[index];
        }

        /// <summary>
        /// Loads department content from json
        /// </summary>
        /// <param name="index"></param>
        public void LoadUpgradeContent(Json.UpgradableContent content)
        {
            upgradeContent = content;
        }

        public virtual void UpgradeDepartmentAssets()
        {
            if (upgradeUnlockList.Length == 0)
                return;
            int index = DeaprtmentUpgradeJson.upgradable_contents.IndexOf(UpgradeContent);
            if (upgradeUnlockList.Length <= index)
                return;
            if(upgradeUnlockList[index] != null)
                upgradeUnlockList[index].SetActive(true);
        }

        public virtual void UpgradeDepartmentAssetsWhileGameLoad()
        {
            if (upgradeUnlockList.Length == 0)
                return;
            int index = DeaprtmentUpgradeJson.upgradable_contents.IndexOf(UpgradeContent);
            if (upgradeUnlockList.Length <= index)
                return;
            for (int i = 0; i < index; i++)
            {
                if (upgradeUnlockList[i] != null)
                    upgradeUnlockList[i].SetActive(true);
            }
        }

        /// <summary>
        /// Return true if all doctor slary not upgraded to max value
        /// </summary>
        /// <returns></returns>
        public bool IsSalaryUpgradeAvailable(int SalaryIncreaseIndex)
        {
            return (reduceWorkTimeByPercent * SalaryIncreaseIndex) < maxCanReduceWorkTimeByPercent;
        }

        /// <summary>
        /// Get staff work time
        /// </summary>
        /// <returns></returns>
        public virtual float GetWorkTime(int salaryIncreaseIndex)
        {
            float workTime = initialWorkTime - GetReducedTimePercentage(salaryIncreaseIndex);
            return Mathf.Clamp(workTime, minWorkTime, workTime);
        }

        float GetReducedTimePercentage(int salaryIncreaseIndex)
        {
            return ((reduceWorkTimeByPercent * salaryIncreaseIndex) / maxCanReduceWorkTimeByPercent) * initialWorkTime;
        }
    }
}
