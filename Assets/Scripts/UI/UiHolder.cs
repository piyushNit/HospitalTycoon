using System.Collections.Generic;
using Arch.Core;
using UnityEngine;

namespace Management.UI
{
    public class UiHolder : MonoBehaviour, Arch.Core.iManagerBase
    {
        [SerializeField] UiUtilities uiUtilities;
        public UiUtilities UiUtilities { get => uiUtilities; }

        List<UiCoreHolder> uiElements;
        Arch.Core.StateController stateController;
        public Arch.Core.StateController StateController { get => stateController; }

        private UiType currActiveUI = UiType.NONE;
        public UiType CurrActiveUI { get => currActiveUI; }

        [SerializeField] PopupContainer popupContainer;
        public PopupContainer PopupContainer { get => popupContainer; }

        [SerializeField] UiPlayerIncomeTab refUiPlayerIncomeTab;
        public UiPlayerIncomeTab PlayerIncomeTab { get => refUiPlayerIncomeTab; }

        public UI_FTUE UI_FTUE { get => GetUI<UI_FTUE>(UiType.FTUE); }

        [HideInInspector] public bool ignoreTouchWhenNotificationClicked = false;

        List<PopUpCore> activePopupList = new List<PopUpCore>();

        Management.UI.PopupGeneric internetConnectionPopup = null;

        [Header("ONLY FOR DEBUG")]
        public Management.GameDebug.UiDebugPrinter uiDebugPrinter;

        public void Initilize(Arch.Core.StateController _stateController)
        {
            stateController = _stateController;
            uiElements = new List<UiCoreHolder>();
            foreach (Transform childTrans in transform)
            {
                UiCoreHolder uiHolder = childTrans.GetComponent<UiCoreHolder>();
                uiHolder.SetReferenceOfParent(this);
                uiElements.Add(uiHolder);
            }

            #if TEST_GAME
            uiDebugPrinter.gameObject.SetActive(true);
            #endif
        }

        public void LoadGameData()
        {
        }

        public void SaveGameData()
        {
        }

        public void OnGameFocused(bool hasFocus)
        {
        }


        public UiCoreHolder GetActiveCoreUI()
        {
            return uiElements.Find(obj => obj.UiType == CurrActiveUI);
        }

        public void OnStateChange(Arch.Core.StateController _stateController)
        {
            switch (_stateController.CurrSubState)
            {
                case Arch.Core.SubStates.PregameInit:
                    LoadGameData();
                    break;
                case Arch.Core.SubStates.PregameLevelSet:
                    break;
                case Arch.Core.SubStates.PregameUiUpdate:
                    break;
                case Arch.Core.SubStates.PregameFinished:
                    break;
                case Arch.Core.SubStates.IngameInit:
                    ReloadPlayerScoreInHeaderUI();
                    refUiPlayerIncomeTab.UpdateIncomePerMin();
                    break;
                case Arch.Core.SubStates.IngameFinished:
                    break;
                case Arch.Core.SubStates.PostInit:
                    break;
                case Arch.Core.SubStates.Result:
                    break;
                case Arch.Core.SubStates.PostFinished:
                    break;
            }
        }

        /// <summary>
        /// Reloads the player income to Player score UI
        /// </summary>
        public void ReloadPlayerScoreInHeaderUI()
        {
            refUiPlayerIncomeTab.UpdatePlayerScore(StateController.GameManager.MasterLoader.PlayerScoreModel);
        }

        public void ExamineAndLoadGame(StateController _stateController)
        {
        }

        /// <summary>
        /// Enable UI with the UI type
        /// And disables the rest of the UI
        /// </summary>
        /// <param name="_uiType"></param>
        /// <param name="forceHidePlayerScoreTab"></param>
        /// forceHidePlayerScoreTab will forcefully disable the player score header, once hide remember to unhide it again
        public void ShowUi(UiType _uiType, bool forceHidePlayerScoreTab = false, bool playMenuSound = true)
        {
            if (_uiType == UiType.FTUE)
            {
                uiElements.Find(obj => obj.UiType == _uiType).ToggleUI(true);
                Debug.Log(_uiType.ToString() + " Is UI Active: " + uiElements.Find(obj => obj.UiType == _uiType).gameObject.activeSelf);
                return;
            }
            if (!CanOpenUI() && _uiType != UiType.HUD && (currActiveUI != UiType.Welcome_Back_Screen && _uiType != UiType.Hire_Admin))
                return;
            currActiveUI = _uiType;
            foreach (UiCoreHolder element in uiElements)
            {
                if (element.UiType == UiType.Player_Score_Header && forceHidePlayerScoreTab == false
                    || element.UiType == UiType.Utilities || element.UiType == UiType.FTUE)
                    continue;
                element.ToggleUI(element.UiType == _uiType);
            }

            if (playMenuSound)
                stateController.GameAudioManager.PlaySound(Audio.AudioType.MENU_POP_UP);
        }

