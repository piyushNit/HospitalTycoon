using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.UI
{
    public class UI_AdminDepartment : UiCoreHolder
    {
        [SerializeField] Transform contextParent;
        [SerializeField] UIAdminDepartmentContentPanel uiAdminDepartmentContentPanelPrefab;
        [SerializeField] Management.UI.Admin.AdminDepartmentsScriptable adminDepartmentScriptable;

        List<UIAdminDepartmentContentPanel> uiAdminDepartmentsList = new List<UIAdminDepartmentContentPanel>();

        Management.Hospital.Core.DepartmentType currUnlockingDepartment;

        public override void OnEnable()
        {
            base.OnEnable();
            InstantiateDepartmentPanels();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            for (int i = 0; i < uiAdminDepartmentsList.Count; i++)
            {
                Destroy(uiAdminDepartmentsList[i].gameObject);
            }
            uiAdminDepartmentsList.Clear();
        }

        void InstantiateDepartmentPanels()
        {
            for (int i = 0; i < adminDepartmentScriptable.DepartmentDataList.Count; i++)
            {
                UIAdminDepartmentContentPanel adminPanelInstance = Instantiate(uiAdminDepartmentContentPanelPrefab, contextParent) as UIAdminDepartmentContentPanel;
                bool isUnlocked = uiHolder.StateController.HospitalManager.IsDepartmentUnlocked(adminDepartmentScriptable.DepartmentDataList[i].departmentType);
                adminPanelInstance.Initilize(this, adminDepartmentScriptable.DepartmentDataList[i], isUnlocked, CbOnDepartmentBuyClicked);
                uiAdminDepartmentsList.Add(adminPanelInstance);
            }
        }

        void CbOnDepartmentBuyClicked(Management.Hospital.Core.DepartmentType departmentType)
        {
            currUnlockingDepartment = departmentType;
            uiHolder.StateController.HospitalManager.UnlockNewDepartment(departmentType, CbUnlockeFuncCallback);

            decimal departmentUnlockCost = adminDepartmentScriptable.DepartmentDataList.Find(obj => obj.departmentType == departmentType).buyValue;
            uiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.DeductCash(departmentUnlockCost);
            uiHolder.ReloadPlayerScoreInHeaderUI();

            for (int i = 0; i < uiAdminDepartmentsList.Count; i++)
            {
                if (!uiAdminDepartmentsList[i].IsAlreadyPurchased)
                {
                    uiAdminDepartmentsList[i].UpdateBuyButton();
                }
            }
        }

        void CbUnlockeFuncCallback(bool isUnlockd)
        {
            if(isUnlockd)
                uiAdminDepartmentsList.Find(obj => obj.DepartmentType == currUnlockingDepartment).SetUnlocked();
        }
    }
}