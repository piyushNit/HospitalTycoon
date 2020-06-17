using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This is a single parking slot in the parking area
 */
namespace Management.Parking {
    public class ParkingSlot : MonoBehaviour {
        [SerializeField] Transform actorSpawnPoint; // Point where person will spawn
        public Transform ActorSpawnPoint{ get => actorSpawnPoint; }

        Management.Car.CarBase carBase;
        public Management.Car.CarBase CarBase { get => carBase; set => carBase = value; }
        public bool isUnlocked;
        bool isUndergroundParkingSlot;
        public bool IsUndergroundParkingSlot { get => isUndergroundParkingSlot; }

        public void UnlockSlot()
        {
            isUnlocked = true;
        }

        private void Start()
        {
            if (isUnlocked)
            {
                UnlockSlot();
            }
        }

        private void OnEnable()
        {
            isUndergroundParkingSlot = GetComponentInParent<UndergroundParking>() != null;
        }

        public List<Vector3> GetDirectionPointTowardsThisSlot(Vector3 entryGatePos, Vector3 carPosition, bool isGoingBackHome = false)
        {
            if (isUndergroundParkingSlot)
            {
                return GetDirectionTowodsSlot(carPosition.y, entryGatePos, transform.position.x);
            }
            ParkingUnit parkingUnit = GetComponentInParent<ParkingUnit>();
            return parkingUnit.GetDirectionPoints(this, entryGatePos, carPosition.y, isGoingBackHome);
        }

        /// <summary>
        /// Get list of Vector3 from gate to parking slot
        /// Grounded parking slots should call the ParkingUnit.GetDirectionTowardsSlot()method to get accurate result
        /// Only underground parking slots will call this method directly
        /// </summary>
        /// <param name="carYPos"></param>
        /// <param name="gatePos"></param>
        /// <param name="parkingSlotOffset"></param>
        /// <returns></returns>
        public List<Vector3> GetDirectionTowodsSlot(float carYPos, Vector3 gatePos, float parkingSlotOffset, bool isGoingBackHome = false)
        {
            List<Vector3> movePositions = new List<Vector3>();

            Vector3 turnTowardsParking = new Vector3(parkingSlotOffset, carYPos, gatePos.z);
            movePositions.Add(gatePos);
            movePositions.Add(turnTowardsParking);

            turnTowardsParking.z = transform.position.z;
            movePositions.Add(turnTowardsParking);

            if(!isGoingBackHome)
                movePositions.Add(transform.position);
            return movePositions;
        }

        public Vector3[] ReverseingPositionToGetOut(float backwardPosition = 3)
        {
            if (isUndergroundParkingSlot)
                return null;

            List<Vector3> positions = new List<Vector3>();
            ParkingUnit parkingUnit = GetComponentInParent<ParkingUnit>();
            Vector3 position2 = parkingUnit.GetSlotEntryOffsetPosition(this);

            positions.Add(position2);
            positions.Add(position2 + Vector3.forward * backwardPosition);

            return positions.ToArray();
        }

        public string GetParkingUnitName()
        {
            return transform.parent.name;
        }

        /// <summary>
        /// returns true if the slot is unlocked and not holding a car
        /// </summary>
        /// <returns></returns>
        public bool IsSlotFreeForParking()
        {
            if (!isUnlocked)
                return false;
            return CarBase == null;
        }

    }
}
