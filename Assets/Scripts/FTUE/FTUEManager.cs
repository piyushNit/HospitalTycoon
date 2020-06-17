using Arch.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Management.FTUE
{
    public enum FTUEType
    {
        //Part 1
        NONE = 0,
        FTUE_WELCOME,
        FTUE_PATIENT_CAR_INCOMING,
        FTUE_PATIENT_CAR_PARKING,

        FTUE_PATIENT_CONSULTING,
        FTUE_PATIENT_TREATED,

        FTUE_IMPROVE_ENT_CLINIC,
        FTUE_ENT_UPGRADE_CLICK,
        FTUE_ENT_UPGRADE_ADVICE,

        //Part 2
        FTUE_PARING_FULL,
        FTUE_PARKING_CLICK,
        FTUE_PARKING_UPGRADE_CLICK,
        FTUE_PARKING_UPGRADE_ADVICE,

        FTUE_PARKING_ADVERTISING,

        FTUE_END,

        // Part 3
        FTUE_PATINETS_WAIT_TOO_LONG,
        FTUE_HIRE_ANOTHER_RECEPTIONIST,
        FTUE_RECEPTIONIST_HIRE_CLICK,//Frame missing in Tutorial PDF
        FTUE_RECEPTIONIST_HIRE_CONGRATS,

        FTUE_CLOSING_MESSAGE_1,//Tap the office to unlock new deapartments
        FTUE_CLOSING_MESSAGE_2, // For now focus on ENT clinic
        FTUE_CLOSING_MESSAGE_3, //Able to open hospital in new city

        FTUE_ENT_UI_CLOSE_CLICK// Part 2
    }

    [System.Serializable]
    public class FTUEHandler
    {
        public FTUEType ftueType;
        public UnityEvent startTutorialFuncCallback;
        public UnityEvent endTutorialFuncCallback;
    }

    public class FTUEManager : MonoBehaviour, Arch.Core.iManagerBase
    {
        Arch.Core.StateController refStateController;

        [SerializeField] List<FTUEHandler> ftueHandlers;
        [SerializeField] FTUEDataContainer ftueDataContainer;

        bool isFTUERunning = true;
        public bool IsFTUERunning { get => isFTUERunning; }

        bool isCameraDrag = false;
        public bool IsCameraDrag { get => isCameraDrag; }

        FTUEType currentFTUEType;
        public FTUEType CurrentFtueType { get => currentFTUEType; }

        public void Initilize(StateController _stateController)
        {
            refStateController = _stateController;
            isFTUERunning = true;//Update this from MasterLoader.PlayerConfig.FTUE
        }

        object customData = null;

        Coroutine timeoutCoroutine = null;

        public void LoadGameData(){}
        public void OnGameFocused(bool focus){}
        public void OnStateChange(StateController _stateController){}
        public void SaveGameData() {}

        /// <summary>
        /// Shows FTUE based on type
        /// </summary>
        /// <param name="_ftueType"></param>
        public void ShowFTUE(FTUEType _ftueType)
        {
            currentFTUEType = _ftueType;
            FTUEData ftueData = ftueDataContainer.GetFTUE(currentFTUEType);
            if (ftueData == null)
            {
                Debug.LogError("GAME ERROR!! Ftue Data for '" + currentFTUEType.ToString() + "' Not Found");
                return;
            }

            FTUEHandler ftueHandler = ftueHandlers.Find(obj => obj.ftueType == currentFTUEType);

            refStateController.UiHolder.ShowUi(UI.UiType.FTUE);

            Management.UI.UI_FTUE refUIFTUE = refStateController.UiHolder.UI_FTUE;
            if (!System.String.IsNullOrEmpty(ftueData.message))
                refUIFTUE.ShowSpeechBubble(ftueData.message);
            else
                refUIFTUE.DisableSpeechBubble();

            if (ftueData.sprDoctor != null)
                refUIFTUE.UpdateDoctorSpr(ftueData.sprDoctor);
            else
                refUIFTUE.DisableDoctorSpr();

            if (ftueHandler != null)
            {
                ftueHandler.startTutorialFuncCallback.Invoke();
            }

            switch (currentFTUEType)
            {
                case FTUEType.FTUE_WELCOME:
                    break;
                case FTUEType.FTUE_PATIENT_CAR_INCOMING:
                    refStateController.GameManager.SpawnFTUECar();
                    refStateController.UiHolder.UI_FTUE.DisableTapToSkipButton();
                    break;
                case FTUEType.FTUE_PATIENT_CAR_PARKING:
                    refStateController.GameManager.GetFTUECar().SetCarIntoParking(false);
                    break;
                case FTUEType.FTUE_PATIENT_CONSULTING:
                    break;
                case FTUEType.FTUE_PATIENT_TREATED:
                    break;
                case FTUEType.FTUE_IMPROVE_ENT_CLINIC:
                    RemoveCameraFollower();
                    break;
                case FTUEType.FTUE_ENT_UPGRADE_CLICK:
                    refStateController.UiHolder.UI_FTUE.ToggleRaycastTarget(false);
                    break;
                case FTUEType.FTUE_ENT_UPGRADE_ADVICE:
                    refStateController.UiHolder.UI_FTUE.EnableTapToSkipButton();
                    break;
                case FTUEType.FTUE_ENT_UI_CLOSE_CLICK:
                    refStateController.UiHolder.GetUI<Management.UI.Ui_Department>(UI.UiType.Hospital_Department).DisableUpgradeAndStffSalaryBtn();
                    refStateController.UiHolder.UI_FTUE.ToggleRaycastTarget(false);
                    break;
                case FTUEType.FTUE_PARING_FULL:
                    refStateController.UiHolder.UI_FTUE.EnableTapToSkipButton();
                    break;
                case FTUEType.FTUE_PARKING_CLICK:
                    refStateController.UiHolder.UI_FTUE.DisableTapToSkipButton();
                    isCameraDrag = false;
                    break;
                case FTUEType.FTUE_PARKING_UPGRADE_CLICK:
                    refStateController.UiHolder.UI_FTUE.ToggleRaycastTarget(false);
                    break;
                case FTUEType.FTUE_PARKING_UPGRADE_ADVICE:
                    refStateController.UiHolder.UI_FTUE.EnableTapToSkipButton();
                    break;
                case FTUEType.FTUE_PARKING_ADVERTISING:
                    refStateController.UiHolder.GetUI<UI.Ui_Parking>(UI.UiType.Parking).FTUEReloadAdvertisement();
                    break;
                case FTUEType.FTUE_END:
                    break;
                case FTUEType.FTUE_PATINETS_WAIT_TOO_LONG:
                    break;
                case FTUEType.FTUE_HIRE_ANOTHER_RECEPTIONIST:
                    break;
                case FTUEType.FTUE_RECEPTIONIST_HIRE_CLICK:
                    break;
                case FTUEType.FTUE_RECEPTIONIST_HIRE_CONGRATS:
                    break;
                case FTUEType.FTUE_CLOSING_MESSAGE_1:
                    break;
                case FTUEType.FTUE_CLOSING_MESSAGE_2:
                    break;
                case FTUEType.FTUE_CLOSING_MESSAGE_3:
                    break;
            }

            AnalyzeCustomData(ftueData);
        }

        void AnalyzeCustomData(FTUEData ftueData)
        {
            if (ftueData.customData.Length == 0)
            {
                refStateController.UiHolder.UI_FTUE.ToggleHandDisplay(false);
                return;
            }
            if (!ftueData.customData.Contains("HAND="))
                refStateController.UiHolder.UI_FTUE.ToggleHandDisplay(false);

            string[] allData = ftueData.customData.Contains(";") ? ftueData.customData.Split(';') : new string[] { ftueData.customData };
            for (int i = 0; i < allData.Length; i++)
            {
                if (allData[i].Contains("HAND="))
                {
                    Vector3 handPosition = new Vector3();
                    int indexOfColon = allData[i].IndexOf('=');
                    string[] data = allData[i].Substring(indexOfColon, allData[i].Length - indexOfColon).Split(',');
                    for (int j = 0; j < data.Length; ++j)
                    {
                        if (data[j].ToLower().Contains("x"))
                        {
                            handPosition.x = float.Parse(data[j].Split(':')[1]);
                        }
                        if (data[j].ToLower().Contains("y"))
                        {
                            handPosition.y = float.Parse(data[j].Split(':')[1]);
                        }
                        if (data[j].ToLower().Contains("z"))
                        {
                            handPosition.z = float.Parse(data[j].Split(':')[1]);
                        }
                    }
                    refStateController.UiHolder.UI_FTUE.UpdateHandPosition(handPosition);
                }

                if (allData[i].Contains("CAMERA="))
                {
                    Vector3 cameraPos = new Vector3();
                    int indexOfColon = allData[i].IndexOf('=');
                    string[] data = allData[i].Substring(indexOfColon, allData[i].Length - indexOfColon).Split(',');
                    for (int j = 0; j < data.Length; ++j)
                    {
                        if (data[j].ToLower().Contains("x"))
                        {
                            cameraPos.x = float.Parse(data[j].Split(':')[1]);
                        }
                        if (data[j].ToLower().Contains("y"))
                        {
                            cameraPos.y = float.Parse(data[j].Split(':')[1]);
                        }
                        if (data[j].ToLower().Contains("z"))
                        {
                            cameraPos.z = float.Parse(data[j].Split(':')[1]);
                        }
                    }
                    refStateController.PlayerInputManager.UpdateCameraPosition(cameraPos);
                }
                if (allData[i].Contains("HANDFLIP="))
                {
                    string[] data = allData[i].Split('=');
                    refStateController.UiHolder.UI_FTUE.FlipHand(data[1].ToUpper() == "X");
                }

                if (allData[i].Contains("CAMFOLLOW="))
                {
                    FollowCameraInPatientCarIncoming(allData[i]);
                }

                if (allData[i].Contains("TIMEOUT="))
                {
                    float timeout = float.Parse(allData[i].Split('=')[1]);
                    timeoutCoroutine = StartCoroutine(TimeoutFTUE(timeout));
                }
            }
        }

        IEnumerator TimeoutFTUE(float timeout)
        {
            yield return new WaitForSeconds(timeout);
            SkipToNext();
            timeoutCoroutine = null;
        }

        void FollowCameraInPatientCarIncoming(string customData)
        {
            customData = customData.Split('=')[1];
            string[] paramData = customData.Split(',');
            Transform cameraTrans = refStateController.PlayerInputManager.CameraTrans;
            Management.SimpleTween.CameraObjectFollower cameraObjectFollower = cameraTrans.gameObject.AddComponent<Management.SimpleTween.CameraObjectFollower>();

            for (int i = 0; i < paramData.Length; ++i)
            {
                string[] splitData = paramData[i].Split(':');
                var fieldInfo = cameraObjectFollower.GetType().GetProperty(splitData[0].ToUpper());
                if (paramData[i].Contains("FTUE_CAR"))
                {
                    fieldInfo.SetValue(cameraObjectFollower, refStateController.GameManager.GetFTUECar().transform);
                    cameraObjectFollower.CalculateOffset();
                }
                else 
                if(paramData[i].Contains("FTUE_PATIENT"))
                {
                    fieldInfo.SetValue(cameraObjectFollower, refStateController.GameManager.GetFTUEPatient().transform);
                    cameraObjectFollower.CalculateOffset();
                }
                else
                {
                    fieldInfo.SetValue(cameraObjectFollower, float.Parse(splitData[1]));
                }
            }
        }

        public void SkipToNext(object _customData = null)
        {
            customData = _customData;
            FTUEHandler ftueHandler = ftueHandlers.Find(obj => obj.ftueType == currentFTUEType);
            if (ftueHandler != null && ftueHandler.endTutorialFuncCallback != null)
                ftueHandler.endTutorialFuncCallback.Invoke();

            if (timeoutCoroutine != null)
            {
                StopCoroutine(timeoutCoroutine);
            }

            switch (currentFTUEType)
            {
                case FTUEType.FTUE_WELCOME:
                    ShowFTUE(FTUEType.FTUE_PATIENT_CAR_INCOMING);
                    break;
                case FTUEType.FTUE_PATIENT_CAR_INCOMING:
                    RemoveCameraFollower();
                    refStateController.UiHolder.UI_FTUE.EnableTapToSkipButton();
                    ShowFTUE(FTUEType.FTUE_PATIENT_CAR_PARKING);
                    break;
                case FTUEType.FTUE_PATIENT_CAR_PARKING:
                    refStateController.UiHolder.UI_FTUE.gameObject.SetActive(false);
                    break;
                case FTUEType.FTUE_PATIENT_CONSULTING:
                    break;
                case FTUEType.FTUE_PATIENT_TREATED:
                    break;
                case FTUEType.FTUE_IMPROVE_ENT_CLINIC:
                    isCameraDrag = true;
                    break;
                case FTUEType.FTUE_ENT_UPGRADE_CLICK:
                    ShowFTUE(FTUEType.FTUE_ENT_UPGRADE_ADVICE);
                    break;
                case FTUEType.FTUE_ENT_UPGRADE_ADVICE:
                    ShowFTUE(FTUEType.FTUE_ENT_UI_CLOSE_CLICK);
                    break;
                case FTUEType.FTUE_ENT_UI_CLOSE_CLICK:
                    refStateController.UiHolder.UI_FTUE.gameObject.SetActive(false);
                    refStateController.UiHolder.UI_FTUE.DisableTapToSkipButton();
                    refStateController.ChangeSubState(SubStates.FTUE_PART_ONE_FINISHED);
                    refStateController.UiHolder.UI_FTUE.ToggleRaycastTarget(true);
                    refStateController.UiHolder.UI_FTUE.ResetHandFlip();
                    refStateController.UiHolder.UI_FTUE.ToggleHandDisplay(false);
                    break;
                case FTUEType.FTUE_PARING_FULL:
                    ShowFTUE(FTUEType.FTUE_PARKING_CLICK);
                    break;
                case FTUEType.FTUE_PARKING_CLICK:
                    ShowFTUE(FTUEType.FTUE_PARKING_UPGRADE_CLICK);
                    break;
                case FTUEType.FTUE_PARKING_UPGRADE_CLICK:
                    ShowFTUE(FTUEType.FTUE_PARKING_UPGRADE_ADVICE);
                    break;
                case FTUEType.FTUE_PARKING_UPGRADE_ADVICE:
                    ShowFTUE(FTUEType.FTUE_PARKING_ADVERTISING);
                    break;
                case FTUEType.FTUE_PARKING_ADVERTISING:
                    break;
                case FTUEType.FTUE_END:
                    break;
                case FTUEType.FTUE_PATINETS_WAIT_TOO_LONG:
                    break;
                case FTUEType.FTUE_HIRE_ANOTHER_RECEPTIONIST:
                    break;
                case FTUEType.FTUE_RECEPTIONIST_HIRE_CLICK:
                    break;
                case FTUEType.FTUE_RECEPTIONIST_HIRE_CONGRATS:
                    break;
                case FTUEType.FTUE_CLOSING_MESSAGE_1:
                    break;
                case FTUEType.FTUE_CLOSING_MESSAGE_2:
                    break;
                case FTUEType.FTUE_CLOSING_MESSAGE_3:
                    refStateController.GameManager.MasterLoader.PlayerGameConfig.SetFTUEPlayed();
                    break;
            }
        }

        void RemoveCameraFollower()
        {
            Destroy(refStateController.PlayerInputManager.CameraTrans.GetComponent<Management.SimpleTween.CameraObjectFollower>());
        }
    }
}