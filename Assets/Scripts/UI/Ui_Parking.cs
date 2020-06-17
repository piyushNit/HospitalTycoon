using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Management.UI
{
    public class Ui_Parking : UiCoreHolder
    {
        #region UI_REFERENCES
        [Header("Parking")]
        [SerializeField] TextMeshProUGUI txtParkingCost;
        [SerializeField] TextMeshProUGUI txtParkingSpace;
        [SerializeField] Button btnParkingUnlock;

        [Header("Advertisement")]
        [SerializeField] TextMeshProUGUI txtAdvertisementCost;
        [SerializeField] TextMeshProUGUI txtCarEntryPerMin;
        [SerializeField] Slider sliderAdvertisement;
        [SerializeField] TextMeshProUGUI txtPatientsPerMin;
        [SerializeField] Button btnAdvertisement;
        #endregion

        double nextParkingUnlockCost = 0;
        double nextAdvertisementCost = 0;

        public override void OnEnable()
        {
            base.OnEnable();
            UpdateParkingData();
            UpdateAdvertisementData();
        }

        #region UI_CALLBACKS
        public void CbOnParkingUpgradeClicked()
        {
            if (!uiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(nextParkingUnlockCost))
                return;

            int currentUnlockedSlotsCount = uiHolder.StateController.ParkingManager.ParkingSlotHandler.GetCountOfUnlockedParkingSlots();
            if (currentUnlockedSlotsCount <= uiHolder.StateController.ParkingManager.ParkingSlotHandler.TotalParingSlotsCount)
            {
                uiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.DeductCash((decimal)nextParkingUnlockCost);
                UiHolder.ReloadPlayerScoreInHeaderUI();
                uiHolder.StateController.ParkingManager.ParkingSlotHandler.UnlockNewParkingSlot(true);
                UiHolder.StateController.GameAudioManager.PlaySound(Audio.AudioType.UPGRADE_BTN_CLICKED);
                UpdateParkingData();
            }

            if (UiHolder.StateController.FTUEManager.IsFTUERunning && UiHolder.StateController.FTUEManager.CurrentFtueType == FTUE.FTUEType.FTUE_PARKING_UPGRADE_CLICK)
            {
                UiHolder.StateController.FTUEManager.SkipToNext();
            }
        }

        public void CbOnAdvertisingClicked()
        {
            if (!UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(nextAdvertisementCost))
                return;
            if (UiHolder.StateController.GameManager.MasterLoader.ParkingUnitSaveModel.GetPatientPerMinWithoutOffset >= uiHolder.StateController.ParkingManager.ParkingJson.AdvertisementMaxPatientsPerMinute)
                return;

            UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.DeductCash((decimal)nextAdvertisementCost);
            UiHolder.ReloadPlayerScoreInHeaderUI();
            UiHolder.StateController.ParkingManager.IncreasePatientPerMinAfterAdvertisement();
            UiHolder.StateController.GameAudioManager.PlaySound(Audio.AudioType.UPGRADE_BTN_CLICKED);
            UpdateAdvertisementData();
        }

        public override void CbOnBackBtnClicked()
        {
            base.CbOnBackBtnClicked();
            UiHolder.StateController.ParkingManager.ParkingSlotHandler.SaveGameData();
            uiHolder.CloseAllAndOpenHUD();
        }
        #endregion

        void UpdateAdvertisementData()
        {
            bool isAdvertisementLimitReached = UiHolder.StateController.GameManager.MasterLoader.ParkingUnitSaveModel.GetPatientPerMinWithoutOffset >= uiHolder.StateController.ParkingManager.ParkingJson.AdvertisementMaxPatientsPerMinute;
            txtCarEntryPerMin.text = uiHolder.StateController.GameManager.MasterLoader.ParkingUnitSaveModel.patientsPerMin.ToString() + "/MIN";
            if (isAdvertisementLimitReached)
            {
                btnAdvertisement.interactable = false;
                txtAdvertisementCost.text = "MAX";
            }
            else
            {
                int patientPerMin = UiHolder.StateController.GameManager.MasterLoader.ParkingUnitSaveModel.GetPatientPerMinWithoutOffset;
                sliderAdvertisement.value = patientPerMin / (float)uiHolder.StateController.ParkingManager.ParkingJson.AdvertisementMaxPatientsPerMinute;
                txtPatientsPerMin.text = patientPerMin.ToString() + "/MIN";

                //Calculate cost
                double totalCost = UiHolder.StateController.ParkingManager.ParkingJson.AdvertisementBaseCost * UiHolder.StateController.GameManager.MasterLoader.ParkingUnitSaveModel.GetPatientPerMinWithoutOffset;
                nextAdvertisementCost = totalCost + ((uiHolder.StateController.ParkingManager.ParkingJson.AdvertisementCostIncreasePercent / 100.0f) * totalCost);
                if (nextAdvertisementCost <= 0)
                    nextAdvertisementCost = UiHolder.StateController.ParkingManager.ParkingJson.AdvertisementBaseCost;
                txtAdvertisementCost.text = Utils.FormatWithMeasues((decimal)nextAdvertisementCost);

                UpdateButtons();
            }
        }

        void UpdateParkingData()
        {
            int totalUnclokedSlots = UiHolder.StateController.ParkingManager.ParkingSlotHandler.GetCountOfUnlockedParkingSlots();
            txtParkingSpace.text = totalUnclokedSlots.ToString() + "/" + UiHolder.StateController.ParkingManager.ParkingJson.MaxParkingSlots.ToString();
            if (totalUnclokedSlots >= UiHolder.StateController.ParkingManager.ParkingJson.MaxParkingSlots)
            {
                btnParkingUnlock.interactable = false;
                txtParkingCost.text = "MAX";
            }
            else
            {
                nextParkingUnlockCost = GetParkingUnlockCost(totalUnclokedSlots);
                txtParkingCost.text = Utils.FormatWithMeasues((decimal)nextParkingUnlockCost);

                UpdateButtons();
            }
        }

        public double GetParkingUnlockCost(int totalUnclokedSlots)
        {
            double totalCost = UiHolder.StateController.ParkingManager.ParkingJson.ParkingBaseCost * totalUnclokedSlots / UiHolder.StateController.ParkingManager.ParkingJson.UnlockParkingPerUpgrade;
            return totalCost + ((UiHolder.StateController.ParkingManager.ParkingJson.ParkingCostIncreasePercent / 100.0f) * totalCost);
        }

        void UpdateButtons()
        {
            btnParkingUnlock.interactable = UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(nextParkingUnlockCost);
            if (UiHolder.StateController.FTUEManager.IsFTUERunning && UiHolder.StateController.FTUEManager.CurrentFtueType == FTUE.FTUEType.FTUE_PARKING_UPGRADE_CLICK)
            {
                btnAdvertisement.interactable = false;
            }
            else
            {
                btnAdvertisement.interactable = UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(nextAdvertisementCost);
            }
        }

        
        public void FTUEReloadAdvertisement()
        {
            btnParkingUnlock.interactable = false;
            btnAdvertisement.interactable = UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(nextAdvertisementCost);
        }
    }
}