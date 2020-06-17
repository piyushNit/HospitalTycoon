using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Management.GameDebug
{
    public class UiDebugPrinter : MonoBehaviour
    {
        [SerializeField] Arch.Core.StateController refStateController;
        [SerializeField] GameObject scrollRect;
        [SerializeField] GameObject hud;
        [SerializeField] GameObject saveLoadPanel;
        [SerializeField] GameObject panelPlayerIncome;

        [SerializeField] TMP_InputField inputField;
        [SerializeField] TMP_InputField gemsInputField;
        [SerializeField] UnityEngine.UI.Toggle toggle;

        [SerializeField] TextMeshProUGUI txtMeshPro;
        [SerializeField] TextMeshProUGUI txtUserName;
        [SerializeField] TextMeshProUGUI txtSaveStatus;
        [SerializeField] TextMeshProUGUI txtDeleteStatus;

        [SerializeField] TextMeshProUGUI txtFps;

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLogs;
            if (Management.Services.GooglePlayServices.Instance != null && Management.Services.GooglePlayServices.Instance.IsUserAuthenticated())
            {
                txtUserName.text = "Username: " + Social.localUser.userName;
            }
            else
            {
                txtUserName.text = "Guest User";
            }

            toggle.isOn = Management.GameManager.IS_INFINITE_INCOME;
            if(fpsUpdateCoroutine == null)
                StartCoroutine(UpdateFps());
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLogs;
        }

        public bool IsDebugActive()
        {
            return saveLoadPanel.activeSelf == true 
                    || panelPlayerIncome.activeSelf == true
                    || scrollRect.activeSelf == true;
        }

        #region LOGS
        void HandleLogs(string logString, string stackTrace, LogType type)
        {
            txtMeshPro.text += "//n" + logString;
        }

        public void CbOnClearLogs()
        {
            txtMeshPro.text = "";
        }

        public void CbOnShowLogs()
        {
            scrollRect.SetActive(true);
            hud.gameObject.SetActive(false);
        }
        #endregion

        #region SAVE_LOAD
        public void CbOnShowSave()
        {
            saveLoadPanel.SetActive(true);
            hud.gameObject.SetActive(false);
        }

        public void CbOnSaveClicked()
        {
            refStateController.GameManager.SaveGameData();
            txtSaveStatus.text = "Save Attempt: " + System.DateTime.Now.ToString();
        }

        public void CbOnDeleteClicked()
        {
            Management.Services.GooglePlayServices.Instance.DeleteSaveFile();
            txtDeleteStatus.text = "Delete Success: " + System.DateTime.Now.ToString();
        }

        #endregion

        #region PLAYER_SCORE
        public void CbOnShowPlayerCurrency()
        {
            panelPlayerIncome.SetActive(true);
            hud.gameObject.SetActive(false);
        }

        public void CbOnTotalAdd()
        {
            decimal valueToAdd = decimal.Parse(inputField.text);
            refStateController.GameManager.OnPaymentComplate(valueToAdd);
            refStateController.UiHolder.ReloadPlayerScoreInHeaderUI();
        }

        public void CbOnDeductMoney()
        {
            decimal valueToAdd = decimal.Parse(inputField.text);
            refStateController.GameManager.MasterLoader.PlayerScoreModel.DeductCash(valueToAdd);
            refStateController.UiHolder.ReloadPlayerScoreInHeaderUI();
        }

        public void CbOnAddGemsClicked()
        {
            int valueToAdd = int.Parse(gemsInputField.text);
            refStateController.GameManager.MasterLoader.PlayerScoreModel.AddDiamonds(valueToAdd);
            refStateController.UiHolder.ReloadPlayerScoreInHeaderUI();
        }

        public void CbOnDeductGemsClicked()
        {
            int valueToAdd = int.Parse(gemsInputField.text);
            refStateController.GameManager.MasterLoader.PlayerScoreModel.DeductDiamond(valueToAdd);
            refStateController.UiHolder.ReloadPlayerScoreInHeaderUI();
        }

        public void CbOnToggleUpdate(bool isActive)
        {
            Management.GameManager.IS_INFINITE_INCOME = toggle.isOn;
        }

        public void CbOnApplicationQuit()
        {
            Application.Quit();
        }
        #endregion

        public void CbOnCloseClicked()
        {
            scrollRect.SetActive(false);
            saveLoadPanel.SetActive(false);
            panelPlayerIncome.SetActive(false);

            hud.gameObject.SetActive(true);
        }

        Coroutine fpsUpdateCoroutine = null;

        public void cbOnShowFps()
        {
            txtFps.gameObject.SetActive(!txtFps.gameObject.activeSelf);
            if (txtFps.gameObject.activeSelf && fpsUpdateCoroutine == null)
            {
                fpsUpdateCoroutine = StartCoroutine(UpdateFps());
            }
            else
            {
                if (fpsUpdateCoroutine != null)
                    StopCoroutine(UpdateFps());
            }
        }

        IEnumerator UpdateFps()
        {
            float deltaTime = 0;
            while (gameObject != null)
            {
                deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
                float fps = Mathf.Floor(1.0f / deltaTime);
                txtFps.text = "FPS: " + fps.ToString();
                yield return null;
            }
        }
    }
}