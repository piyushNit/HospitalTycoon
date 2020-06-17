using System.Collections;
using System.Collections.Generic;
using Arch.Core;
using UnityEngine;

/*
 * 1. handles Parking base
 * 2. instantate parking slots as per the player unlockes
 * 3. this is a parking specific if player upgrades (Replces) this script to another object
 * */

namespace Management.Parking
{

    public class ParkingSlotHandler : MonoBehaviour, Arch.Core.iManagerBase
    {
        [SerializeField] int totalParkingSlotsCount = 150;
        public int TotalParingSlotsCount { get => totalParkingSlotsCount; }
        [SerializeField] ParkingUnit[] parkingUnitList;
        [SerializeField] ParkingTransformUpdater[] parkingObjectScalerArr;
        [SerializeField] UndergroundParking undergroundParking;
        public UndergroundParking UndergroundParking { get => undergroundParking; }

        ParkingRevealIndex currParkingRvealIndex;

        public static System.Action<int> CbOnParkingCountUpdate;

        Arch.Core.StateController refStateController;

        public void Initilize(Arch.Core.StateController _stateController)
        {
            refStateController = _stateController;
            for (int i = 0; i < parkingUnitList.Length; i++)
            {
                parkingUnitList[i].Initilize();
            }
        }

        public void LoadGameData()
        {
            currParkingRvealIndex = (Management.Parking.ParkingRevealIndex)refStateController.GameManager.MasterLoader.ParkingUnitSaveModel.parkingRevealIndex;
            UnlockParkingUnit();
            refStateController.ParkingManager.AnalyzeAndRevealParking(currParkingRvealIndex);
            refStateController.ParkingManager.WaypointPathFinder.UpdateWaypointPosition(currParkingRvealIndex);

            if (currParkingRvealIndex == ParkingRevealIndex.UNDERGROUND_PARKING)
                undergroundParking.UnlockNewSlots(refStateController.GameManager.MasterLoader.ParkingUnitSaveModel.undergroundParkingSlots);

            if (CbOnParkingCountUpdate != null)
                CbOnParkingCountUpdate(GetCountOfUnlockedParkingSlots());
        }

        public void SaveGameData()
        {
        }

        public void OnGameFocused(bool hasFocus)
        {
        }

        public void OnStateChange(Arch.Core.StateController _stateController)
        {
            switch (_stateController.CurrSubState)
            {
                case Arch.Core.SubStates.PregameInit:
                    LoadGameData();
                    break;
                /*case Arch.Core.SubStates.PregameLevelSet:
                    break;
                case Arch.Core.SubStates.PregameUiUpdate:
                    break;
                case Arch.Core.SubStates.PregameFinished:
                    break;
                case Arch.Core.SubStates.IngameInit:
                    break;
                case Arch.Core.SubStates.IngameFinished:
                    break;
                case Arch.Core.SubStates.PostInit:
                    break;
                case Arch.Core.SubStates.Result:
                    break;
                case Arch.Core.SubStates.PostFinished:
                    break;*/
            }
        }

        public void ExamineAndLoadGame(StateController _stateController)
        {
        }

