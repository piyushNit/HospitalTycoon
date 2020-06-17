using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Management.UI
{
    public class UiBoosterTimer : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI txtTimer;
        [SerializeField] Image imgBooster;
        [SerializeField] Management.Hospital.Scriptable.BoostersTypeContainer boosterContainer;

        Management.Hospital.BoosterType boosterType;

        public void UpdateUI(Management.Hospital.BoosterType _boosterType)
        {
            boosterType = _boosterType;
            imgBooster.sprite = boosterContainer.GetBoosterSpr(boosterType);
        }

        private void OnEnable()
        {
            GameManager.OnTimeBoosterUpdate += CbOnTimeBooster;
        }

        private void OnDisable()
        {
            GameManager.OnTimeBoosterUpdate -= CbOnTimeBooster;
        }

        void CbOnTimeBooster(Management.Hospital.BoosterType type, System.TimeSpan time)
        {
            if (type != boosterType)
                return;
            txtTimer.text = time.Minutes.ToString() + ":" + time.Seconds.ToString();

            if (time.TotalSeconds <= 0)
                Destroy(gameObject);
        }
    }
}