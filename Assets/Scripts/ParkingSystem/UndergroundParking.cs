using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Parking
{
    public class UndergroundParking : MonoBehaviour
    {
        [SerializeField] Transform parkingEntryGate;
        [SerializeField] Transform parkingExitGate;
        [SerializeField] Management.Parking.ParkingSlot parkingSlot;
        public Management.Parking.ParkingSlot ParkingSlot { get => parkingSlot; }
        [SerializeField] ParkingManager parkingManager;

        List<Management.Car.CarBase> parkingCarList;
        public List<Management.Car.CarBase> ParkingCarList { get => parkingCarList; }

        public bool IsParkingUnlocked { get => gameObject.activeSelf; }

        int slotCount = 0;
        public int SlotCount { get => slotCount; }

        private void OnEnable()
        {
            if (parkingCarList == null)
                parkingCarList = new List<Car.CarBase>();
            Management.Car.CarBase.CbOnCarGoingHomeBack += CbOnCarGoingBackHome;
        }

        private void OnDestroy()
        {
            Management.Car.CarBase.CbOnCarGoingHomeBack -= CbOnCarGoingBackHome;
        }

        /// <summary>
        /// Unlocks and activate the game object
        /// </summary>
        /// <param name="isLoadGame"></param>
        public void UnlockTheParking(int count = 10,bool isLoadGame = false)
        {
            UnlockNewSlots(count);
        }

        public void UnlockNewSlots(int count = 10)
        {
            slotCount += count;
            if (slotCount >= parkingManager.ParkingJson.MaxUndergroundParkingSlots)
                slotCount = parkingManager.ParkingJson.MaxUndergroundParkingSlots;
        }

        /// <summary>
        /// Get parking entry gate 
        /// </summary>
        /// <returns></returns>
        public Vector3 GetParkingEntryGate()
        {
            return parkingEntryGate.position;
        }

        public bool IsParkingSlotEmpty()
        {
            return parkingCarList.Count < SlotCount;
        }

        /// <summary>
        /// Get count of slots which are unlocked and empty
        /// </summary>
        /// <returns></returns>
        public int GetFreeParkingSlotsCount()
        {
            return slotCount - ParkingCarList.Count;
        }

        /// <summary>
        /// Checks wether the maximum underground parking slots are unlocked
        /// </summary>
        /// <returns></returns>
        public bool IsMaxSlotsUnlocked()
        {
            return slotCount >= parkingManager.ParkingJson.MaxUndergroundParkingSlots;
        }

        /// <summary>
        /// Add car into underground parking
        /// </summary>
        /// <param name="car"></param>
        public void AddCar(Management.Car.CarBase carBase)
        {
            parkingCarList.Add(carBase);
        }

        void CbOnCarGoingBackHome(Management.Car.CarBase carBase)
        {
            parkingCarList.Remove(carBase);
        }
    }
}