using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Management.UI
{
    public class UiDepartmentInfoPanelDataHolder : MonoBehaviour
    {
        [SerializeField] Image imgIcon;
        [SerializeField] TextMeshProUGUI txtUpgradeLevelIndex;
        [SerializeField] TextMeshProUGUI txtIncomeMultiplyer;

        public void UpdateData(Sprite icon, int index, int incomeMultiplyer)
        {
            imgIcon.sprite = icon;
            txtUpgradeLevelIndex.text = index.ToString();
            txtIncomeMultiplyer.text = "X" + incomeMultiplyer.ToString();
        }
    }
}