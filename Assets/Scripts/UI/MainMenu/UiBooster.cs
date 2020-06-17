using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

namespace Management.Hospital
{
    public enum BoosterType
    {
        Doctor_Time_Booster = 0,
        Cashier_Time_Booster
    }
}

namespace Management.UI
{
    public class UiBooster : MonoBehaviour
    {
        [SerializeField] Image imgBooster;
        [SerializeField] float shakeTime = 0.8f;
        [SerializeField] float shakeStrength = 10;
        [SerializeField] float shakeDelaySec = 2;
        [SerializeField] Management.Hospital.Scriptable.BoostersTypeContainer boosterContainer;

        Management.Hospital.BoosterType boosterType;
        public Management.Hospital.BoosterType BoosterType { get => boosterType; }

        System.Action<UiBooster> CallbackBooster;

        public void UpdateBooster(Management.Hospital.BoosterType _boosterType, System.Action<UiBooster> cbBoosterCallback)
        {
            CallbackBooster = cbBoosterCallback;
            boosterType = _boosterType;
            imgBooster.sprite = boosterContainer.GetBoosterSpr(boosterType);

            StartCoroutine(ShakeRotation());
        }

        IEnumerator ShakeRotation()
        {
            WaitForSeconds waitSecond = new WaitForSeconds(shakeDelaySec);
            while (gameObject != null)
            {
                yield return waitSecond;
                transform.DOShakeRotation(shakeTime, shakeStrength);
            }
        }

        public void Remove()
        {
            Destroy(gameObject);
        }

        public void CbOnBoosterClicked()
        {
            if (CallbackBooster != null)
                CallbackBooster(this);
        }

        private void OnDisable()
        {
            transform.DOKill();
        }
    }
}