using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Management.Car
{

    public class CarBase : MonoBehaviour
    {
        public enum CarState
        {
            INITLIZATION = 0,
            MOVING_TOWARDS_HOSPITAL_PARKING,
            In_Parking,
            MOVING_TOWARDS_HOME,
            INACTIVE,

            FTUE_IN_PARKING
        }

        public enum SpawnPoint
        {
            SPAWN_POINT_1 = 0,
            SPAWN_POINT_2,
            SPAWN_POINT_3,
            SPAWN_POINT_4,

            FTUE_SPAWN_POINT
        }
        [SerializeField] float carReachTime = 0.5f;
        [SerializeField] float carReturnTime = 5;

        Management.Parking.ParkingSlot parkingSlot;
        public Management.Parking.ParkingSlot ParkingSlot { get => parkingSlot; set => parkingSlot = value; }

        private SpawnPoint spawnPoint;
        public SpawnPoint Spawn_Point { get => spawnPoint; set => spawnPoint = value; }

        private bool isParkedUndeground;
        public bool IsParkedUnderground { get => isParkedUndeground; }

        public static System.Action<CarBase> CbOnCarGoingHomeBack;

        Vector3[] positions;

        CarState carState;
        public CarState Car_State { get => carState; }
        Management.Car.CarMover carMover;
        Animator carAnim;
        const string IS_MOVING = "IsMoving";

        Arch.Core.StateController stateController;

        public virtual void Initilize(Arch.Core.StateController _controller)
        {
            stateController = _controller;
            ChangeState(CarState.INITLIZATION);
        }

        private void Awake()
        {
            carMover = GetComponent<CarMover>();
            carAnim = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            Management.Parking.ParkingManager.OnParkingUnitUpdated += OnParkingUnitUpdated;
        }

        private void OnDisable()
        {
            Management.Parking.ParkingManager.OnParkingUnitUpdated -= OnParkingUnitUpdated;
        }

        #if UNITY_EDITOR
        [ContextMenu("Print Log")]
        public void PrintLog()
        {
            string logStr = "";
            logStr += "Name: " + transform.name + " | ";
            logStr += "State: " + carState.ToString() + " | ";
            logStr += "Spawn Point: " + spawnPoint.ToString() + " | ";
            logStr += "Is_Grounded: " + isParkedUndeground.ToString() + " | ";
            logStr += "Parking Slot: " + (parkingSlot == null ? "NULL" : parkingSlot.name) + " | ";
            List<Tween> tweens = DOTween.TweensByTarget(transform);
            logStr += "Tween Running: " + (tweens == null ? "0" : "" + tweens.Count);
            Debug.Log(logStr);
        }
        #endif

        void ChangeState(CarState _carState, object data = null)
        {
            carState = _carState;
            switch (carState)
            {
                case CarState.INITLIZATION:
                    if (!gameObject.activeSelf)
                        gameObject.SetActive(true);
                    break;
                case CarState.MOVING_TOWARDS_HOSPITAL_PARKING:
                    carAnim.SetBool(IS_MOVING, true);
                    TowardsHospital();
                    break;
                case CarState.In_Parking:
                    carAnim.SetBool(IS_MOVING, false);
                    if (isParkedUndeground)
                    {
                        gameObject.SetActive(false);
                    }
                    bool isPreload = data != null ? (bool)data : false;
                    if (!isPreload)
                    {
                        stateController.GameManager.SpawnPatient(this, stateController);
                    }
                    

                    if (stateController.FTUEManager.IsFTUERunning
                    && stateController.FTUEManager.CurrentFtueType == FTUE.FTUEType.FTUE_ENT_UI_CLOSE_CLICK
                    && stateController.ParkingManager.ParkingSlotHandler.FTUEIsAllCarsInParkingState(9)
                    && stateController.GameManager.MasterLoader.PlayerScoreModel.IsEnoughCash(
                       stateController.UiHolder.GetUI<UI.Ui_Parking>(UI.UiType.Parking)
                       .GetParkingUnlockCost(stateController.ParkingManager.ParkingSlotHandler.GetCountOfUnlockedParkingSlots())))
                    {
                        stateController.FTUEManager.ShowFTUE(FTUE.FTUEType.FTUE_PARING_FULL);
                    }
                    break;
                case CarState.MOVING_TOWARDS_HOME:
                    carAnim.SetBool(IS_MOVING, true);
                    GofromTheHospital();
                    break;
                case CarState.INACTIVE:
                    carAnim.SetBool(IS_MOVING, false);
                    ResetVars();
                    break;
                case CarState.FTUE_IN_PARKING:
                    stateController.FTUEManager.SkipToNext(this);
                    break;
            }
        }

        void ResetVars()
        {
            stateController = null;
            parkingSlot = null;
            positions = null;
            pathIndex = 0;
            transform.DOKill();
            gameObject.SetActive(false);
        }

        public void SetCarIntoParking(bool isPreloadGame)
        {
            ChangeState(CarState.In_Parking, isPreloadGame);
            //pathIndex = positions.Length - 1;
        }

        public void SetParkingInUnderground(bool isUnderground)
        {
            isParkedUndeground = isUnderground;
        }

        void OnParkingUnitUpdated()
        {
            if (carMover.IsMoving && Car_State == CarState.MOVING_TOWARDS_HOSPITAL_PARKING)
            {
                Vector3[] newPositions = stateController.GameManager.RecalculateCarPath(this, parkingSlot);
                carMover.UpdateWaypoints(newPositions);
            }
            else if(Car_State == CarState.MOVING_TOWARDS_HOME && !carMover.IsTakingReverse)
            {
                CalculatePositionsToGoBack();
                carMover.UpdateWaypoints(positions);
            }
        }

        public void AddMovablePositions(Vector3[] _positions)
        {
            positions = _positions;
        }

        public virtual void MoveCarToDestination(Vector3[] _positions)
        {
            positions = _positions;
            ChangeState(CarState.MOVING_TOWARDS_HOSPITAL_PARKING);
        }

        public virtual void MoveCarToDestination()
        {
            if (positions == null || positions.Length == 0)
                return;
            ChangeState(CarState.MOVING_TOWARDS_HOSPITAL_PARKING);
        }

        int pathIndex = 0;
        void TowardsHospital()
        {
            pathIndex = 0;
            MoveTowards(isGoingTowardsParkingSlot:true, completeCallback: OnCarReachedDesignation);
        }

        void MoveTowards(bool isReverse = false, bool isGoingTowardsParkingSlot = false, System.Action completeCallback = null)
        {
            transform.DOKill();
            carMover.MoveTowardsDestination(positions, isGoingTowardsParkingSlot ? parkingSlot.transform : null, completeCallback);
        }

        public virtual void UpdateMovingPath(Vector3[] _positions)
        {
            positions = _positions;
            if (carState == CarState.In_Parking)
            {
                transform.DOComplete(true);
                transform.position = positions[pathIndex];
                return;
            }
            TowardsHospital();
        }

        /// <summary>
        /// Gets call when car is reached to the parking position
        /// </summary>
        protected virtual void OnCarReachedDesignation()
        {
            pathIndex = 0;
            ChangeState(stateController.FTUEManager.IsFTUERunning
                && stateController.FTUEManager.CurrentFtueType == FTUE.FTUEType.FTUE_PATIENT_CAR_INCOMING ? CarState.FTUE_IN_PARKING : CarState.In_Parking);
        }

        /// <summary>
        /// Patient treatment is over now go back to its destination
        /// </summary>
        public void CarReturnToExit()
        {
            ChangeState(CarState.MOVING_TOWARDS_HOME);
        }

        DG.Tweening.Plugins.Core.PathCore.Path path;
        void GofromTheHospital()
        {
            if (CbOnCarGoingHomeBack != null)
                CbOnCarGoingHomeBack(this);
            if (isParkedUndeground)
            {
                gameObject.SetActive(true);
            }

            //Taking reverse from parking
            positions = parkingSlot.ReverseingPositionToGetOut();
            if (positions == null)
            {
                OnReverseTaken();
            }
            else
            {
                //MoveTowards(true, OnReverseTaken);
                carMover.TakeReverse(positions, OnReverseTaken);
            }
        }

        void OnReverseTaken()
        {
            //Going towards home now
            CalculatePositionsToGoBack();
            parkingSlot.CarBase = null;
            MoveTowards(completeCallback: () => {
                pathIndex = 0;
                ChangeState(CarState.INACTIVE);
            });
        }

        void CalculatePositionsToGoBack()
        {
            positions = null;
            List<Vector3> returnPositions = new List<Vector3>();
            Vector3[] towardsSlot = parkingSlot.GetDirectionPointTowardsThisSlot(stateController.ParkingManager.WaypointPathFinder.FindGatePosition(false).position, transform.position, true).ToArray();
            System.Array.Reverse(towardsSlot);
            returnPositions.AddRange(towardsSlot);

            Transform exitPoint = stateController.CarSpawner.GetRandomExitPoint();
            returnPositions.AddRange(stateController.ParkingManager.WaypointPathFinder.getWaypointsInVector3List(stateController.ParkingManager.WaypointPathFinder.FindGatePosition(false),exitPoint).ToArray());
            
            positions = returnPositions.ToArray();
        }

        /// <summary>
        /// Places the car at the parking slot position
        /// </summary>
        public void PlaceCarAtParkingSlotPosition()
        {
            if (parkingSlot == null)
                return;
            transform.position = parkingSlot.transform.position;
            transform.rotation = parkingSlot.transform.rotation;
        }

        /// <summary>
        /// Checks if the car is inactive or inside the car pool list
        /// </summary>
        /// <returns></returns>
        public bool IsInactive()
        {
            return carState == CarState.INACTIVE;
        }
    }
}
