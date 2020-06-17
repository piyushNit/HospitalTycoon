using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Management.UI
{
    public class PopupTimeBooster : PopUpCore
    {
        [SerializeField] TextMeshProUGUI txtTitle;
        [SerializeField] TextMeshProUGUI txtMessage;
        [SerializeField] Image imgIcon;
        [SerializeField] TextMeshProUGUI txtSpeedUp;
        [SerializeField] Management.Hospital.Scriptable.PopupBoosterContainer popupBoosterContainer;

        private Management.Hospital.BoosterType boosterType;

        public void UpdateContent(Management.Hospital.BoosterType _boosterType)
        {
            boosterType = _boosterType;
            Management.Hospital.Scriptable.PopupBoosterContainer.BoosterPopupContent content = popupBoosterContainer.GetPopupContent(boosterType);

            txtTitle.text = content.title;
            txtMessage.text = content.message;
            imgIcon.sprite = content.icon;
            imgIcon.rectTransform.rect.Set(imgIcon.rectTransform.rect.x, imgIcon.rectTransform.rect.y, content.icon.rect.width, content.icon.rect.height);

            txtSpeedUp.text = content.speedUpMessage;
        }

        public override void CbOnBtnClicked(int btnIndex)
        {
            if (cbOnResult != null)
                cbOnResult(btnIndex, boosterType);
        }
    }
}