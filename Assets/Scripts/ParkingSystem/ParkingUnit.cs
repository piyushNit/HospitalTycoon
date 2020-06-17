using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Management.Parking
{
    public enum ParkingRevealIndex
    {
        PARKING_1 = 0,
        PARKING_2,
        PARKING_3,
        UNDERGROUND_PARKING
    }

    [System.Serializable]
    public class UnlockSlotsForParking
    {
        public ParkingRevealIndex parkingRevealIndex;
        public int slotsToUnlock;
    }

    public class ParkingUnit : MonoBehaviour
    {
        List<ParkingSlot> parkingSlots;
        public List<ParkingSlot> ParkingSlots { get => parkingSlots; }
        [SerializeField] UnlockSlotsForParking[] unlockSlotsForParking;
        [SerializeField] Transform lhsSlotEntryPoint;
        [SerializeField] Transform rhsSlotEntryPoint;
        //[SerializeField] Renderer parkingMeshRender;
        //public Renderer ParkingMeshRender { get => parkingMeshRender; }

        [SerializeField] Management.Hospital.Revealer.ParkingRevealer[] parkingRevealers;

        public void Initilize()
        {
            parkingSlots = GetComponentsInChildren<ParkingSlot>().ToList();
        }

        public void RevealParking(ParkingRevealIndex parkingRevealIndex)
        {
            for (int i = 0; i < parkingRevealers.Length; ++i)
            {
                parkingRevealers[i].Reveal(parkingRevealIndex);
            }

            for (int i = 0; i < unlockSlotsForParking.Length; ++i)
            {
                if (unlockSlotsForParking[i].parkingRevealIndex == parkingRevealIndex)
                {
                    //Here, this is dynamic slots unlock. hence, cannot be used this value from json
                    UnlockParkingSlots(unlockSlotsForParking[i].slotsToUnlock);
                }
            }
        }

        /// <summary>
        /// Returns true if parking slot is unlocked and empty
        /// </summary>
        /// <returns></returns>
        public bool IsParkingSlotEmpty()
        {
            foreach (ParkingSlot slot in parkingSlots)
            {
                if (slot.isUnlocked)
                {
                    if (slot.CarBase == null)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get count of unlocked slot list
        /// </summary>
        /// <returns></returns>
        public int GetCountOfUnlocledSlots()
        {
            return parkingSlots.Count((obj) => { return obj.isUnlocked == true; });
        }

        /// <summary>
        /// Get count slots which are unlocked and not holding a car
        /// </summary>
        /// <returns></returns>
        public int GetFreeSlotsCount()
        {
            return parkingSlots.Count((obj) => { return obj.IsSlotFreeForParking() == true; });
        }

        #region FTUE
        public bool IsCarInParkingState(int maxCount = 9)
        {
            Debug.Log("Countof paring state : " + ParkingSlots.Count((obj) => obj.CarBase != null && (obj.CarBase.Car_State == Car.CarBase.CarState.In_Parking || obj.CarBase.Car_State == Car.CarBase.CarState.FTUE_IN_PARKING)));
            return ParkingSlots.Count((obj) => obj.CarBase != null && (obj.CarBase.Car_State == Car.CarBase.CarState.In_Parking || obj.CarBase.Car_State == Car.CarBase.CarState.FTUE_IN_PARKING)) >= maxCount;
        }
        #endregion

        /// <summary>
        /// returns true if Parking unit is having parking slots to unlock
        /// </summary>
        /// <returns></returns>
        public bool IsParkingSlotAvailableToUnlock()
        {
            foreach (ParkingSlot slot in parkingSlots)
            {
                if (slot.isUnlocked == false)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Unlocks the new [count] slots
        /// Default : Unlocks one parking slot
        /// </summary>
        /// <param name="slotsCount"></param>
        public void UnlockParkingSlots(int slotsCount = 1)
        {
            for (int i = 0; i < parkingSlots.Count; i++)
            {
                if (slotsCount <= 0)
                    break;
                if (parkingSlots[i].isUnlocked == false)
                {
                    slotsCount--;
                    parkingSlots[i].UnlockSlot();
                }
            }

            //Save into binary
        }

        /// <summary>
        /// Unlocks all parking slots
        /// </summary>
        public void UnlockAllSlots()
        {
            for (int i = 0; i < parkingSlots.Count; i++)
            {
                parkingSlots[i].UnlockSlot();
            }
        }

        /// <summary>
        /// parking slots which are grounded should called this method to get the  direction towards Parking slots
        /// </summary>
        /// <param name="parkingSlot"></param>
        /// <param name="gatePos"></param>
        /// <param name="carYPos"></param>
        /// <param name="parkingSlotOffset"></param>
        /// <returns></returns>
        public List<Vector3> GetDirectionPoints(ParkingSlot parkingSlot, Vector3 gatePos, float carYPos, bool isGoingBackHome = false)
        {
            return parkingSlot.GetDirectionTowodsSlot(carYPos, gatePos, GetSlotEntryOffsetPosition(parkingSlot).x, isGoingBackHome:isGoingBackHome);
        }

        /// <summary>
        /// Get the Slot entry Position
        /// </summary>
        /// <param name="parkingSlot"></param>
        /// <returns></returns>
        public Vector3 GetSlotEntryOffsetPosition(ParkingSlot parkingSlot)
        {
            int index = parkingSlots.IndexOf(parkingSlot);

            float x = index < parkingSlots.Count / 2 ? lhsSlotEntryPoint.position.x : rhsSlotEntryPoint.position.x;
            return new Vector3(x, parkingSlot.transform.position.y, parkingSlot.transform.position.z);
        }

        Vector3 GetNewDirectionPosition(Vector3 pos1, Vector3 pos2)
        {
            Vector3 newPosition = pos1;
            newPosition.z = pos2.z;
            return newPosition;
        }
    }
}