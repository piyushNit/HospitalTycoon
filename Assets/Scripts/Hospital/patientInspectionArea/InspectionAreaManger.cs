using System.Collections;
using System.Collections.Generic;
using Arch.Core;
using UnityEngine;

namespace Management.Hospital
{
    public class InspectionAreaManger : MonoBehaviour, Arch.Core.iManagerBase
    {
        InspectionArea[] inspectionArea;

        public void Initilize(StateController _stateController)
        {
            inspectionArea = GetComponentsInChildren<InspectionArea>();
        }

        public void LoadGameData()
        {
        }

        public void OnGameFocused(bool hasFocus)
        {
        }

        public void OnStateChange(StateController _stateController)
        {
            switch (_stateController.CurrSubState)
            {
                case SubStates.PregameInit:
                    LoadGameData();
                    break;
                /*case SubStates.PregameLevelSet:
                    break;
                case SubStates.PregameUiUpdate:
                    break;
                case SubStates.PregameFinished:
                    break;
                case SubStates.IngameInit:
                    break;
                case SubStates.IngameFinished:
                    break;
                case SubStates.PostInit:
                    break;
                case SubStates.Result:
                    break;
                case SubStates.PostFinished:
                    break;*/
            }
        }

        public void ExamineAndLoadGame(StateController _stateController)
        {
        }

        public void SaveGameData()
        {
        }


        /// <summary>
        /// Gets the random position from area
        /// If returns zero means area not defined
        /// </summary>
        /// <param name="areaPoint"></param>
        /// <returns></returns>
        public Vector3 GetRandomPositionFromArea(InspectionArea.InspectionAreaPoint areaPoint)
        {
            for (int i = 0; i < inspectionArea.Length; i++)
            {
                if (inspectionArea[i].InspectinoPoint == areaPoint)
                {
                    return inspectionArea[i].GetRandomPosition();
                }
                else
                {
                    continue;
                }
            }
            Debug.LogError("GameError: Area is not defined in the Inspection area");
            return Vector3.zero;
        }
    }
}
