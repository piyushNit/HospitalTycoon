using System.Collections.Generic;
using UnityEngine;

namespace Management.Hospital.Scriptable
{
    [CreateAssetMenu(menuName = "UI/Popup Booster Container")]
    public class PopupBoosterContainer : ScriptableObject
    {
        [System.Serializable]
        public class BoosterPopupContent
        {
            public string name;
            public Management.Hospital.BoosterType boosterType;
            public string title;
            public string message;
            public Sprite icon;
            public string speedUpMessage;
        }

        [SerializeField] List<BoosterPopupContent> boosterPopupContentList;
        public BoosterPopupContent GetPopupContent(Management.Hospital.BoosterType boosterType)
        {
            return boosterPopupContentList.Find(obj => obj.boosterType == boosterType);
        }
    }
}