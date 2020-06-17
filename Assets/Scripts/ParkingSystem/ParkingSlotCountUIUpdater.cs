using UnityEngine;
using TMPro;

namespace Management.Parking
{
    public class ParkingSlotCountUIUpdater : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI txtParkingSlot;

        private void OnEnable()
        {
            ParkingSlotHandler.CbOnParkingCountUpdate += UpdateParkingSlotCount;
        }

        private void OnDisable()
        {
            ParkingSlotHandler.CbOnParkingCountUpdate -= UpdateParkingSlotCount;
        }

        void UpdateParkingSlotCount(int count)
        {
            txtParkingSlot.text = count.ToString();
        }
    }
}