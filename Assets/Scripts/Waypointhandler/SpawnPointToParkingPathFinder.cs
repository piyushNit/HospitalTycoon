using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Parking
{
    [System.Serializable]
    public class WaypointPositionUpdate
    {
        public ParkingRevealIndex parkingRevealIndex;
        public Vector2 newPosition;
    }
}

namespace Management.Pathfind
{
    public class SpawnPointToParkingPathFinder : WaypointPathFinder
    {
        [Header("Parkng Entry gates")]
        [SerializeField] Transform entryGate;
        [SerializeField] Transform pathpointTowardsEntryGate;
        [SerializeField] Transform exitGate;
        [SerializeField] Transform pathpointTowardsExitGate;

        public static System.Action<Vector3, Vector3> OnEntryGatePositionUpdated;
        public static System.Action<Vector3, Vector3> OnExitGatePositionUpdated;

        [SerializeField]Management.Parking.WaypointPositionUpdate[] newPositionsForEntryWaypoints;
        [SerializeField] Management.Parking.WaypointPositionUpdate[] newPositionsForExitWaypoints;

        /// <summary>
        /// Finds the entry gate based on the car spawned 
        /// </summary>
        /// <param name="carSpawnPostion"></param>
        /// <returns></returns>
        public Transform FindGatePosition(bool isEntry = true)
        {
            return isEntry ? entryGate : exitGate;
        }

        public void UpdateWaypointPosition(Management.Parking.ParkingRevealIndex parkingRevealIndex)
        {
            Vector3 oldPosition = entryGate.transform.position;
            for (int i = 0; i < newPositionsForEntryWaypoints.Length; ++i)
            {
                if (newPositionsForEntryWaypoints[i].parkingRevealIndex == parkingRevealIndex)
                {
                    entryGate.transform.position = new Vector3(newPositionsForEntryWaypoints[i].newPosition.x, entryGate.transform.position.y, entryGate.transform.position.z);
                    pathpointTowardsEntryGate.position = new Vector3(newPositionsForEntryWaypoints[i].newPosition.x, pathpointTowardsEntryGate.transform.position.y, pathpointTowardsEntryGate.transform.position.z);
                }
            }
            if (OnEntryGatePositionUpdated != null)
                OnEntryGatePositionUpdated(oldPosition, entryGate.position/*New Position*/);

            oldPosition = exitGate.position;
            for (int i = 0; i < newPositionsForExitWaypoints.Length; ++i)
            {
                if (newPositionsForExitWaypoints[i].parkingRevealIndex == parkingRevealIndex)
                {
                    exitGate.transform.position = new Vector3(newPositionsForExitWaypoints[i].newPosition.x, exitGate.transform.position.y, exitGate.transform.position.z);
                    pathpointTowardsExitGate.position = new Vector3(newPositionsForExitWaypoints[i].newPosition.x, pathpointTowardsExitGate.transform.position.y, pathpointTowardsExitGate.transform.position.z);
                }
            }
            if (OnExitGatePositionUpdated != null)
                OnExitGatePositionUpdated(oldPosition, exitGate.position);
        }
    }
}