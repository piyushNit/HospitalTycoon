using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Management.UI
{
    public class Ui_WaitingLobby : UiCoreHolder
    {
        #region UI_REFERENCES
        [SerializeField] Button btnUnlock;
        [SerializeField] TextMeshProUGUI txtUnlockCost;
        [SerializeField] TextMeshProUGUI txtAvailableSeats;

        #endregion

        Management.Hospital.Json.WaitingLobbyUnlockModel waitingLobbyJson;

        public override void OnEnable()
        {
            base.OnEnable();
            waitingLobbyJson = Arch.Json.JsonReader.LoadJson<Management.Hospital.Json.WaitingLobbyUnlockModel>
                                (UiHolder.StateController.HospitalJsonDataScriptable.WaitingLobbyJson.text);

            UpdateUI();
        }

        decimal GetUnlockCost()
        {
            return (decimal)((UiHolder.StateController.GameManager.MasterLoader.WaitingLobbySaveModel.numberOfSeatsUnlocked * waitingLobbyJson.WaitingLobbyBaseCost) * waitingLobbyJson.WaitingLobbyMultuplyBy);
        }

        #region UI_CALLBAKCS
        public void CbOnUpdateWaitingLobby()
        {
            decimal unlockCost = GetUnlockCost();
            if (!uiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(unlockCost))
                return;
            uiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.DeductCash(unlockCost);
            uiHolder.ReloadPlayerScoreInHeaderUI();
            UiHolder.StateController.GameAudioManager.PlaySound(Audio.AudioType.UPGRADE_BTN_CLICKED);

            UiHolder.StateController.HospitalManager.HospitalBuilding.WaitingLobby.UnlockNewSeats();

            UpdateUI();
        }
        #endregion

        void UpdateUI()
        {
            decimal unlockCost = GetUnlockCost();
            btnUnlock.interactable = uiHolder.StateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(unlockCost);
            txtUnlockCost.text = Utils.FormatWithMeasues(unlockCost);

            txtAvailableSeats.text = uiHolder.StateController.HospitalManager.HospitalBuilding.WaitingLobby.GetUnlockedSeatCount().ToString() + "/" +
                uiHolder.StateController.HospitalManager.HospitalBuilding.WaitingLobby.GetTotalSeatsCount().ToString();
        }
    }
}
