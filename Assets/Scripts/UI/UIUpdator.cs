using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Management.UI
{
    public class UIUpdator : MonoBehaviour
    {
        [SerializeField] protected Image imgDiagnsisComplete;

        /// <summary>
        /// Fill amount should be 0-1 range
        /// </summary>
        /// <param name="amount"></param>
        public virtual void UpdateImageAmount(float amount)
        {
            imgDiagnsisComplete.fillAmount = amount;
        }

        public virtual void ResetUI()
        {
            imgDiagnsisComplete.fillAmount = 0;
        }
    }
}