using System.Collections.Generic;
using UnityEngine;

namespace Management.Hospital.Revealer
{
    public class ConditionalRevealer : Revealer
    {
/*        public enum Result
        {
            HIDE = 0,
            UNHIDE
        }
*/
        /*[Tooltip("1. EYE_SPECILIST=true"
            + "2.EYE_SPECILIST = true & EYE_SPECILIST = true & EYE_SPECILIST = true"
            + "3.EYE_SPECILIST = true | EYE_SPECILIST = true | EYE_SPECILIST = true"
            + "4.EYE_SPECILIST = true & (EYE_SPECILIST = true | EYE_SPECILIST = true)"
            + "5.EYE_SPECILIST = true & (EYE_SPECILIST = true & EYE_SPECILIST = true) & (EYE_SPECILIST = true & EYE_SPECILIST = false)")]*/
        //[SerializeField] string condition;
        //public string Condition { get => condition; }
        //[SerializeField] Result result;
        //public Result PResult { get => result; }

        /// <summary>
        /// Analyzes and update the object based on the condition and result
        /// </summary>
        /// <param name="departments"></param>
        public void AnalyzeConditionAndUpdateResult(List<Department> departments)
        {
            /*int assignerIndex = condition.IndexOf("=");
            string conditionStr = Condition.Substring(0, assignerIndex);
            string resultStr = Condition.Substring(assignerIndex + 1, Condition.Length - assignerIndex - 1);*/
            string[] decodedCondition = DecodeTheCondition(Condition);
            bool isTrue = decodedCondition[1].ToUpper() == "TRUE";
            Management.Hospital.Core.DepartmentType departmentType = (Management.Hospital.Core.DepartmentType)System.Enum.Parse(typeof(Management.Hospital.Core.DepartmentType), decodedCondition[0]);
            Department department = departments.Find(obj => obj.DepartmentType == departmentType);
            if (department.IsUnlocked == isTrue)
            {
                gameObject.SetActive(result == Result.UNHIDE);
            }
        }

        /// <summary>
        /// Splits into lines and analyze the conditions and then reutrn the final result
        /// </summary>
        /// <param name="conditionStr"></param>
        /// <returns></returns>
        bool AnalyzeMutiLineConditions(string conditionStr, List<Department> departments)
        {
            bool finalBoolValue = false;
            List<bool> values = new List<bool>();

            bool isAndConditionCheck = false;
            string[] lines = GetLinesFromCondition(conditionStr, out isAndConditionCheck);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("("))
                {
                    string subConditionStr = lines[i];
                    subConditionStr = subConditionStr.Replace("(", "");
                    subConditionStr = subConditionStr.Replace(")", "");
                    values.Add(AnalyzeMutiLineConditions(subConditionStr, departments));
                }
                else
                {
                    values.Add(AnalyzeSingleLineConditionResult(lines[i], departments));
                }
            }


            if (lines.Length != 0)
                finalBoolValue = isAndConditionCheck ? values.TrueForAll((val) => val == true) : values.Contains(true);
            else
                finalBoolValue = false;

            return finalBoolValue;
        }

        /// <summary>
        /// Analyze the condition and returns the result
        /// </summary>
        /// <param name="conditionLineStr"></param>
        /// <returns></returns>
        bool AnalyzeSingleLineConditionResult(string conditionLineStr, List<Department> departments)
        {
            string[] decodedCondition = DecodeTheCondition(Condition);
            bool isTrue = decodedCondition[1].ToUpper() == "TRUE";

            Management.Hospital.Core.DepartmentType departmentType = (Management.Hospital.Core.DepartmentType)System.Enum.Parse(typeof(Management.Hospital.Core.DepartmentType), decodedCondition[0]);
            Department department = departments.Find(obj => obj.DepartmentType == departmentType);
            return department.IsUnlocked == isTrue;
        }
    }
}