        bool CanOpenUI()
        {
            return currActiveUI == UiType.HUD || currActiveUI == UiType.NONE;
        }

        public void ShowPlayerScoreTab()
        {
            uiElements.Find(obj => obj.UiType == UiType.Player_Score_Header).ToggleUI(true);
        }

        public T GetUI<T>(UiType _uiType)
        {
            return uiElements.Find(obj => obj.UiType == _uiType).GetComponent<T>();
        }

        /// <summary>
        /// ShowUI and loads department information
        /// </summary>
        /// <param name="_uiType"></param>
        /// <param name="_type"></param>
        public void ShowUiWithDepartmentInfo(UiType _uiType, Management.Hospital.Department department)
        {
            if (!CanOpenUI())
                return;
            ShowUi(_uiType);
            Ui_Department uiDepartment = (Ui_Department)uiElements.Find(obj => obj.UiType == _uiType);
            uiDepartment.LoadDepartmentData(department);
            if (department.UiType == UiType.Hospital_Department
                && department.DepartmentType == Hospital.Core.DepartmentType.ENT
                && StateController.FTUEManager.IsFTUERunning
                && StateController.FTUEManager.CurrentFtueType == FTUE.FTUEType.FTUE_IMPROVE_ENT_CLINIC)
                stateController.FTUEManager.ShowFTUE(FTUE.FTUEType.FTUE_ENT_UPGRADE_CLICK);
        }

        /// <summary>
        /// ShowUI and loads department information
        /// </summary>
        /// <param name="_uiType"></param>
        /// <param name="_type"></param>
        public void ShowUiWithDepartmentInfo(UiType _uiType, Management.Hospital.PaymentDepartment paymentDepartment)
        {
            if (!CanOpenUI())
                return;
            ShowUi(_uiType);
            Ui_Department uiDepartment = (Ui_Department)uiElements.Find(obj => obj.UiType == _uiType);
            uiDepartment.LoadDepartmentData(paymentDepartment);
        }

        public void ShowParkingInfo(UiType _uiType = UiType.Parking)
        {
            ShowUi(_uiType);
            if (stateController.FTUEManager.IsFTUERunning && stateController.FTUEManager.CurrentFtueType == FTUE.FTUEType.FTUE_PARKING_CLICK)
            {
                stateController.FTUEManager.SkipToNext();
            }
        }

        /// <summary>
        /// Close all active tabs and opens HUD tab
        /// </summary>
        public void CloseAllAndOpenHUD()
        {
            ShowUi(UiType.HUD);
        }

        /// <summary>
        /// Adds into popup list
        /// Adding popups into list is important to take decisions like raycast blocking
        /// </summary>
        /// <param name="popup"></param>
        public void AddIntoPopupList(PopUpCore popup)
        {
            activePopupList.Add(popup);
        }

        /// <summary>
        /// Removes the popup from list
        /// </summary>
        /// <param name="popup"></param>
        public void RemoveFromPopupList(PopUpCore popup)
        {
            if (activePopupList.Contains(popup))
                activePopupList.Remove(popup);
        }

        public bool IsPopupActive()
        {
            return activePopupList.Count != 0;
        }

        public void ShowWelcomeScreen()
        {
            ShowUi(UiType.Welcome_Back_Screen);
        }

        public void ShowHireAdmingUi()
        {
            ShowUi(UiType.Hire_Admin);
        }

        public void ShowInternetConnectionErrorPopup()
        {
            if (internetConnectionPopup != null)
                return;

            internetConnectionPopup = Instantiate(popupContainer.GetPopupPrefab(PopupType.GENERIC_POPUP)) as Management.UI.PopupGeneric;
            internetConnectionPopup.Initilize(this, CbInternetConnectionCallback);
            internetConnectionPopup.UpdateContent("No Internet Connection", "Please connect with the internet to use internet related featues", "Ok");
        }

        void CbInternetConnectionCallback(int btnIndex, object customData)
        {
            internetConnectionPopup.Close();
            internetConnectionPopup = null;
        }
    }
}