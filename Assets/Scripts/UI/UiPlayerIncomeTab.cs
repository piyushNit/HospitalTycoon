using UnityEngine;
using TMPro;

namespace Management.UI
{
    public class UiPlayerIncomeTab : UiCoreHolder
    {
        [SerializeField] TextMeshProUGUI txtPerMinIncome;
        [SerializeField] TextMeshProUGUI txtPlayerIncome;
        [SerializeField] TextMeshProUGUI txtDiamond;

        /// <summary>
        /// Updates the player score to UI
        /// </summary>
        /// <param name="playerScoreModel"></param>
        public void UpdatePlayerScore(Management.SaveLoad.PlayerScoreModel playerScoreModel)
        {
            txtPlayerIncome.text = Utils.FormatWithMeasues(playerScoreModel.TotalIncome);
            txtDiamond.text = playerScoreModel.DiamondCount.ToString();
        }

        public void UpdateIncomePerMin()
        {
            double incomePerMin = 0;
            Hospital.HospitalBuilding refHospitalBuilding = uiHolder.StateController.HospitalManager.HospitalBuilding;
            for (int i = 0; i < refHospitalBuilding.Departments.Count; ++i)
            {
                if (!refHospitalBuilding.Departments[i].IsUnlocked)
                    continue;
                incomePerMin += refHospitalBuilding.Departments[i].DeaprtmentUpgradeJson.base_income 
                                * refHospitalBuilding.Departments[i].UpgradeContent.income_multiplyer;
            }

            //incomePerMin *= 60 / refHospitalBuilding.PharmacyDepartments.GetWorkTime();

            txtPerMinIncome.text = incomePerMin.ToString() + "/Min";
        }
    }
}