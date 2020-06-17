using System.Collections;
using System.Collections.Generic;
using Arch.Core;
using UnityEngine;

namespace Management.Audio
{
    public enum AudioType
    {
        BACKGROUND_MUSIC,
        UPGRADE_BTN_CLICKED,
        CASH_IN,
        MENU_POP_UP
    }

    public class GameAudioManager : MonoBehaviour, Arch.Core.iManagerBase
    {
        [SerializeField] AudioSource backgroundMusic;
        [SerializeField] GameAudioScriptable gameAudioScriptable;
        [SerializeField] float menuOpenVolumn = 1;

        bool playSound = true;

        public void Initilize(StateController _stateController)
        {
            UpdateMusic(PlayerPrefs.GetInt(Config.SETTING_MUSIC, 1) == 1);
        }

        void UpdateMusic(bool isRunning)
        {
            if (isRunning && !backgroundMusic.isPlaying)
                backgroundMusic.Play();
            else
                if(!isRunning && backgroundMusic.isPlaying)
                    backgroundMusic.Stop();
        }

        public void LoadGameData()
        {
            
        }

        public void OnStateChange(StateController _stateController)
        {
            
        }

        public void SaveGameData()
        {
            
        }

        public void OnGameFocused(bool hasFocus)
        {

        }

        private void OnEnable()
        {
            Management.UI.Ui_Setting.OnSettingUpdated += OnPlayerSettingChanged;
        }

        private void OnDisable()
        {
            Management.UI.Ui_Setting.OnSettingUpdated -= OnPlayerSettingChanged;
        }

        public void PlaySound(AudioType audioType)
        {
            if (!playSound)
                return;
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = gameAudioScriptable.GetAudioClip(audioType);

            switch (audioType)
            {
                case AudioType.MENU_POP_UP:
                    audioSource.volume = menuOpenVolumn;
                    break;
            }

            audioSource.Play();
            Destroy(audioSource, audioSource.clip.length + 1);
        }

        void OnPlayerSettingChanged(Dictionary<string, int> settingDict)
        {
            if (settingDict.ContainsKey(Config.SETTING_MUSIC))
                UpdateMusic(settingDict[Config.SETTING_MUSIC] == 1);

            if (settingDict.ContainsKey(Config.SETTING_SOUND))
                playSound = settingDict[Config.SETTING_SOUND] == 1;
        }
    }
}