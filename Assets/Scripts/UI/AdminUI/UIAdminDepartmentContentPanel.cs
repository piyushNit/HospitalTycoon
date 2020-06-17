using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Management.UI
{
    public class UIAdminDepartmentContentPanel : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI txtDepartmentName;
        [SerializeField] TextMeshProUGUI txtDepartmentDescription;
        [SerializeField] Image imgDepartmentIcon;
        [SerializeField] TextMeshProUGUI txtBuyValue;

        [SerializeField] Button btnBuy;
        [SerializeField] GameObject alreadyPurchasedIcon;

        UI_AdminDepartment uiAdminDepartment;

        Management.UI.Admin.AdminDepartmentsScriptable.DepartmentsDataForUI departmentData;
        public Management.Hospital.Core.DepartmentType DepartmentType { get => departmentData.departmentType; }

        System.Action<Management.Hospital.Core.DepartmentType> OnBuyBtnClicked;

        public bool IsAlreadyPurchased { get => alreadyPurchasedIcon.activeSelf; }

        /// <summary>
        /// UI callback from button clicked
        /// </summary>
        public void CbOnBuyClicked()
        {
            if (!uiAdminDepartment.UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(departmentData.buyValue))
                return;
            if (OnBuyBtnClicked != null)
                OnBuyBtnClicked(DepartmentType);
        }

        public void Initilize(Management.UI.UI_AdminDepartment adminDepartment,
                                Management.UI.Admin.AdminDepartmentsScriptable.DepartmentsDataForUI _departmentData,
                                bool isUnlocked, System.Action<Management.Hospital.Core.DepartmentType> _cbOnBuyBtnClicked)
        {
            uiAdminDepartment = adminDepartment;
            alreadyPurchasedIcon.SetActive(isUnlocked);
            btnBuy.gameObject.SetActive(!isUnlocked);
            OnBuyBtnClicked = _cbOnBuyBtnClicked;

            departmentData = _departmentData;

            txtDepartmentDescription.text = departmentData.description;
            txtDepartmentName.text = departmentData.title;
            imgDepartmentIcon.sprite = departmentData.sprIcon;
            txtBuyValue.text = departmentData.buyValue.ToString();

            btnBuy.interactable = uiAdminDepartment.UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(departmentData.buyValue);
        }

        public void UpdateBuyButton()
        {
            btnBuy.interactable = uiAdminDepartment.UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(departmentData.buyValue);
        }

        /// <summary>
        /// Hides the buy button and shows purchased icon
        /// </summary>
        public void SetUnlocked()
        {
            alreadyPurchasedIcon.SetActive(true);
            btnBuy.gameObject.SetActive(false);
        }
    }
}