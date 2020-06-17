using UnityEngine;

namespace Arch.Json
{
    public static class JsonReader
    {
        /// <summary>
        /// Generic method to deserilize json string to <T> class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static ClassName LoadJson<ClassName>(string jsonString)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ClassName>(jsonString);
            }
            catch
            {
                return default;
            }
        }
    }
}