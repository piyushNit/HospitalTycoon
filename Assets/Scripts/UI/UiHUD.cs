using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Management.UI
{
    public class UiHUD : UiCoreHolder
    {
        [SerializeField] Transform boosterParentTrans;
        [SerializeField] UiBooster uiBoosterPrefab;

        [SerializeField] Transform boosterTimerParentTrans;
        [SerializeField] UiBoosterTimer uiBooterTimerPrefab;

        List<UiBooster> uiBoostersList;

        public static System.Action<Management.Hospital.BoosterType> AddBooster;

        PopupTimeBooster popupTimeBoosterInstance = null;

        #region UI_CALLBACKS
        //ExtraPatientAdPopup popupExtraPatientInstance;
        public void CnOnExtraPatientVideoAdClicked()
        {
            uiHolder.ShowUi(UiType.Ad_Campaign);
            //popupExtraPatientInstance = uiHolder.PopupContainer.GetPopupInstance(PopupType.EXTRA_PATIENT, uiHolder.transform) as ExtraPatientAdPopup;
            //popupExtraPatientInstance.Initilize(uiHolder, CbOnExtraPatientVideoAdCallback);
        }
        private void CbOnExtraPatientVideoAdCallback(int index)
        {
            //popupExtraPatientInstance.Close();
        }
        #endregion

        public override void OnEnable()
        {
            base.OnEnable();
            AddBooster = CbOnAddBooster;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            AddBooster = null;
        }

        [ContextMenu("Test booster")]
        public void CashBooster()
        {
            CbOnAddBooster(Hospital.BoosterType.Cashier_Time_Booster);
        }

        void CbOnAddBooster(Management.Hospital.BoosterType boosterType)
        {
            if (uiBoostersList == null)
                uiBoostersList = new List<UiBooster>();
            // Don't add booster if its already running
            // Or the booster Notification is not already added
            if (!UiHolder.StateController.GameManager.CanAddBooster(boosterType) || uiBoostersList.Count(obj => obj.BoosterType == boosterType) >= 1)
                return;

            if (!Management.Services.AdvertisementManager.Instance.IsRewardedAdAvailable())
            {
                if (!Management.Services.AdvertisementManager.Instance.CheckIfCanLoadRewardedAds())
                {
                    return;
                }
            }

            UiBooster booster = Instantiate(uiBoosterPrefab, boosterParentTrans);
            booster.UpdateBooster(boosterType, CbOnBoosterCallbackClicked);

            uiBoostersList.Add(booster);
        }

        #if UNITY_EDITOR
        [ContextMenu("Add booster")]
        public void AddDocotrBooster()
        {
            CbOnAddBooster(Hospital.BoosterType.Doctor_Time_Booster);
        }

        [ContextMenu("Add Cashier booster")]
        public void AddCashierBooster()
        {
            CbOnAddBooster(Hospital.BoosterType.Doctor_Time_Booster);
        }
        #endif

        private void CbOnBoosterCallbackClicked(UiBooster uiBooster)
        {
            PopupTimeBooster popupTimeBoosterPrefab = (PopupTimeBooster)UiHolder.PopupContainer.GetPopupPrefab(PopupType.TIME_BOOSTER);
            popupTimeBoosterInstance = Instantiate(popupTimeBoosterPrefab);
            popupTimeBoosterInstance.Initilize(UiHolder, PopupCallbackTimeBooster);
            popupTimeBoosterInstance.UpdateContent(uiBooster.BoosterType);
        }

        void PopupCallbackTimeBooster(int result, object data)
        {
            Management.Hospital.BoosterType boosterType = (Management.Hospital.BoosterType)data;
            popupTimeBoosterInstance.Close();

            UiBooster uiBooster = uiBoostersList.Find(obj => obj.BoosterType == boosterType);
            uiBoostersList.Remove(uiBooster);
            uiBooster.Remove();

            switch (result)
            {
                case 0:
                    break;
                case 1:
                    UiHolder.ignoreTouchWhenNotificationClicked = true;
                    if (Management.Services.AdvertisementManager.Instance.IsRewardedAdAvailable())
                        Management.Services.AdvertisementManager.Instance.ShowRewardedVideoAds(boosterType, CbOnBoosterAdFinished);
                    else
                        uiHolder.ShowInternetConnectionErrorPopup();
                    StartCoroutine(ResetIgnoreTouch());
                    break;
            }
        }

        void CbOnBoosterAdFinished(bool isFinished, object cutomData)
        {
            if (!isFinished)
                return;
            Management.Hospital.BoosterType boosterType = (Management.Hospital.BoosterType)cutomData;
            uiHolder.StateController.GameManager.ScheduleBooster(boosterType);
            InstantiateBoostTimerUI(boosterType);
        }

        public void CbOnHireAdminClicked()
        {
            uiHolder.ShowUi(UiType.Hire_Admin);
        }

        IEnumerator ResetIgnoreTouch()
        {
            yield return new WaitForEndOfFrame();
            UiHolder.ignoreTouchWhenNotificationClicked = false;
        }

        void InstantiateBoostTimerUI(Management.Hospital.BoosterType boosterType)
        {
            UiBoosterTimer uiBoosterTimer = Instantiate(uiBooterTimerPrefab, boosterTimerParentTrans);
            uiBoosterTimer.UpdateUI(boosterType);
        }

        public void CnOnSettingBtnClicked()
        {
            UiHolder.ShowUi(UiType.Setting);
        }
    }
}
