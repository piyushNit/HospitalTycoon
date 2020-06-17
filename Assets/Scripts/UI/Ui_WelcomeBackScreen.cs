using UnityEngine;
using UnityEngine.UI;

namespace Management.UI
{
    public class Ui_WelcomeBackScreen : UiCoreHolder
    {
        [SerializeField] Slider sliderHour;
        [SerializeField] Button btnGetAdmin;

        #region UI_Callbacks

        public void CbOn2XClicked()
        {
            Debug.Log("2x Clicked");
            if (Management.Services.AdvertisementManager.Instance.IsRewardedAdAvailable())
                Management.Services.AdvertisementManager.Instance.ShowRewardedVideoAds(cbCallbackFunc: AdCallbackDoubleCoins);
            else
                Debug.Log("Ads not loaded");
        }

        void AdCallbackDoubleCoins(bool success, object customData)
        {
            if (!success)
                return;
            UiHolder.StateController.GameManager.MultiplyReloadedPlayerIncome(2);
            CbOnBackBtnClicked();
        }

        public void CbOn3XClicked()
        {
            UiHolder.StateController.GameManager.MultiplyReloadedPlayerIncome(3);
            CbOnBackBtnClicked();
        }

        public void CbOnGetAdmingClicked()
        {
            uiHolder.ShowHireAdmingUi();
        }
        #endregion

    }
}