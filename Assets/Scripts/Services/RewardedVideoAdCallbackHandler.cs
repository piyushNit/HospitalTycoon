using UnityEngine;

namespace Management.Services
{
    /// <summary>
    /// This extra class is for solving the Admob crash after wating reward ad
    /// 
    /// Reason for crash: Admob is pausing unity main thread in order to show ads,
    /// And unity is receiving the callback not in main thread, thats the reason game is crashing in enabling any game object in in main thread
    /// </summary>
    public class RewardedVideoAdCallbackHandler : MonoBehaviour
    {
        public static bool isRewardedCallbackReceived = false;

        public static System.Action<bool, object> CallbackOnAdFinsished = null;
        public static object customData = null;

        private void Update()
        {
            if (!isRewardedCallbackReceived)
                return;

            if (CallbackOnAdFinsished != null)
                CallbackOnAdFinsished(true, customData);

            Destroy(GetComponent<RewardedVideoAdCallbackHandler>());
        }
    }
}
