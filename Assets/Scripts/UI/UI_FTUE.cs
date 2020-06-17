using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Management.UI
{
    public class UI_FTUE : UiCoreHolder
    {
        #region UI_REFERENCES
        [SerializeField] Image imgSpeechBubble;
        [SerializeField] Image imgDoctor;
        [SerializeField] Image imgHand;
        [SerializeField] TextMeshProUGUI txtSpeechBubbleText;
        [SerializeField] Button btnTapToSkip;
        #endregion

        public void ShowSpeechBubble(string message)
        {
            if (!imgSpeechBubble.gameObject.activeSelf)
                imgSpeechBubble.gameObject.SetActive(true);
            txtSpeechBubbleText.text = message;
        }

        public void DisableSpeechBubble()
        {
            imgSpeechBubble.gameObject.SetActive(false);
        }

        public void UpdateDoctorSpr(Sprite sprDoctor)
        {
            if (!imgDoctor.gameObject.activeSelf)
                imgDoctor.gameObject.SetActive(true);
            imgDoctor.sprite = sprDoctor;
        }

        public void DisableDoctorSpr()
        {
            imgDoctor.gameObject.SetActive(false);
        }

        public void CbOnScreenTapped()
        {
            UiHolder.StateController.FTUEManager.SkipToNext();
        }

        public void EnableTapToSkipButton()
        {
            ToggleRaycastTarget(true);
            btnTapToSkip.interactable = true;
        }

        public void DisableTapToSkipButton()
        {
            btnTapToSkip.interactable = false;
        }

        public void UpdateHandPosition(Vector3 pos)
        {
            if (!imgHand.gameObject.activeSelf)
                ToggleHandDisplay(true);
            imgHand.rectTransform.anchoredPosition = pos;
        }

        public void ToggleHandDisplay(bool enable)
        {
            imgHand.gameObject.SetActive(enable);
        }

        public void ToggleRaycastTarget(bool enable)
        {
            btnTapToSkip.GetComponent<Image>().raycastTarget = enable;
        }

        public void FlipHand(bool flipToX)
        {
            if (flipToX)
            {
                imgHand.rectTransform.localScale = new Vector3(-1, 1, 1);
            }
        }

        public void ResetHandFlip()
        {
            imgHand.rectTransform.localScale = Vector3.one;
        }
    }
}