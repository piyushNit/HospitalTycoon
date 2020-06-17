using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Management.Patient
{
    public class PatientExpressionBox : MonoBehaviour
    {
        [SerializeField] GameObject expressionDialogBox;
        [SerializeField] SpriteRenderer imgExpression;
        public void ShowExpression(Sprite expressionSprite, float visibleTime = 2, System.Action callback = null)
        {
            gameObject.SetActive(true);
            imgExpression.sprite = expressionSprite;
            PlayExpressionAnimation(0.8f, 1);
            StartCoroutine(ResetExpression(visibleTime, callback));
        }

        void PlayExpressionAnimation(float initialScale, float finalScale, System.Action callback = null)
        {
            expressionDialogBox.transform.localScale = Vector3.one * initialScale;
            expressionDialogBox.transform.DOScale(finalScale, 1.0f).OnComplete(() => {
                if (callback != null)
                    callback();
            });
        }

        IEnumerator ResetExpression(float time, System.Action callback = null)
        {
            yield return new WaitForSeconds(time);
            PlayExpressionAnimation(1, 0.8f, () => {
                if (callback != null)
                    callback();
            });
            gameObject.SetActive(false);
        }
    }
}
