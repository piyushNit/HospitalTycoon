using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace Management.UI
{
    public enum PopupType
    {
        EXTRA_PATIENT,
        TIME_BOOSTER,

        GENERIC_POPUP
    }

    public class PopUpCore : MonoBehaviour
    {
        [SerializeField] protected PopupType popupType;
        [SerializeField] float animTime = 0.8f;
        protected UiHolder uiHolder;

        protected System.Action<int, object> cbOnResult;

        protected virtual void OnEnable()
        {
            GetComponent<Canvas>().sortingOrder = Config.POPUP_DEPTH_LEVEL;
            Config.POPUP_DEPTH_LEVEL++;
        }

        protected virtual void OnDisable()
        {
            Config.POPUP_DEPTH_LEVEL--;
        }

        /// <summary>
        /// call after instantiate the popup
        /// </summary>
        /// <param name="_uiHolder"></param>
        /// <param name="onResultCallback"> Required to get callback from popup</param>
        public virtual void Initilize(UiHolder _uiHolder, System.Action<int, object> onResultCallback)
        {
            uiHolder = _uiHolder;
            cbOnResult = onResultCallback;
            uiHolder.AddIntoPopupList(this);

            uiHolder.StateController.GameAudioManager.PlaySound(Audio.AudioType.MENU_POP_UP);
        }

        /// <summary>
        /// Ui_Callback
        /// </summary>
        /// <param name="btnIndex"></param>
        public virtual void CbOnBtnClicked(int btnIndex)
        {
            if (cbOnResult != null)
                cbOnResult(btnIndex, null);
        }

        public virtual void Close()
        {
            StartCoroutine(ClosePopup());
        }

        protected virtual IEnumerator ClosePopup()
        {
            uiHolder.StateController.GameAudioManager.PlaySound(Audio.AudioType.MENU_POP_UP);
            yield return new WaitForEndOfFrame();
            uiHolder.RemoveFromPopupList(this);
            Destroy(gameObject);
        }

        /// <summary>
        /// Plays the opening animation
        /// </summary>
        /// <param name="baseTrans">If null then plays on parent Tranform</param>
        /// <param name="cbAnimFinished"></param>
        protected virtual void PlayOpenAnimation(Transform baseTrans = null, System.Action cbAnimFinished = null)
        {
            if (baseTrans == null)
                baseTrans = transform;
            baseTrans.localScale = Vector3.one * 0.8f;
            baseTrans.DOScale(1, animTime).OnComplete(() =>
            {
                if (cbAnimFinished != null)
                    cbAnimFinished();
            });
        }

        /// <summary>
        /// Plays popup closing animation
        /// </summary>
        /// <param name="baseTrans">If null then plays on parent Tranform</param>
        /// <param name="cbAnimFinished"></param>
        protected virtual void PlayCloseAnimation(Transform baseTrans = null, System.Action cbAnimFinished = null)
        {
            if (baseTrans == null)
                baseTrans = transform;
            baseTrans.DOScale(0.8f, animTime).OnComplete(() =>
            {
                if (cbAnimFinished != null)
                    cbAnimFinished();
            });
        }
    }
}