using System.Collections.Generic;
using UnityEngine;

namespace Management.UI
{
    [CreateAssetMenu(menuName = "UI/PopupContainer")]
    public class PopupContainer : ScriptableObject
    {
        [System.Serializable]
        public class PopupPrefabHolder
        {
            public PopupType popupType;
            [SerializeField] PopUpCore popupPrefab;
            public PopUpCore PopupPrefab { get => popupPrefab; }
        }

        [SerializeField] List<PopupPrefabHolder> popupList;

        /// <summary>
        /// Returns the original prefab
        /// </summary>
        /// <param name="_popupType"></param>
        /// <returns></returns>
        public PopUpCore GetPopupPrefab(PopupType _popupType)
        {
            PopupPrefabHolder popupHolder = popupList.Find(obj => obj.popupType == _popupType);
            return popupHolder.PopupPrefab;
        }

        /// <summary>
        /// Instantiates the popup for you and returns the instance
        /// </summary>
        /// <param name="_popupType"></param>
        /// <param name="_uiHolderTransform"></param>
        /// <returns></returns>
        public PopUpCore GetPopupInstance(PopupType _popupType, Transform _uiHolderTransform)
        {
            PopUpCore popupPrefab = GetPopupPrefab(_popupType);
            if (popupPrefab != null)
            {
                PopUpCore instance = Instantiate(popupPrefab, _uiHolderTransform) as PopUpCore;
                return instance;
            }
            return null;
        }
    }
}