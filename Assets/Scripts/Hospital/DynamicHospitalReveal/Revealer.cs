using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Hospital.Revealer
{
    public enum Result
    {
        HIDE = 0,
        UNHIDE
    }
    public class Revealer : MonoBehaviour
    {
        [SerializeField] protected string condition;
        public string Condition { get => condition; }
        [SerializeField] protected Result result;
        public Result PResult { get => result; }

        /// <summary>
        /// Get Line Array from the whole condition
        /// </summary>
        /// <param name="isAndConditionCheck"></param>
        /// <returns></returns>
        protected string[] GetLinesFromCondition(string conditionStr, out bool isAndConditionCheck)
        {
            string[] lines;
            if (conditionStr.Contains("&"))
            {
                lines = SplitLines(conditionStr, "&");
                isAndConditionCheck = true;
            }
            else
            {
                if (conditionStr.Contains("|"))
                {
                    lines = SplitLines(conditionStr, "|");
                    isAndConditionCheck = false;
                }
                else
                {
                    lines = new string[] { conditionStr };
                    isAndConditionCheck = false;
                }
            }

            return lines;
        }

        /// <summary>
        /// Split condition str into lines with the conditional operator
        /// </summary>
        /// <param name="conditionStr"></param>
        /// <param name="operatorStr"></param>
        /// <returns></returns>
        string[] SplitLines(string conditionStr, string operatorStr)
        {
            string tempConditionStr = conditionStr;
            List<string> lines = new List<string>();
            while (tempConditionStr.Length != 0)
            {
                int operatorIndex = tempConditionStr.IndexOf(operatorStr);
                if (operatorIndex == -1)
                {
                    lines.Add(tempConditionStr);
                    break;
                }
                string subString = tempConditionStr.Substring(0, operatorIndex);
                if (subString.Contains("("))
                {
                    operatorIndex = tempConditionStr.IndexOf(")");
                    subString = tempConditionStr.Substring(0, operatorIndex + 1);
                    operatorIndex++;
                }
                tempConditionStr = tempConditionStr.Substring(operatorIndex, tempConditionStr.Length - operatorIndex);
                lines.Add(subString);

                if (tempConditionStr.Length > 0 && (tempConditionStr[0].Equals('&') | tempConditionStr[0].Equals('|')))
                    tempConditionStr = tempConditionStr.Substring(1, tempConditionStr.Length - 1);

            }
            return lines.ToArray();
        }

        /// <summary>
        /// Decode the condition string and resturns the decoded condition
        /// </summary>
        /// <param name="conditionStr"></param>
        /// <returns></returns>
        protected string[] DecodeTheCondition(string conditionValue)
        {
            string[] result = new string[2];
            int assignerIndex = conditionValue.IndexOf("=");
            string conditionStr = conditionValue.Substring(0, assignerIndex);
            string resultStr = conditionValue.Substring(assignerIndex + 1, conditionValue.Length - assignerIndex - 1);

            result[0] = conditionStr;
            result[1] = resultStr;

            return result;
        }

    }
}