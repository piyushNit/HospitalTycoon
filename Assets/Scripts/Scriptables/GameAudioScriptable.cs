using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Audio
{
    [CreateAssetMenu(menuName = "Audio/Game Audio Asset")]
    public class GameAudioScriptable : ScriptableObject
    {
        [System.Serializable]
        public class AudioData
        {
            public AudioType audioType;
            public AudioClip audioClip;
        }

        [SerializeField] List<AudioData> audioDataList;

        public AudioClip GetAudioClip(AudioType type)
        {
            return audioDataList.Find(obj => obj.audioType == type).audioClip;
        }
    }
}