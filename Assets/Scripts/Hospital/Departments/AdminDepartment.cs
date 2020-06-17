using System.Collections;
using System.Collections.Generic;
using Arch.Core;
using UnityEngine;

namespace Management.Hospital
{
    public class AdminDepartment : BaseDepartment
    {
        [Header("Admin Department Specific")]
        [SerializeField] Management.UI.Admin.AdminDepartmentsScriptable adminDepartmentScriptable;
        [SerializeField] GameObject exclamationMark;

        private void OnEnable()
        {
            Management.GameManager.OnPlayerIncomeUpdated += OnPlayerIncomeUpdated;
        }

        private void OnDisable()
        {
            Management.GameManager.OnPlayerIncomeUpdated -= OnPlayerIncomeUpdated;
        }

        public override void LoadDepartment(StateController stateController)
        {
            base.LoadDepartment(stateController);
            OnPlayerIncomeUpdated();
        }

        void OnPlayerIncomeUpdated()
        {
            bool canShowExclamationMark = false;
            for (int i = 0; i < adminDepartmentScriptable.DepartmentDataList.Count; i++)
            {
                if (adminDepartmentScriptable.DepartmentDataList[i].departmentType == Core.DepartmentType.ENT)
                    continue;
                if (refStateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(adminDepartmentScriptable.DepartmentDataList[i].buyValue))
                {
                    canShowExclamationMark = true;
                    break;
                }
            }

            exclamationMark.SetActive(canShowExclamationMark);
        }
    }
}