        /// <summary>
        /// Gets the empty parking slot
        /// </summary>
        /// <returns></returns>
        public ParkingSlot GetParkingSlot()
        {
            List<int> parkingUnitActiatedList = new List<int>();
            for (int i = 0; i < parkingUnitList.Length; ++i)
            {
                if (parkingUnitList[i].gameObject.activeSelf && parkingUnitList[i].IsParkingSlotEmpty())
                    parkingUnitActiatedList.Add(i);
            }
            if (parkingUnitActiatedList.Count <= 0)
            {
                if (UndergroundParking.IsParkingUnlocked)
                {
                    return UndergroundParking.ParkingSlot;
                }
                else
                {
                    Debug.LogError("Parking Unit is empty!!!");
                    return null;
                }
            }

            int randomParkingUnit = Random.Range(0, parkingUnitActiatedList.Count);
            foreach (ParkingSlot slot in parkingUnitList[parkingUnitActiatedList[randomParkingUnit]].ParkingSlots)
            {
                if (slot.isUnlocked)
                {
                    if (slot.CarBase == null)
                        return slot;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if Parking slot is empty
        /// </summary>
        /// <returns></returns>
        public bool IsParkingSlotEmpty()
        {
            bool isEmpty = false;
            foreach (ParkingUnit parkingUnit in parkingUnitList)
            {
                isEmpty = parkingUnit.IsParkingSlotEmpty();
                if (isEmpty)
                {
                    break;
                }
            }
            if (!isEmpty && UndergroundParking.IsParkingUnlocked)
            {
                return UndergroundParking.IsParkingSlotEmpty();
            }
            return isEmpty;
        }

        /// <summary>
        /// Checks if parking unit is avaible to unlock
        /// </summary>
        /// <returns></returns>
        public bool IsParkingUnitAvailableToUnlock()
        {
            bool isAvailable = false;

            for (int i = 0; i < parkingUnitList.Length; i++)
            {
                isAvailable = !parkingUnitList[i].gameObject.activeSelf;
                if (isAvailable)
                {
                    break;
                }
            }
            return isAvailable;
        }

        /// <summary>
        /// Get count of parking slots
        /// </summary>
        /// <returns></returns>
        public int GetCountOfUnlockedParkingSlots()
        {
            int count = 0;
            for (int i = 0; i < parkingUnitList.Length; i++)
            {
                count += parkingUnitList[i].GetCountOfUnlocledSlots();
            }

            if (UndergroundParking.IsParkingUnlocked)
                count += UndergroundParking.SlotCount;

            return count;
        }

        /// <summary>
        /// Get count of unlocked and empty slots
        /// </summary>
        /// <returns></returns>
        public int GetCountOfEmptyParkingSlots()
        {
            int count = 0;
            for (int i = 0; i < parkingUnitList.Length; i++)
            {
                count += parkingUnitList[i].GetFreeSlotsCount();
            }

            if (UndergroundParking.IsParkingUnlocked)
                count += UndergroundParking.GetFreeParkingSlotsCount();

            return count;
        }

        #region FTUE
        public bool FTUEIsAllCarsInParkingState(int maxCount = 10)
        {
            bool isInParkingState = true;

            for (int i = 0; i < parkingUnitList.Length; ++i)
            {
                if (parkingUnitList[i].GetCountOfUnlocledSlots() != 0 && !parkingUnitList[i].IsCarInParkingState(maxCount))
                {
                    isInParkingState = false;
                    break;
                }
            }

            return isInParkingState;
        }
        #endregion

        /// <summary>
        /// Unlocks new parking slot, 
        /// unlockNewUnit : if true: Unlocks new parking unit if current parking unit all slots are already unlocked
        /// </summary>
        public void UnlockNewParkingSlot(bool unlockNewUnit = false)
        {
            if (UnlockGroundedParking())
            {}
            refStateController.ParkingManager.WaypointPathFinder.UpdateWaypointPosition(currParkingRvealIndex);
            if (ParkingManager.OnParkingUnitUpdated != null)
                ParkingManager.OnParkingUnitUpdated();
        }

        /// <summary>
        /// Unlocks the ground parking unit and returns the result
        /// </summary>
        /// <returns></returns>
        bool UnlockGroundedParking()
        {
            if (currParkingRvealIndex != ParkingRevealIndex.UNDERGROUND_PARKING)
            {
                currParkingRvealIndex++;
                refStateController.ParkingManager.AnalyzeAndRevealParking(currParkingRvealIndex);
                UnlockParkingUnit();

                if (currParkingRvealIndex == ParkingRevealIndex.UNDERGROUND_PARKING)
                {
                    UndergroundParking.UnlockTheParking(refStateController.ParkingManager.ParkingJson.UnlockParkingPerUpgrade);
                    Management.Services.AnalyticsManager.Instance.CustomEvent(Management.Services.EventStrings.UNDERGROUND_PARKING_UNLOCK);
                }
                else
                {
                    Management.Services.AnalyticsManager.Instance.CustomEvent(Management.Services.EventStrings.PARKING_UNLOCK);
                }

                SaveParkingUnitData();

                if (CbOnParkingCountUpdate != null)
                    CbOnParkingCountUpdate(GetCountOfUnlockedParkingSlots());
                return true;
            }
            else
            {
                if (undergroundParking.IsParkingUnlocked && !undergroundParking.IsMaxSlotsUnlocked())
                {
                    undergroundParking.UnlockNewSlots();
                    SaveParkingUnitData();
                    if (CbOnParkingCountUpdate != null)
                        CbOnParkingCountUpdate(GetCountOfUnlockedParkingSlots());
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Unlocks the new parking unit
        /// </summary>
        public void UnlockParkingUnit()
        {
            for (int i = 0; i < parkingObjectScalerArr.Length; ++i)
            {
                parkingObjectScalerArr[i].UpdateTransform(currParkingRvealIndex);
            }

            for (int i = 0; i < parkingUnitList.Length; ++i)
            {
                parkingUnitList[i].RevealParking(currParkingRvealIndex);
            }
        }

        void SaveParkingUnitData()
        {
            refStateController.GameManager.MasterLoader.ParkingUnitSaveModel.parkingRevealIndex = (int)currParkingRvealIndex;
            refStateController.GameManager.MasterLoader.ParkingUnitSaveModel.undergroundParkingSlots = undergroundParking.SlotCount;
            SaveGameData();
        }

        void ArrangeUnits(float xSize, bool isEventNumber)
        {
            int multiplyer = 1;
            int directional = 1;
            int indexer = 0;
            for (int i = 1; i <= parkingUnitList.Length; i++)
            {
                if (isEventNumber == false && i == 1)
                {
                    Vector3 pos = parkingUnitList[i - 1].transform.position;
                    pos.x = transform.position.x;
                    parkingUnitList[i - 1].transform.position = pos;
                    continue;
                }
                Vector3 position = parkingUnitList[i - 1].transform.position;
                position.x = transform.position.x;
                if(isEventNumber)
                    position.x += ((xSize * multiplyer) + (xSize * indexer)) * directional;
                else
                    position.x += (xSize * multiplyer) * directional;
                parkingUnitList[i - 1].transform.position = position;
                directional *= -1;

                if (isEventNumber == true && i % 2 == 0)
                {
                    multiplyer++;
                    indexer++;
                }
                else if (isEventNumber == false && i % 2 != 0)
                {
                    multiplyer++;
                    indexer++;
                }
            }
        }
    }
}
