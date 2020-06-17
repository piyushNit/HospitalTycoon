using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Management.UI
{
    public class UI_AdCampaign : UiCoreHolder
    {
        [SerializeField] Slider sliderAdCampaign;
        [SerializeField] TextMeshProUGUI txtIncomeVal;
        [SerializeField] int diamondOfferForAdCampaign = 60;

        public override void OnEnable()
        {
            base.OnEnable();
            UpdateAdWatchSlider();
            if (!Management.Services.AdvertisementManager.Instance.IsRewardedAdAvailable() && !Management.Services.AdvertisementManager.Instance.CheckIfCanLoadRewardedAds())
                UiHolder.ShowInternetConnectionErrorPopup();
        }

        #region UI_Reference
        public void CbOnWatchVideo()
        {
            if (Management.Services.AdvertisementManager.Instance.IsRewardedAdAvailable())
                Management.Services.AdvertisementManager.Instance.ShowRewardedVideoAds(cbCallbackFunc: AdCallbackFinished);
            else
                UiHolder.ShowInternetConnectionErrorPopup();
        }

        void AdCallbackFinished(bool success, object customData)
        {
            if (!success)
                return;
            UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IncreaseAdWatchCount();
            UpdateAdWatchSlider();

            float value = (float)UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.adCampaignAdsWatchCount / 8;
            if (value >= 1)
            {
                UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.AddDiamonds(diamondOfferForAdCampaign);
                UiHolder.PlayerIncomeTab.UpdatePlayerScore(UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel);
                UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.ResetAdWatchCount();
            }
        }

        void UpdateAdWatchSlider()
        {
            float value = (float)UiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.adCampaignAdsWatchCount / 8;
            if(sliderAdCampaign.value == 1)
                sliderAdCampaign.value = value;
            else
                sliderAdCampaign.DOValue(value, 1);
        }
        #endregion
    }
}