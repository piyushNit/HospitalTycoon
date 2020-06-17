using System.Collections;
using System.Collections.Generic;
using Arch.Core;
using UnityEngine;

/*
 * 1. handles Parking system
 * 2. Saves into binary and load data from binary
*/
namespace Management.Parking
{
    public class ParkingManager : MonoBehaviour, Arch.Core.iManagerBase
    {
        [SerializeField] ParkingSlotHandler parkingSlotHandler;
        public ParkingSlotHandler ParkingSlotHandler { get => parkingSlotHandler; }
        public UndergroundParking UndergroundParking { get => parkingSlotHandler.UndergroundParking; }
        [SerializeField] Management.Pathfind.SpawnPointToParkingPathFinder waypointPathFinder;
        public Management.Pathfind.SpawnPointToParkingPathFinder WaypointPathFinder { get => waypointPathFinder; }
        [SerializeField] CarSpawner carSpawner;
        [SerializeField] Management.Hospital.Revealer.ParkingRevealer[] parkingRevealerArr;

        Management.Hospital.Json.ParkingJsonModel parkingJsonModel = null;
        public Management.Hospital.Json.ParkingJsonModel ParkingJson { get => parkingJsonModel; }

        private StateController refStateController;
        public StateController RefStateController { get => refStateController; }

        public static System.Action OnParkingUnitUpdated;

        public void Initilize(Arch.Core.StateController _stateController)
        {
            refStateController = _stateController;
            parkingSlotHandler.Initilize(_stateController);
        }

        public void LoadGameData()
        {
            parkingJsonModel = Arch.Json.JsonReader.LoadJson<Management.Hospital.Json.ParkingJsonModel>
                               (refStateController.HospitalJsonDataScriptable.ParkingJson.text);
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

            parkingSlotHandler.OnStateChange(_stateController);
        }

        public void SetCarToSlot(ParkingSlot slot, Car.CarBase carBase)
        {
            slot.CarBase = carBase;
        }

        public List<Vector3> GetDirectionTowoardsParking(Management.Car.CarBase.SpawnPoint spawnPoint, bool isEntryGate = true)
        {
            Transform spawnPointTrans = carSpawner.GetTransformBySpawnPoint(spawnPoint);
            Transform gate = waypointPathFinder.FindGatePosition(isEntryGate);

            List<Vector3> points = null;
            if(isEntryGate)
                points = waypointPathFinder.getWaypointsInVector3List(spawnPointTrans, gate);
            else
                points = waypointPathFinder.getWaypointsInVector3List(gate, spawnPointTrans);

            return points;
        }

        #region PRELOAD_GAME
        /// <summary>
        /// This would help to examine and preloads the cars
        /// </summary>
        /// <param name="_stateController"></param>
        /// <param name="dictonary"></param>
        public void ExamineAndLoadGame(StateController _stateController, ref Dictionary<string, object> dictonary)
        {
            int count = parkingSlotHandler.GetCountOfUnlockedParkingSlots();
            dictonary.Add("CarCount", count);

            List<Car.CarBase> carList = new List<Car.CarBase>();
            for (int i = 0; i < count; ++i)
            {
                if (!_stateController.GameManager.IsProbabilityTrue(SaveLoad.GameProbabilityType.CAR_SPAWN_PROBABILITY))
                    continue;
                Car.CarBase carbase = _stateController.GameManager.GetCarBySpawned(_stateController);
                if (carbase == null)
                    break;
                carbase.SetCarIntoParking(true);
                carbase.PlaceCarAtParkingSlotPosition();
                if (carbase != null)
                    carList.Add(carbase);
            }
            dictonary.Add("CarList", carList);
        }
        #endregion

        public void AnalyzeAndRevealParking(ParkingRevealIndex parkingRevealIndex)
        {
            for (int i = 0; i < parkingRevealerArr.Length; ++i)
            {
                parkingRevealerArr[i].Reveal(parkingRevealIndex);
            }
        }

        public void IncreasePatientPerMinAfterAdvertisement()
        {
            RefStateController.GameManager.MasterLoader.ParkingUnitSaveModel.IncreasePatientsPerMinBy();
        }
    }
}