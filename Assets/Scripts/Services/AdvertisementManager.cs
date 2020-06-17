using UnityEngine;
using GoogleMobileAds.Api;
using System.Collections;

namespace Management.Services
{
    public class AdvertisementManager : MonoBehaviour
    {
        private static AdvertisementManager instance;
        public static AdvertisementManager Instance { get => instance; }

        const string AD_MOB_APP_ID = "ca-app-pub-1177908863371033~8330278671";

        private RewardedAd rewardedVideoAd;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            MobileAds.Initialize(initStatus => { });
            AddTestDevices();
            LoadRewardedVideoAds();
        }

        public bool CheckIfCanLoadRewardedAds()
        {
            if (GameManager.CheckForInternetConnection())
            {
                LoadRewardedVideoAds();
                return IsRewardedAdAvailable();
            }
            return false;
        }

        /// <summary>
        /// Loads rewarded video ads
        /// </summary>
        public void LoadRewardedVideoAds()
        {
            rewardedVideoAd = CreateAndLoadRewardedAd();
        }

        void AddTestDevices()
        {
            System.Collections.Generic.List<string> deviceIds = new System.Collections.Generic.List<string>();
            deviceIds.Add("0925583f0805");//MY MI 6 PRO
            deviceIds.Add("44496397d25");//MY MI WHITE
            RequestConfiguration requestConfigurationBuilder = new RequestConfiguration
                .Builder()
                .SetTestDeviceIds(deviceIds)
                .build();
            MobileAds.SetRequestConfiguration(requestConfigurationBuilder);
        }

        RewardedAd CreateAndLoadRewardedAd()
        {
            #if TEST_GAME
            const string REWARDED_AD_UNIT = "ca-app-pub-3940256099942544/5224354917";//TEST ADS
            #else
            const string REWARDED_AD_UNIT = "ca-app-pub-1177908863371033/3844397859";
            #endif

            RewardedAd rewardedAd = new RewardedAd(REWARDED_AD_UNIT);

            rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
            rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
            rewardedAd.OnAdClosed += HandleRewardedAdClosed;

            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the rewarded ad with the request.
            rewardedAd.LoadAd(request);
            return rewardedAd;
        }

        public void HandleRewardedAdLoaded(object sender, System.EventArgs args)
        {
            MonoBehaviour.print("HandleRewardedAdLoaded event received");
        }

        public void HandleRewardedAdClosed(object sender, System.EventArgs args)
        {
            MonoBehaviour.print("HandleRewardedAdClosed event received");
            LoadRewardedVideoAds();
        }

        public void HandleUserEarnedReward(object sender, Reward args)
        {
            string type = args.Type;
            double amount = args.Amount;
            MonoBehaviour.print(
                "HandleRewardedAdRewarded event received for "
                            + amount.ToString() + " " + type);
            RewardedVideoAdCallbackHandler.isRewardedCallbackReceived = true;
        }

        #region REWARDED_VIDEO_AD

        /// <summary>
        /// Checks if rewarded video ads available
        /// </summary>
        /// <returns></returns>
        public bool IsRewardedAdAvailable()
        {
            return rewardedVideoAd != null && rewardedVideoAd.IsLoaded();
        }

        public void ShowRewardedVideoAds(object customdata = null, System.Action<bool, object> cbCallbackFunc = null)
        {
            gameObject.AddComponent<RewardedVideoAdCallbackHandler>();

            RewardedVideoAdCallbackHandler.customData = customdata;
            RewardedVideoAdCallbackHandler.CallbackOnAdFinsished = cbCallbackFunc;
            if (IsRewardedAdAvailable())
            {
                RewardedVideoAdCallbackHandler.isRewardedCallbackReceived = false;
                rewardedVideoAd.Show();
            }
            #if UNITY_EDITOR
            RewardedVideoAdCallbackHandler.isRewardedCallbackReceived = true;
            #endif
        }
        #endregion
    }
}
