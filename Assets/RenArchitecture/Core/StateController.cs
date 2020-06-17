using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 1. This class doesnt contains any implementation only use for changing the states of the game
 */

namespace Arch.Core
{
    public class StateController : MonoBehaviour
    {
        #region MANAGERS_VARIABLES
        [Header("Managers References")]
        [SerializeField] Management.core.PlayerInputManager playerInputManager;
        public Management.core.PlayerInputManager PlayerInputManager { get => playerInputManager; }

        [SerializeField] Management.GameManager gameManager;
        public Management.GameManager GameManager { get => gameManager; }

        [SerializeField] Management.CarSpawner carSpawner;
        public Management.CarSpawner CarSpawner { get => carSpawner; }

        [SerializeField] Management.Parking.ParkingManager parkingManager;
        public Management.Parking.ParkingManager ParkingManager { get => parkingManager; }

        [SerializeField] Management.Hospital.HospitalManager hospitalManager;
        public Management.Hospital.HospitalManager HospitalManager { get => hospitalManager; }

        [SerializeField] Management.UI.UiHolder uiHolder;
        public Management.UI.UiHolder UiHolder { get => uiHolder; }

        [SerializeField] Management.Hospital.InspectionAreaManger inspectionAreaMgr;
        public Management.Hospital.InspectionAreaManger InspectionAreaManager { get => inspectionAreaMgr; }

        [SerializeField] Management.Audio.GameAudioManager gameAudioManager;
        public Management.Audio.GameAudioManager GameAudioManager { get => gameAudioManager; }

        [SerializeField] Management.FTUE.FTUEManager ftueManager;
        public Management.FTUE.FTUEManager FTUEManager { get => ftueManager; }
        #endregion

        #region SCRIPTABLES
        [Header("Container References")]
        [SerializeField] Management.Patient.PatientTypeScriptable patientScriptable;
        public Management.Patient.PatientTypeScriptable PatientScriptable { get => patientScriptable; }

        [SerializeField] Management.Car.CarDataContainer carDataScriptable;
        public Management.Car.CarDataContainer CarDataScriptable { get => carDataScriptable; }

        [SerializeField] Management.Doctor.DoctorDataContainer doctorDataContainer;
        public Management.Doctor.DoctorDataContainer DoctorDataScriptable { get => doctorDataContainer; }

        [SerializeField] Management.Hospital.Scriptable.DepartmentUpgradeDataScriptable departmentUpgradeScriptable;
        public Management.Hospital.Scriptable.DepartmentUpgradeDataScriptable DepartmentUpgradeScriptable { get => departmentUpgradeScriptable; }

        [SerializeField] Management.Hospital.Scriptable.HospitalJsonDataScriptable hospitalJsonDataScriptable;
        public Management.Hospital.Scriptable.HospitalJsonDataScriptable HospitalJsonDataScriptable { get => hospitalJsonDataScriptable; }
        #endregion

        Core.State currentState;
        public Core.State CurrState { get => currentState; }
        Core.SubStates currSubState;
        public Core.SubStates CurrSubState { get => currSubState; }

        System.Action<StateController> CbOnStateChange;

        Management.UI.PopupGeneric popupGeneric = null;

        //This is the Starting point of the game
        private void Start()
        {
            //Initlize managers
            GameManager.Initilize(this);
            ParkingManager.Initilize(this);
            CarSpawner.Initilize(this);
            HospitalManager.Initilize(this);
            UiHolder.Initilize(this);
            inspectionAreaMgr.Initilize(this);
            gameAudioManager.Initilize(this);
            ftueManager.Initilize(this);

            //At last change state to init game
            if (PlayerPrefs.GetInt(Management.SaveLoad.KEYS.KEY_USER_SILENT_LOGIN, 0) != 1
                && Management.Services.GooglePlayServices.Instance != null
                && !Management.Services.GooglePlayServices.Instance.IsUserAuthenticated())
            {
                //show popup here
                popupGeneric = (Management.UI.PopupGeneric)Instantiate(UiHolder.PopupContainer.GetPopupPrefab(Management.UI.PopupType.GENERIC_POPUP));
                popupGeneric.Initilize(UiHolder, CbLoginFuncCallback);
                popupGeneric.UpdateContent("LOG IN", "Please login with google to save your data on cloud", "No Thanks", "Log-In");
            }
            else
            {
                ChangeSubState(SubStates.PregameLoad);
            }
        }

        void CbLoginFuncCallback(int btnIndex, object customData)
        {
            switch (btnIndex)
            {
                case 0:
                    popupGeneric.Close();
                    popupGeneric = null;
                    ChangeSubState(SubStates.PregameLoad);
                    break;
                case 1:
                    Management.Services.GooglePlayServices.Instance.SignIn((bool success) =>
                    {
                        popupGeneric.Close();
                        if (success)
                            PlayerPrefs.SetInt(Management.SaveLoad.KEYS.KEY_USER_SILENT_LOGIN, 1);
                        ChangeSubState(SubStates.PregameLoad);
                    });
                    break;
            }
        }

