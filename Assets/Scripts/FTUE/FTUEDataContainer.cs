using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.FTUE
{
    [System.Serializable]
    public class FTUEData
    {
        public FTUEType ftueType;
        public string message;
        public Sprite sprDoctor;
        public string customData;
        public float waitTimeSec = 10;
        public Vector2 sprPosition = Vector2.zero;

        /*
         *  Custom data attributes
         *  All attributes should be seperated by ';' seperator
         *   1. HAND=x:00,y:00,z:00		                        [Hand is use for positioning the hand]
         *   2. FLIPHAND=X (x or y)                             [Flips the hand image]
         *   3. CAMERA=x:00,y:00,z:00 	                        [Positions the camera]
         *   4. CAMFOLLOW=Target:FTUE_CAR,FollowTime:value 	    [Target for the camera which need to follow]
         *      # FTUE_CAR, FTUE_PATIENT
         *      # While settting CAMFOLLOW make sure that [CAMERA] set before this attribute to calculate offset
         *   5. TIMEOUT=00                                      [FTUE timeout]
         * */
    }

    [CreateAssetMenu(menuName ="Hospital/FTUE/DataContainer")]
    public class FTUEDataContainer : ScriptableObject
    {
        [SerializeField] List<FTUEData> ftueDataList;
        [SerializeField] Sprite sprHand;

        public FTUEData GetFTUE(FTUEType ftueType)
        {
            return ftueDataList.Find(obj => obj.ftueType == ftueType);
        }
    }
}