using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Management.UI
{
    public class Ui_Setting : UiCoreHolder
    {
        [SerializeField] Sprite toggleOff;
        [SerializeField] Sprite tooggleOn;
        [SerializeField] Button btnMusic;
        [SerializeField] Button btnSound;
        [SerializeField] Button btnGoogleSave;
        [SerializeField] TMPro.TextMeshProUGUI txtStatus;

        public TMPro.TextMeshProUGUI txtGameScore;
        public TMPro.TextMeshProUGUI txtReadStatus;
        public TMPro.TextMeshProUGUI txtSavedScore;
        public TMPro.TextMeshProUGUI txtWriteStatus;

        public static System.Action<Dictionary<string, int>> OnSettingUpdated;

        public override void OnEnable()
        {
            base.OnEnable();
            btnMusic.GetComponent<Image>().sprite = PlayerPrefs.GetInt(Config.SETTING_MUSIC, 1) == 1 ? tooggleOn : toggleOff;
            btnSound.GetComponent<Image>().sprite = PlayerPrefs.GetInt(Config.SETTING_SOUND, 1) == 1 ? tooggleOn : toggleOff;
            GoogleSignInCallback(Management.Services.GooglePlayServices.Instance == null
                                    ? false
                                    : Management.Services.GooglePlayServices.Instance.IsUserAuthenticated());
        }

        #region UI_Callbacks
        public void CbOnMusicClicked()
        {
            int value = PlayerPrefs.GetInt(Config.SETTING_MUSIC, 1);
            value = value == 1 ? 0 : 1;
            PlayerPrefs.SetInt(Config.SETTING_MUSIC, value);

            btnMusic.GetComponent<Image>().sprite = value == 1 ? tooggleOn : toggleOff;

            Dictionary<string, int> settingData = new Dictionary<string, int>();
            settingData[Config.SETTING_MUSIC] = value;
            if (OnSettingUpdated != null)
                OnSettingUpdated(settingData);
        }

        public void CbOnSoundClicked()
        {
            int value = PlayerPrefs.GetInt(Config.SETTING_SOUND, 1);
            value = value == 1 ? 0 : 1;
            PlayerPrefs.SetInt(Config.SETTING_SOUND, value);

            btnSound.GetComponent<Image>().sprite = PlayerPrefs.GetInt(Config.SETTING_SOUND, 1) == 1 ? tooggleOn : toggleOff;

            Dictionary<string, int> settingData = new Dictionary<string, int>();
            settingData[Config.SETTING_SOUND] = value;
            if (OnSettingUpdated != null)
                OnSettingUpdated(settingData);
        }

        public void CbOnGooglePlayClicked()
        {
            if (!Management.Services.GooglePlayServices.Instance.IsUserAuthenticated())
            {
                Management.Services.GooglePlayServices.Instance.SignIn(GoogleSignInCallback);
            }
            else
            {
                Management.Services.GooglePlayServices.Instance.SignOut();
                PlayerPrefs.DeleteKey(Management.SaveLoad.KEYS.KEY_USER_SILENT_LOGIN);
                GoogleSignInCallback(false);
            }
        }

        private void GoogleSignInCallback(bool result)
        {
            btnGoogleSave.GetComponent<Image>().sprite = result ? tooggleOn : toggleOff;

            if (result)
            {
                txtStatus.text = "Signed in as: " + Social.localUser.userName;
            }
            else
            {
                txtStatus.text = "SIGN INTO GOOGLEPLAY TO SAVE PROGRESS.";
            }
        }

        public void CbOnSupportClicked()
        {
        }
        #endregion
    }
}