        private void OnEnable()
        {
            CbOnStateChange += GameManager.OnStateChange;
            CbOnStateChange += ParkingManager.OnStateChange;
            CbOnStateChange += CarSpawner.OnStateChange;
            CbOnStateChange += HospitalManager.OnStateChange;
            CbOnStateChange += UiHolder.OnStateChange;
            CbOnStateChange += InspectionAreaManager.OnStateChange;
            CbOnStateChange += gameAudioManager.OnStateChange;
            CbOnStateChange += ftueManager.OnStateChange;
        }

        private void OnDisable()
        {
            CbOnStateChange -= ParkingManager.OnStateChange;
            CbOnStateChange -= CarSpawner.OnStateChange;
            CbOnStateChange -= HospitalManager.OnStateChange;
            CbOnStateChange -= UiHolder.OnStateChange;
            CbOnStateChange -= GameManager.OnStateChange;
            CbOnStateChange -= InspectionAreaManager.OnStateChange;
            CbOnStateChange -= gameAudioManager.OnStateChange;
            CbOnStateChange -= ftueManager.OnStateChange;
        }

        #region CHANGE_STATES
        public void ChangeState(Core.State _state, Core.SubStates _subState)
        {
            currentState = _state;

            switch (currentState)
            {
                case Core.State.PREGAME:
                    break;
                case Core.State.INGAME:
                    break;
                case Core.State.POSTGAME:
                    break;
            }
        }

        /// <summary>
        /// Chnages substate and sets main state with appropriate state
        /// </summary>
        /// <param name="_subState"></param>
        public void ChangeSubState(Core.SubStates _subState)
        {
            Debug.Log("HOSPITAL_TYCOON: Log states: " + _subState.ToString());
            currSubState = _subState;
            switch (currSubState)
            {
                case Core.SubStates.PregameLoad:
                    uiHolder.UiUtilities.ShowSplashScreen();
                    currentState = Core.State.PREGAME;
                    DispatchEvent();
                    //Game Manager will change state to PregameLevelSet after load game success
                    break;
                case Core.SubStates.PregameInit:
                    uiHolder.UiUtilities.HideSplashScreen();
                    currentState = Core.State.PREGAME;
                    DispatchEvent();
                    ChangeSubState(SubStates.PregameLevelSet);
                    break;
                case Core.SubStates.PregameLevelSet:
                    currentState = Core.State.PREGAME;
                    DispatchEvent();
                    ChangeSubState(SubStates.PregameUiUpdate);
                    break;
                case Core.SubStates.PregameUiUpdate:
                    currentState = Core.State.PREGAME;
                    DispatchEvent();
                    ChangeSubState(SubStates.PregameFinished);
                    break;
                case Core.SubStates.PregameFinished:
                    currentState = Core.State.PREGAME;

                    InitilizeGame();
                    DispatchEvent();
                    if(FTUEManager.IsFTUERunning)
                        ChangeSubState(SubStates.FTUE_IN_GAME_INIT);
                    else
                        ChangeSubState(SubStates.IngameInit);
                    break;
                case Core.SubStates.IngameInit:
                    currentState = Core.State.INGAME;
                    DispatchEvent();
                    break;

                case SubStates.FTUE_IN_GAME_INIT:
                    currentState = Core.State.INGAME;
                    DispatchEvent();
                    break;
                case SubStates.FTUE_PART_ONE_FINISHED:
                    currentState = Core.State.INGAME;
                    DispatchEvent();
                    break;

                case Core.SubStates.IngameFinished:
                    currentState = Core.State.INGAME;
                    DispatchEvent();
                    break;
                case Core.SubStates.PostInit:
                    currentState = Core.State.POSTGAME;
                    DispatchEvent();
                    break;
                case Core.SubStates.Result:
                    currentState = Core.State.POSTGAME;
                    DispatchEvent();
                    break;
                case Core.SubStates.PostFinished:
                    currentState = Core.State.POSTGAME;
                    DispatchEvent();
                    break;

            }
        }
        #endregion

        void DispatchEvent()
        {
            if (CbOnStateChange != null)
                CbOnStateChange(this);
        }

        /// <summary>
        /// Initlize all game corooutines here
        /// </summary>
        public void InitilizeGame()
        {
            if (ftueManager.IsFTUERunning)
                ftueManager.ShowFTUE(Management.FTUE.FTUEType.FTUE_WELCOME);
            else
                UiHolder.ShowUi(Management.UI.UiType.HUD, playMenuSound:false);
        }

        #region NATIVE_ANDROID
        private void OnApplicationFocus(bool focus)
        {
            //Tells the managers what to do
            GameManager.OnGameFocused(focus);
            //ParkingManager.OnGameFocused(focus);
            //CarSpawner.OnGameFocused(focus);
            //HospitalManager.OnGameFocused(focus);
            //UiHolder.OnGameFocused(focus);
            //inspectionAreaMgr.OnGameFocused(focus);
            //gameAudioManager.OnGameFocused(focus);
        }
        #endregion
    }
}
