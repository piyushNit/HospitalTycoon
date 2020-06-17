using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Hospital.Core
{
    public class BaseEntity : MonoBehaviour
    {
        public enum MoodState
        {
            NORMAL,
            RELEAVED,
            CONFUSED,
            FRUSTRATE,
            ANGRY,
            SICK
        }

        protected Arch.Core.StateController refStateController;
   
        public virtual void InitStateController(Arch.Core.StateController _stateController)
        {
            refStateController = _stateController;
        }

        public IEnumerator InitilizeTiming(float workTime, Management.UI.UIUpdator refUiUpdator)
        {
            float treatmentTime = workTime;
            float currTreatmentTime = 0;
            while (currTreatmentTime <= treatmentTime)
            {
                currTreatmentTime += Time.deltaTime;
                UpdateImageAmount(currTreatmentTime / treatmentTime);
                refUiUpdator.UpdateImageAmount(currTreatmentTime / treatmentTime);
                yield return null;
            }
            ResetUI();
        }

        protected virtual void UpdateImageAmount(float time)
        {
        }

        protected virtual void ResetUI()
        {
        }

        protected virtual void ChangeMoodState(MoodState moodState)
        {
        }
    }
}