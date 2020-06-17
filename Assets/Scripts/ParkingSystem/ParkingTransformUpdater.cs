using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Parking
{
    [System.Serializable]
    public struct ParkingBaseScaler
    {
        public enum UpdateType
        {
            Scale = 0,
            Position
        }
        public UpdateType updateType;
        public ParkingRevealIndex parkingRevealIndex;
        public float value;
    }
    public class ParkingTransformUpdater : MonoBehaviour
    {
        [SerializeField] bool isActive = true;
        [SerializeField] ParkingBaseScaler[] parkingBaseArr;

        public void UpdateTransform(ParkingRevealIndex currParkingRvealIndex)
        {
            if (!isActive)
                return;
            for (int i = 0; i < parkingBaseArr.Length; ++i)
            {
                if (parkingBaseArr[i].parkingRevealIndex == currParkingRvealIndex)
                {
                    if(parkingBaseArr[i].updateType == ParkingBaseScaler.UpdateType.Scale)
                        transform.localScale = new Vector3(parkingBaseArr[i].value, transform.localScale.y, transform.localScale.z);
                    else
                        transform.localPosition = new Vector3(parkingBaseArr[i].value, transform.localPosition.y, transform.localPosition.z);
                    break;
                }
            }
        }
    }
}