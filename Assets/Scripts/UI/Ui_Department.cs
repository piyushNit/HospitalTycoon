using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Management.UI
{
    public class Ui_Department : UiCoreHolder
    {
        Management.Hospital.Department department;
        Management.Hospital.PaymentDepartment paymentDepartment;

        #region UI_REFERENCES
        [Header("player curr Stats")]
        [SerializeField] Image imgCurrUpgradeIcon;
        [SerializeField] TextMeshProUGUI txtDepartmentTitle;
        [SerializeField] TextMeshProUGUI txtUpgradeTitle;
        [SerializeField] TextMeshProUGUI txtUpgradeDescription;
        [SerializeField] TextMeshProUGUI txtLevelValue;
        [SerializeField] TextMeshProUGUI txtStaffValue;
        [SerializeField] TextMeshProUGUI txtBaseIncomeValue;
        [SerializeField] TextMeshProUGUI txtTreatmentTime;
        [SerializeField] TextMeshProUGUI txtNextUpgradeLevelValue;
        [SerializeField] Slider sliderNextUpgrade;
        [SerializeField] TextMeshProUGUI txtNextUpgradeIncome;

        [Header("Department Stats")]
        [SerializeField] TextMeshProUGUI txtPatientTreated;
        [SerializeField] TextMeshProUGUI txtDepartmentTotalValue;

        [Header("UI Buttons")]
        [SerializeField] Button btnDeaprtmentUpgrade;
        [SerializeField] Button btnHireEmployee;
        [SerializeField] Button btnStaffSalary;

        [Header("Staff Info")]
        [SerializeField] TextMeshProUGUI txtDepartmentUpgradeRquiredValue;
        [SerializeField] TextMeshProUGUI txtHireEmployeeRequiredValue;
        [SerializeField] Slider sliderStaffHire;
        [SerializeField] TextMeshProUGUI txtStaffSliderValue;
        [SerializeField] TextMeshProUGUI txtStaffSalaryRequiredValue;

        [Header("Staff Salary")]
        [SerializeField] Slider sliderStaffSalaryHike;
        [SerializeField] TextMeshProUGUI txtStaffSalaryMax;

        [Header("Info")]
        [SerializeField] UiDepartmentInfoTab uiDepartmentInfoTab;
        [Header("Other")]
        [Tooltip("Will hide staff hire panel in departments, only will be active in consulation and pharmacy department")]
        [SerializeField] GameObject panelHireStaff;
        #endregion

        bool saveNeeded = false;
        Management.SaveLoad.HospitalDepartentSave hospitalDepartmentData = null;
        Management.Hospital.Json.UpgradableContent upgradeContent = null;
        Management.Hospital.Json.DepartmenStaffAndSalarytData departmentStaffAndSalaryData = null;

        decimal newLevelUnlockaValue = -1;
        double employeeHireCost = -1;
        double employeeSalaryCost = -1;

        Coroutine enoughCashCheckCorouting = null;

        public override void OnDisable()
        {
            base.OnDisable();
            department = null;
            paymentDepartment = null;
            hospitalDepartmentData = null;
            upgradeContent = null;
            newLevelUnlockaValue = -1;
        }

        /// <summary>
        /// Loads department upgrade values from JSON
        /// </summary>
        /// <param name="_department"></param>
        public void LoadDepartmentData(Management.Hospital.Department _department)
        {
            saveNeeded = false;
            newLevelUnlockaValue = -1;

            department = _department;
            if (department == null)
            {
                Debug.LogError("Ui_Department: base departent is null in");
                return;
            }

            panelHireStaff.SetActive(false);

            hospitalDepartmentData = uiHolder.StateController.GameManager.MasterLoader.HospitalDepartmentSaveModel.GetData(department.DepartmentType);
            departmentStaffAndSalaryData = Arch.Json.JsonReader.LoadJson<Management.Hospital.Json.DepartmenStaffAndSalarytData>
                                           (uiHolder.StateController.HospitalJsonDataScriptable.DepartmentStaffAndSalaryJson.text);

            if (_department.DeaprtmentUpgradeJson == null || hospitalDepartmentData == null)
            {
                ToggleAllButtons(false);
            }

            txtDepartmentTitle.text = _department.DeaprtmentUpgradeJson.department;
            txtBaseIncomeValue.text = _department.DeaprtmentUpgradeJson.base_income.ToString();

            LoadNextUpgradeData(department);

            sliderStaffHire.value = department.GetDoctorAvailibilityPercent();
            UpdateStaffCost((Management.Hospital.BaseDepartment)department);
        }

        public void LoadDepartmentData(Management.Hospital.PaymentDepartment _paymentDepartment)
        {
            saveNeeded = false;
            paymentDepartment = _paymentDepartment;
            if (paymentDepartment == null)
            {
                Debug.LogError("Ui_Department: base departent is null in");
                return;
            }
            panelHireStaff.SetActive(true);
            hospitalDepartmentData = uiHolder.StateController.GameManager.MasterLoader.HospitalDepartmentSaveModel.GetData(paymentDepartment.DepartmentType);
            departmentStaffAndSalaryData = Arch.Json.JsonReader.LoadJson<Management.Hospital.Json.DepartmenStaffAndSalarytData>
                                           (uiHolder.StateController.HospitalJsonDataScriptable.DepartmentStaffAndSalaryJson.text);

            txtDepartmentTitle.text = paymentDepartment.DeaprtmentUpgradeJson.department;
            txtBaseIncomeValue.text = paymentDepartment.DeaprtmentUpgradeJson.base_income.ToString();

            LoadNextUpgradeData(paymentDepartment);

            sliderStaffHire.value = paymentDepartment.GetStaffPercentage();

            UpdateStaffCost((Management.Hospital.BaseDepartment)paymentDepartment);
        }

        void UpdateStaffCost(Management.Hospital.BaseDepartment baseDepartment)
        {
            int upcomingStaffHireCount = uiHolder.StateController.GameManager.MasterLoader.HospitalDepartmentSaveModel.GetTotalStaffCount(baseDepartment.DepartmentType) + 1;
            int upcomingSalaryIndex = uiHolder.StateController.GameManager.MasterLoader.HospitalDepartmentSaveModel.GetSalaryIndex(baseDepartment.DepartmentType) + 1;

            switch (baseDepartment.DepartmentType)
            {
                case Hospital.Core.DepartmentType.ENT:
                    employeeSalaryCost = (departmentStaffAndSalaryData.ENTSalaryHikeBaseCost * upcomingSalaryIndex) * departmentStaffAndSalaryData.ENTSalaryHikeMultiplyBy;
                    break;
                case Hospital.Core.DepartmentType.DENTIST:
                    employeeSalaryCost = (departmentStaffAndSalaryData.DentalSalayHikeBaseCost * upcomingSalaryIndex) * departmentStaffAndSalaryData.DentalHikeMultiplyBy;
                    break;
                case Hospital.Core.DepartmentType.SURGEON:
                    employeeSalaryCost = (departmentStaffAndSalaryData.SurgeonSalayHikeBaseCost * upcomingSalaryIndex) * departmentStaffAndSalaryData.SurgeonHikeMultiplyBy;
                    break;
                case Hospital.Core.DepartmentType.GYNAECOLOGIST:
                    employeeSalaryCost = (departmentStaffAndSalaryData.GynecologySalaryHikeBaseCost * upcomingSalaryIndex) * departmentStaffAndSalaryData.GynecologySalaryHikeMultiplyBy;
                    break;
                case Hospital.Core.DepartmentType.PAEDIATRICS:
                    employeeSalaryCost = (departmentStaffAndSalaryData.PaediatricsSalaryHikeBaseCost * upcomingSalaryIndex) * departmentStaffAndSalaryData.PaediatricsSalaryHikeMultiplyBy;
                    break;
                case Hospital.Core.DepartmentType.EYE_SPECILIST:
                    employeeSalaryCost = (departmentStaffAndSalaryData.EyeSalaryHikeBaseCost * upcomingSalaryIndex) * departmentStaffAndSalaryData.EyeSalaryHikeMultiplyBy;
                    break;
                case Hospital.Core.DepartmentType.ORTHOPAEDICS:
                    employeeSalaryCost = (departmentStaffAndSalaryData.OrthopedicsSalayHikeBaseCost * upcomingSalaryIndex) * departmentStaffAndSalaryData.OrthopedicsHikeMultiplyBy;
                    break;
                case Hospital.Core.DepartmentType.NEUROLOGIST:
                    employeeSalaryCost = (departmentStaffAndSalaryData.NurologySalayHikeBaseCost * upcomingSalaryIndex) * departmentStaffAndSalaryData.NurologyHikeMultiplyBy;
                    break;
                case Hospital.Core.DepartmentType.CARDIOLOGIST:
                    employeeSalaryCost = (departmentStaffAndSalaryData.CardiologySalaryHikeBaseCost * upcomingSalaryIndex) * departmentStaffAndSalaryData.CardiologySalaryHikeMultiplyBy;
                    break;
                case Hospital.Core.DepartmentType.PSYCHIATRIST:
                    break;
                case Hospital.Core.DepartmentType.SKIN_SPECIALIST:
                    employeeSalaryCost = (departmentStaffAndSalaryData.SkinSalayHikeBaseCost * upcomingSalaryIndex) * departmentStaffAndSalaryData.SkinHikeMultiplyBy;
                    break;
                case Hospital.Core.DepartmentType.V_D:
                    break;
                case Hospital.Core.DepartmentType.PLASTIC_SURGEON:
                    employeeSalaryCost = (departmentStaffAndSalaryData.PlasticSurgenSalaryHikeBaseCost * upcomingSalaryIndex) * departmentStaffAndSalaryData.PlasticSurgenSalaryHikeMultiplyBy;
                    break;
                case Hospital.Core.DepartmentType.PHARMASY:
                    employeeSalaryCost = (departmentStaffAndSalaryData.PharmacySalaryHikeBaseCost * upcomingSalaryIndex) * departmentStaffAndSalaryData.PharmacySalaryHikeCostMultiplyBy;
                    employeeHireCost = (departmentStaffAndSalaryData.PharmacyStaffHireBaseCost * upcomingStaffHireCount) * departmentStaffAndSalaryData.PharmacyStaffHireCostMultiplyBy;
                    break;
                case Hospital.Core.DepartmentType.CONSULTATION_FEES:
                    employeeSalaryCost = (departmentStaffAndSalaryData.ConsulationSalaryHikeBaseCost * upcomingSalaryIndex) * departmentStaffAndSalaryData.ConsulationSalaryHikeCostMultiplyBy;
                    employeeHireCost = (departmentStaffAndSalaryData.ConsulationStaffHireBaseCost * upcomingStaffHireCount) * departmentStaffAndSalaryData.ConsulationStaffHireCostMultiplyBy;
                    break;
            }

            txtStaffSalaryRequiredValue.text = Utils.FormatWithMeasues((decimal)employeeSalaryCost);
            txtHireEmployeeRequiredValue.text = Utils.FormatWithMeasues((decimal)employeeHireCost);

            btnHireEmployee.interactable = UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(employeeHireCost);
            btnStaffSalary.interactable = UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(employeeSalaryCost);
        }

        void LoadNextUpgradeData(Management.Hospital.BaseDepartment baseDepartment)
        {
            try
            {
                upgradeContent = baseDepartment.GetUpgradeContentFromLevel(hospitalDepartmentData.CurrLevel);
                int currLevelIndex = baseDepartment.DeaprtmentUpgradeJson.upgradable_contents.IndexOf(upgradeContent);
                Hospital.Json.UpgradableContent nextUpgradeContent = null;
                if (currLevelIndex < baseDepartment.DeaprtmentUpgradeJson.upgradable_contents.Count - 1)
                {
                    nextUpgradeContent = baseDepartment.DeaprtmentUpgradeJson.upgradable_contents[currLevelIndex + 1];
                }
                UpdateNextLevelUnlockValue(hospitalDepartmentData.CurrLevel + 1, baseDepartment.DeaprtmentUpgradeJson.base_income, uiHolder.StateController.DepartmentUpgradeScriptable.GetLevelIncreasePercentage(baseDepartment.DepartmentType));

                // This level upgrade
                txtDepartmentUpgradeRquiredValue.text = Utils.FormatWithMeasues(newLevelUnlockaValue);
                imgCurrUpgradeIcon.sprite = uiHolder.StateController.DepartmentUpgradeScriptable.GetIconFromIndex(baseDepartment.DepartmentType, currLevelIndex);

                txtUpgradeTitle.text = upgradeContent.upgrade_name;
                txtUpgradeDescription.text = upgradeContent.description;
                txtLevelValue.text = hospitalDepartmentData.CurrLevel.ToString();

                txtBaseIncomeValue.text = baseDepartment.DeaprtmentUpgradeJson.base_income.ToString();
                txtTreatmentTime.text = upgradeContent.time.ToString();
                UpdateStaffUI();

                //Staff salary
                txtStaffSalaryMax.text = baseDepartment.MaxCanReduceWorkTimeByPercent.ToString() + "%";
                UpdateSalryHikeUI(baseDepartment);

                txtPatientTreated.text = (60 / (int)upgradeContent.time).ToString() + "/Min";

                //Next Upgrade
                if (nextUpgradeContent != null)
                {
                    txtNextUpgradeIncome.text = "X" + nextUpgradeContent.income_multiplyer.ToString();
                    txtNextUpgradeLevelValue.text = nextUpgradeContent.level.ToString();

                    sliderNextUpgrade.minValue = upgradeContent.level;
                    sliderNextUpgrade.maxValue = nextUpgradeContent.level;
                    sliderNextUpgrade.value = hospitalDepartmentData.CurrLevel;
                }
                else
                {
                    txtNextUpgradeIncome.text = "-";
                    txtNextUpgradeLevelValue.text = "-";

                    sliderNextUpgrade.minValue = 0;
                    sliderNextUpgrade.maxValue = 0;
                    sliderNextUpgrade.value = 0;
                }

                ToggleAllButtons(true);
            }
            catch (System.Exception e)
            {
                ToggleAllButtons(false);
                Debug.LogError("Department upgrade data load failed");
                Debug.LogError(e.Message.ToString());
            }
        }

        void UpdateNextLevelUnlockValue(int level, int baseIncome, float increasePercentage)
        {
            newLevelUnlockaValue = (decimal)((baseIncome + level) + (baseIncome * level) * (level * increasePercentage));
        }

        private void UpdateStaffUI()
        {
            int totalSlots = department != null ? department.GetTotalStaffSlots() : paymentDepartment.GetTotalCashierSlots();
            string value = hospitalDepartmentData.StaffCount.ToString() + "/" + totalSlots.ToString();
            txtStaffValue.text = value;
            txtStaffSliderValue.text = value;
        }

        void ToggleAllButtons(bool flag = true)
        {
            btnDeaprtmentUpgrade.interactable = flag;
            btnHireEmployee.interactable = flag;
            btnStaffSalary.interactable = flag;
        }

        #region UI_BUTTON_CALLBACKS
        public override void CbOnBackBtnClicked()
        {            
            UiHolder.UiUtilities.LoadingSpinner(true);
            if (saveNeeded)
            {
                //Sending analytics event
                SendEvents(department != null ? department.DepartmentType : paymentDepartment.DepartmentType,
                           hospitalDepartmentData.CurrLevel);

                UiHolder.StateController.GameManager.SaveGameData();
            }

            UiHolder.PlayerIncomeTab.UpdateIncomePerMin();
            base.CbOnBackBtnClicked();
            UiHolder.UiUtilities.LoadingSpinner(false);

            if (uiHolder.StateController.FTUEManager.IsFTUERunning && uiHolder.StateController.FTUEManager.CurrentFtueType == FTUE.FTUEType.FTUE_ENT_UI_CLOSE_CLICK)
                uiHolder.StateController.FTUEManager.SkipToNext();
        }

        public void CbOnInfoClicked()
        {
            if (department != null)
                uiDepartmentInfoTab.ShowInfoTab(UiHolder.StateController.DepartmentUpgradeScriptable.GetIconScriptable(department.DepartmentType), department);
            else
                uiDepartmentInfoTab.ShowInfoTab(UiHolder.StateController.DepartmentUpgradeScriptable.GetIconScriptable(paymentDepartment.DepartmentType), paymentDepartment);
        }

        public void CbOnBuyUpgradeDepartmentClicked()
        {
            Debug.Log("Upgrade department clicked");
            if (newLevelUnlockaValue == -1)
                return;

            if (!uiHolder.StateController.FTUEManager.IsFTUERunning && !UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(newLevelUnlockaValue))
            {
                btnDeaprtmentUpgrade.interactable = false;
                if(enoughCashCheckCorouting == null)
                    enoughCashCheckCorouting = StartCoroutine(CheckForEnoughCash());
                return;
            }
            if(!uiHolder.StateController.FTUEManager.IsFTUERunning)
                uiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.DeductCash(newLevelUnlockaValue);

            hospitalDepartmentData.UpdateCurrLevel();
            LoadNextUpgradeData(department != null ? (Management.Hospital.BaseDepartment)department : (Management.Hospital.BaseDepartment)paymentDepartment);
            UiHolder.ReloadPlayerScoreInHeaderUI();

            UiHolder.StateController.GameAudioManager.PlaySound(Audio.AudioType.UPGRADE_BTN_CLICKED);

            //Update department content
            if (department != null)
            {
                department.LoadUpgradeContent(upgradeContent);
                department.UpgradeDepartmentAssets();
            }
            else if (paymentDepartment != null)
            {
                paymentDepartment.LoadUpgradeContent(upgradeContent);
                paymentDepartment.UpgradeDepartmentAssets();
            }

            if (department != null
                && department.DepartmentType == Hospital.Core.DepartmentType.ENT
                && uiHolder.StateController.FTUEManager.IsFTUERunning)
            {
                uiHolder.StateController.FTUEManager.SkipToNext();
            }

            saveNeeded = true;
        }

        IEnumerator CheckForEnoughCash()
        {
            WaitForSeconds waitSec = new WaitForSeconds(2);
            while (gameObject.activeSelf)
            {
                if (UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(newLevelUnlockaValue))
                {
                    btnDeaprtmentUpgrade.interactable = true;
                }
                if (uiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(employeeHireCost))
                {
                    btnHireEmployee.interactable = true;
                }
                if (uiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(employeeSalaryCost))
                {
                    btnStaffSalary.interactable = true;
                }
                yield return waitSec;
            }
        }

        public void CbOnBtnHireEmployeeClicked()
        {
            if (!uiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(employeeHireCost))
            {
                btnHireEmployee.interactable = false;
                if (enoughCashCheckCorouting == null)
                    enoughCashCheckCorouting = StartCoroutine(CheckForEnoughCash());
                return;
            }

            UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.DeductCash((decimal)employeeHireCost);
            UiHolder.ReloadPlayerScoreInHeaderUI();

            if (department != null && department.CanAddDoctor())
            {
                Management.Doctor.Core.DoctorType doctorType = uiHolder.StateController.GameManager.WhichDoctor(department.DepartmentType);
                uiHolder.StateController.GameManager.SpawnDoctor(doctorType, uiHolder.StateController);
                sliderStaffHire.value = department.GetDoctorAvailibilityPercent();
                uiHolder.StateController.HospitalManager.HospitalBuilding.SaveGameDataWithStaffHiring(department.DepartmentType);

                UiHolder.StateController.GameAudioManager.PlaySound(Audio.AudioType.UPGRADE_BTN_CLICKED);
                UpdateStaffCost(department);
            }
            else if(paymentDepartment != null && paymentDepartment.CanAddStaff())
            {
                if (paymentDepartment.DepartmentType == Hospital.Core.DepartmentType.CONSULTATION_FEES)
                {
                    UiHolder.StateController.GameManager.SpawnCashierInConsulationDepartment(UiHolder.StateController);
                }
                else
                {
                    UiHolder.StateController.GameManager.SpawnCashierInPharmacyDepartment(UiHolder.StateController);
                }
                sliderStaffHire.value = paymentDepartment.GetStaffPercentage();
                uiHolder.StateController.HospitalManager.HospitalBuilding.SaveGameDataWithStaffHiring(paymentDepartment.DepartmentType);

                UiHolder.StateController.GameAudioManager.PlaySound(Audio.AudioType.UPGRADE_BTN_CLICKED);
                UpdateStaffCost(paymentDepartment);
            }
            UpdateStaffUI();
            saveNeeded = true;
        }

        public void CbOnBtnEmployeeSalaryHikeClicked()
        {
            if (!uiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(employeeSalaryCost))
            {
                btnStaffSalary.interactable = false;
                if (enoughCashCheckCorouting == null)
                    enoughCashCheckCorouting = StartCoroutine(CheckForEnoughCash());
                return;
            }

            if ((department != null && !department.IsSalaryUpgradeAvailable(hospitalDepartmentData.SalaryIncreaseIndexCount))
            || (paymentDepartment != null && !paymentDepartment.IsSalaryUpgradeAvailable(hospitalDepartmentData.SalaryIncreaseIndexCount)))
                return;

            UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.DeductCash((decimal)employeeSalaryCost);
            UiHolder.ReloadPlayerScoreInHeaderUI();

            hospitalDepartmentData.IncreaseSalaryIndex();

            UiHolder.StateController.GameAudioManager.PlaySound(Audio.AudioType.UPGRADE_BTN_CLICKED);

            UpdateSalryHikeUI(department != null ? department : (Management.Hospital.BaseDepartment)paymentDepartment);

            UpdateStaffCost(department != null ? (Management.Hospital.BaseDepartment)department : (Management.Hospital.BaseDepartment)paymentDepartment);

            saveNeeded = true;
        }

        void UpdateSalryHikeUI(Management.Hospital.BaseDepartment baseDepartment)
        {
            sliderStaffSalaryHike.value = hospitalDepartmentData.SalaryIncreaseIndexCount / baseDepartment.MaxCanReduceWorkTimeByPercent;
            float costForNextSalaryHike = baseDepartment.DeaprtmentUpgradeJson.salary_initial_cost * (hospitalDepartmentData.SalaryIncreaseIndexCount + 1);
            btnStaffSalary.interactable = UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(costForNextSalaryHike);
            txtStaffSalaryRequiredValue.text = Utils.FormatWithMeasues((decimal)costForNextSalaryHike);
        }

        #endregion

        #region External_Services
        void SendEvents(Management.Hospital.Core.DepartmentType departmentType, int level)
        {
            System.Collections.Generic.Dictionary<string, object> analyticsData = new Dictionary<string, object>();
            analyticsData["Department"] = departmentType.ToString();
            analyticsData["Department_Level"] = level.ToString();

            Management.Services.AnalyticsManager.Instance.CustomEvent(Management.Services.EventStrings.DEPARTMENT_UPGRADE, analyticsData);
        }
        #endregion

        #region FTUE
        /// <summary>
        /// Disables the upgrade and staff salary hike button
        /// </summary>
        public void DisableUpgradeAndStffSalaryBtn()
        {
            btnDeaprtmentUpgrade.interactable = false;
            btnStaffSalary.interactable = false;
        }
        #endregion
    }
}