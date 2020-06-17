using UnityEngine;
using UnityEngine.UI;

namespace Management.UI
{
    public class UiDepartmentInfoTab : MonoBehaviour
    {
        [SerializeField] UiDepartmentInfoPanelDataHolder dataHolderPrefab;
        [SerializeField] Scrollbar scrollbar;
        [SerializeField] Transform container;

        public void ShowInfoTab(Management.Hospital.Scriptable.DepartmentUpgradeIconDataScriptable iconScriptable, Management.Hospital.BaseDepartment baseDepartment)
        {
            gameObject.SetActive(true);
            scrollbar.value = 0;
            for (int i = 0; i < baseDepartment.DeaprtmentUpgradeJson.upgradable_contents.Count; ++i)
            {
                UiDepartmentInfoPanelDataHolder dataHolderInstance = Instantiate(dataHolderPrefab, container);
                dataHolderInstance.UpdateData(iconScriptable.Icons[i], i + 1, baseDepartment.DeaprtmentUpgradeJson.upgradable_contents[i].income_multiplyer);
            }
        }

        public void CbOnCloseClicked()
        {
            ClearAll();
            gameObject.SetActive(false);
        }

        void ClearAll()
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
        }
    }
}