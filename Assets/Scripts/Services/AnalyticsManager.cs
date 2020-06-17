using UnityEngine.Analytics;

namespace Management.Services
{
    public class AnalyticsManager
    {
        private static AnalyticsManager instance;
        public static AnalyticsManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new AnalyticsManager();
                return instance;
            }
        }

        /// <summary>
        /// Collect game events
        /// </summary>
        /// <param name="eventStr"></param>
        /// <param name="data"></param>
        public void CustomEvent(string eventStr, System.Collections.Generic.Dictionary<string, object> data = null)
        {
            #if UNITY_EDITOR
            UnityEngine.Debug.Log("##Analytics: " + eventStr.ToString());
            #endif
            Analytics.CustomEvent(eventStr, data);
        }
    }
}