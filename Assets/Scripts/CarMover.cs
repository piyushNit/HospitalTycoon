using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Car
{
    [RequireComponent(typeof(Car))]
    public class CarMover : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 1;
        [SerializeField] float turnSpeed = 1;
        [SerializeField] float carParkingSpeed = 1;
        [SerializeField] float carParkingTurnSpeed = 1;
        [SerializeField] float cornorDistanceCheck = 1;
        [SerializeField] float lastPositionCheck = 1;

        Vector3[] waypoints;
        Transform destination;
        int moveTowardsIndex;

        bool isTakingReverse = false;
        public bool IsTakingReverse { get => isTakingReverse; }

        bool isMoving;
        public bool IsMoving { get => isMoving; }

        private void OnEnable()
        {
            Management.Pathfind.SpawnPointToParkingPathFinder.OnEntryGatePositionUpdated += OnEntryGatePositionUpdated;
            Management.Pathfind.SpawnPointToParkingPathFinder.OnExitGatePositionUpdated += OnExitGatePositionUpdtaed;
        }

        private void OnDisable()
        {
            Management.Pathfind.SpawnPointToParkingPathFinder.OnEntryGatePositionUpdated -= OnEntryGatePositionUpdated;
            Management.Pathfind.SpawnPointToParkingPathFinder.OnExitGatePositionUpdated -= OnExitGatePositionUpdtaed;
        }

        void OnEntryGatePositionUpdated(Vector3 oldPosition, Vector3 newPosition)
        {
            if (Mathf.Round(transform.position.x) == Mathf.Round(oldPosition.x))
            {
                transform.position = new Vector3(newPosition.x, transform.position.y, transform.position.z);
            }
        }

        void OnExitGatePositionUpdtaed(Vector3 oldPosition, Vector3 newPosition)
        {
            if (GetComponent<CarBase>().Car_State != CarBase.CarState.MOVING_TOWARDS_HOME)
                return;
            if (Mathf.Round(transform.position.x) == Mathf.Round(oldPosition.x))
            {
                transform.position = new Vector3(newPosition.x, transform.position.y, transform.position.z);
            }
        }

        public void TakeReverse(Vector3[] positions, System.Action OnDestinationCallbackFunc = null)
        {
            waypoints = positions;
            moveTowardsIndex = 0;
            isTakingReverse = true;
            StartCoroutine(OnReverseUpdate(OnDestinationCallbackFunc));
        }

        public void MoveTowardsDestination(Vector3[] positions, Transform target = null, System.Action OnDestinationCallbackFunc = null)
        {
            moveTowardsIndex = 0;
            waypoints = positions;
            destination = target;
            isTakingReverse = false;
            StartCoroutine(OnUpdate(OnDestinationCallbackFunc));
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (waypoints == null || waypoints.Length == 0)
                return;
            for (int i = 1; i < waypoints.Length; i++)
            {
                Gizmos.DrawLine(waypoints[i - 1], waypoints[i]);
            }
        }
        #endif

        IEnumerator OnUpdate(System.Action OnDestinationCallbackFunc = null)
        {
            isMoving = true;
            bool isReached = false;
            bool updateLastIndex = false;
            while (!isReached)
            {
                if (updateLastIndex)
                {
                    //Parking the car in parking slot
                    transform.position = Vector3.Lerp(transform.position, destination.position, Time.deltaTime * carParkingSpeed);
                    transform.rotation = Quaternion.Slerp(transform.rotation, destination.rotation, Time.deltaTime * carParkingTurnSpeed);
                    if (Vector3.Distance(transform.position, destination.position) < lastPositionCheck
                        && Mathf.Round(transform.eulerAngles.y) == Mathf.Round(destination.eulerAngles.y))
                    {
                        transform.rotation = destination.rotation;
                        transform.position = destination.position;
                        moveTowardsIndex = 0;
                        isReached = true;
                    }
                }
                else
                {
                    //Normal movement
                    if (moveTowardsIndex >= waypoints.Length)
                        yield break;//TODO this seems bug if the position is out of range
                    transform.position += transform.forward * Time.deltaTime * moveSpeed;
                    var targetRotation = Quaternion.LookRotation(waypoints[moveTowardsIndex] - transform.position, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

                    if (Vector3.Distance(transform.position, waypoints[moveTowardsIndex]) < cornorDistanceCheck)
                    {
                        moveTowardsIndex++;
                        if (moveTowardsIndex >= waypoints.Length - 1 && destination != null)
                        {
                            updateLastIndex = true;
                        }
                        else {
                            if (moveTowardsIndex >= waypoints.Length)
                            {
                                moveTowardsIndex = 0;
                                isReached = true;
                            }
                        }
                    }
                }

                yield return null;
            }
            ResetVars();
            if (OnDestinationCallbackFunc != null)
                OnDestinationCallbackFunc();
        }

        void ResetVars()
        {
            waypoints = null;
            destination = null;
            isMoving = false;
        }

        public void UpdateWaypoints(Vector3[] newWaypoints)
        {
            waypoints = newWaypoints;
        }

        public void RepositionIfCarEnterningParkingArea(Vector3 oldEntryGatePosition, Vector3 newEntrygatePos)
        {
            if (Mathf.Round(transform.position.x) == Mathf.Round(oldEntryGatePosition.x))
            {
                transform.position = new Vector3(newEntrygatePos.x, transform.position.y, transform.position.z);
            }
        }

        IEnumerator OnReverseUpdate(System.Action OnDestinationCallbackFunc = null)
        {
            bool isReached = false;
            isMoving = true;
            while (!isReached)
            {
                transform.position += -transform.forward * Time.deltaTime * moveSpeed;
                var targetRotation = Quaternion.LookRotation(transform.position - waypoints[waypoints.Length - 1], Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                if (Vector3.Distance(transform.position, waypoints[moveTowardsIndex]) < cornorDistanceCheck)
                {
                    moveTowardsIndex++;
                    if (moveTowardsIndex >= waypoints.Length)
                    {
                        isReached = true;
                    }
                }
                yield return null;
            }
            ResetVars();
            if (OnDestinationCallbackFunc != null)
                OnDestinationCallbackFunc();
        }
    }
}