using UnityEngine;
using TMPro;
using System.Collections;

namespace Management.UI
{
    public class PopupGeneric : PopUpCore
    {
        #region UI_Reference
        [SerializeField] Transform contentParent;
        [SerializeField] TextMeshProUGUI txtHeader;
        [SerializeField] TextMeshProUGUI txtBody;
        [SerializeField] TextMeshProUGUI txtBtn1;
        [SerializeField] TextMeshProUGUI txtBtn2;
        [SerializeField] UnityEngine.UI.Button btn1;
        [SerializeField] UnityEngine.UI.Button btn2;
        #endregion

        /// <summary>
        /// Shows Popup with provided values
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="btn1Text"></param>
        /// <param name="btn2Text"></param>
        public void UpdateContent(string title, string message, string btn1Text, string btn2Text = "")
        {
            txtHeader.text = title;
            txtBody.text = message;
            txtBtn1.text = btn1Text;
            txtBtn2.text = btn2Text;

            if (System.String.IsNullOrEmpty(btn2Text))
            {
                RectTransform btnNoRect = btn1.GetComponent<RectTransform>();
                btnNoRect.anchoredPosition = new Vector2(0, btnNoRect.anchoredPosition.y);

                btn2.gameObject.SetActive(false);
            }

            PlayOpenAnimation(contentParent);
        }

        #region UI_Callbacks
        public override void CbOnBtnClicked(int btnIndex)
        {
            if (cbOnResult != null)
                cbOnResult(btnIndex, null);
        }
        #endregion
    }
}