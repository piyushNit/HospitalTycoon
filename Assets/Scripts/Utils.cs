namespace Management
{
    public static class Utils
    {
        /// <summary>
        /// Get lenght from position path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static float GetLengthFromPathArray(UnityEngine.Vector3[] path)
        {
            float length = 1;
            for (int i = 0; i < path.Length; i++)
            {
                if (i == 0)
                    continue;
                length += UnityEngine.Vector3.Distance(path[i - 1], path[i]);
            }
            return length;
        }

        /// <summary>
        /// Convert time from length
        /// </summary>
        /// <param name="length"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static float ConvertTimeByLength(float length, float time)
        {
            return time * length;
        }

        /// <summary>
        /// Convert time from path array
        /// </summary>
        /// <param name="path"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static float ConvertTimeFromPathArray(UnityEngine.Vector3[] path, float time)
        {
            float length = GetLengthFromPathArray(path);
            return ConvertTimeByLength(length, time);
        }

        /// <summary>
        /// Formats the value to its measures
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FormatWithMeasues(decimal value)
        {
            //Deviding the value by 1 Million measues
            string valueStr = System.Math.Floor(value).ToString("0.#");
            int size = valueStr.Length;
            switch (size)
            {
                case 7://million
                    return valueStr.Substring(0, 1) + "M";
                case 10://Billion
                    return valueStr.Substring(0, 1) + "B";
                case 13://Trillion
                    return valueStr.Substring(0, 1) + "T";
                case 16://Quadrillion
                    return valueStr.Substring(0, 1) + "q";
                case 19://Quintillion
                    return valueStr.Substring(0, 1) + "Q";
                case 21://Sextillion
                    return valueStr.Substring(0, 1) + "s";
                case 24://Septillion
                    return valueStr.Substring(0, 1) + "S";
                case 27://Octillion
                    return valueStr.Substring(0, 1) + "O";
                case 30://Nonillion
                    return valueStr.Substring(0, 1) + "N";
                case 33://Decillion
                    return valueStr.Substring(0, 1) + "d";
                case 36://Undecillion
                    return valueStr.Substring(0, 1) + "U";
                case 39://Dudecillion
                    return valueStr.Substring(0, 1) + "D";
            }
            return value.ToString("0.#");
        }

        public static float GetTimeFromDistance(float time, float distance)
        {
            return distance * time;
        }

        public static string ConvertIntoJsonFormat(string data)
        {
            return "\"{" + data + "}\"";
        }

        public static string ConvertIntoJsonElement(string elementTitle, string elementValue, bool addCommaInEnd = true)
        {
            string data = "\"" + elementTitle + "\": \"" + elementValue + "\"";
            if (addCommaInEnd)
                data += ",";
            return data;
        }

        public static string ConvertToJsonArrayRootElement(string arrayTitle, string body, bool addCommaInEnd = true)
        {
            string data = "\"" + arrayTitle + "\" : [" + body + "]";
            if(addCommaInEnd)
                data += ",";
            return data;
        }

        public static string ConvertIntoJsonArrayElement(string body, bool addCommaInEnd = true)
        {
            string data = "{" + body + "}";
            if (addCommaInEnd)
                data += ",";
            return data;
        }

        public static string ConvertIntoJsonRootElement(string rootElementTitle, string body, bool addCommaInEnd = true)
        {
            string data = "\"" + rootElementTitle + "\" : {" + body + "}";
            if (addCommaInEnd)
                data += ",";
            return data;
        }
    }
}