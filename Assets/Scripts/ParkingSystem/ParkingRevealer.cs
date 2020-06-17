using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Hospital.Revealer
{
    public class ParkingRevealer : Revealer
    {
        public void Reveal(Management.Parking.ParkingRevealIndex parkingRevealIndex)
        {
            bool isAndConditionCheck = false;
            List<bool> values = new List<bool>();

            string[] conditionString = GetLinesFromCondition(condition, out isAndConditionCheck);

            for (int i = 0; i < conditionString.Length; ++i)
            {
                values.Add(AnalyzeSingleLineCondition(conditionString[i], parkingRevealIndex));
            }

            bool isConditionMeet = isAndConditionCheck ? values.TrueForAll((val) => val == true) : values.Contains(true);
            if (isConditionMeet)
            {
                gameObject.SetActive(result == Result.UNHIDE);
            }
        }

        bool AnalyzeSingleLineCondition(string conditionStr, Management.Parking.ParkingRevealIndex parkingRevealIndex)
        {
            string[] decodedCondition = DecodeTheCondition(conditionStr);
            bool isTrue = decodedCondition[1].ToUpper() == "TRUE";
            Parking.ParkingRevealIndex checkParkingRevealIndex = (Parking.ParkingRevealIndex)System.Enum.Parse(typeof(Parking.ParkingRevealIndex), decodedCondition[0]);

            if (checkParkingRevealIndex == parkingRevealIndex)
                return isTrue;
            return false;
        }
    }
}