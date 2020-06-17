using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.UI
{
    // Warning : Always add new element at the end
    public enum UiType
    {
        NONE = 0,
        HUD,
        Hospital_Department,
        Player_Score_Header,
        Parking,
        Admin_Department,
        Utilities,
        Ad_Campaign,
        Welcome_Back_Screen,
        Hire_Admin,
        Setting,
        Waiting_Lobby,
        FTUE
    }

    public class UiCoreHolder : MonoBehaviour
    {
        protected UiHolder uiHolder;
        public UiHolder UiHolder { get => uiHolder; }
        [SerializeField] protected UiType uiType;
        public UiType UiType { get => uiType; }

        public void SetReferenceOfParent(UiHolder _uiHolder)
        {
            uiHolder = _uiHolder;
        }

        /// <summary>
        /// Enabling or disabling the UI game object
        /// </summary>
        /// <param name="active"></param>
        public virtual void ToggleUI(bool active)
        {
            gameObject.SetActive(active);
        }

        public virtual void OnEnable()
        {
        }
        public virtual void OnDisable()
        {
        }

        #region UI_BUTTON_CALLBACK
        public virtual void CbOnBackBtnClicked()
        {
            StartCoroutine(CloseUi());
        }
        #endregion

        protected virtual IEnumerator CloseUi()
        {
            yield return new WaitForEndOfFrame();

            uiHolder.CloseAllAndOpenHUD();
        }
    }
}
