using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Hospital
{
    public class WaitingLobby : MonoBehaviour
    {
        [System.Serializable]
        public class WaitingSeat
        {
            Transform t_seat;
            public Transform T_seat { get => t_seat; }

            Management.Patient.PatientBase patientBase;
            public Management.Patient.PatientBase PatientBase { get => patientBase; set => patientBase = value; }

            public WaitingSeat(Transform seatTrans)
            {
                t_seat = seatTrans;
                t_seat.gameObject.SetActive(false);
                patientBase = null;
            }

            public WaitingSeat(Transform seatTrans, Management.Patient.PatientBase _patientBase)
            {
                t_seat = seatTrans;
                t_seat.gameObject.SetActive(false);
                patientBase = null;
                InitPatient(_patientBase);
            }

            public void InitPatient(Management.Patient.PatientBase _patientBase)
            {
                patientBase = _patientBase;

            }

            /// <summary>
            /// Reseats the seat and make it available for other patients
            /// </summary>
            public void Reset()
            {
                t_seat.gameObject.SetActive(true);
            }
        }
        [SerializeField] HospitalManager refHospitalManager;
        [SerializeField] WaitingLobbySeats[] waitingLobbySeats;
        [Tooltip("This variable is using while calculating path from the patient position towards lobby seat")]
        [SerializeField] float patientSeatPathOffset = 10;
        List<Transform> seatTrans = new List<Transform>();
        Queue<WaitingSeat> waitingSeatQueue = new Queue<WaitingSeat>();

        public void LoadGameData()
        {
            UpdateSeatList();

            Management.SaveLoad.WaitingLobySaveModel waitingSaveGame = refHospitalManager.StateController.GameManager.MasterLoader.WaitingLobbySaveModel;

            //i = 1 because we have 1 lobby unlocked already
            for (int i = 1; i < waitingSaveGame.numberOfSeatsUnlocked; i++)
            {
                UnlockNewSeats(true);
            }
        }

        void UpdateSeatList()
        {
            for (int i = 0; i < waitingLobbySeats.Length; i++)
            {
                if (waitingLobbySeats[i].IsUnlocked)
                {
                    foreach (Transform seatTransform in waitingLobbySeats[i].Seats)
                    {
                        if (!seatTrans.Contains(seatTransform))
                        {
                            seatTrans.Add(seatTransform);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get count of total seats
        /// </summary>
        /// <returns></returns>
        public int GetTotalSeatsCount()
        {
            return waitingLobbySeats.Length;
        }

        /// <summary>
        /// Get count of total locked seats
        /// </summary>
        /// <returns></returns>
        public int GetUnlockedSeatCount()
        {
            int unlockedSeatCount = 0;
            for (int i = 0; i < waitingLobbySeats.Length; i++)
            {
                unlockedSeatCount += waitingLobbySeats[i].IsUnlocked ? 1 : 0;
            }
            return unlockedSeatCount;
        }

        /// <summary>
        /// Get count of total locked seats
        /// </summary>
        /// <returns></returns>
        public int GetLockedSeatCount()
        {
            int lockedSeatCount = 0;
            for (int i = 0; i < waitingLobbySeats.Length; i++)
            {
                lockedSeatCount += waitingLobbySeats[i].IsUnlocked ? 0 : 1;
            }
            return lockedSeatCount;
        }

        /// <summary>
        /// Unlocks new seat pairs
        /// </summary>
        public void UnlockNewSeats(bool isLoadGame = false)
        {
            for (int i = 0; i < waitingLobbySeats.Length; i++)
            {
                if (!waitingLobbySeats[i].IsUnlocked)
                {
                    waitingLobbySeats[i].UnlockSeat();
                    break;
                }
            }
            UpdateSeatList();
            if (!isLoadGame)
            {
                refHospitalManager.StateController.GameManager.MasterLoader.WaitingLobbySaveModel.UnlockNew();
            }
        }

        /// <summary>
        /// Checks if seat available
        /// </summary>
        /// <returns></returns>
        public bool IsWaitingSeatAvailable()
        {
            if (waitingSeatQueue == null)
                return false;
            return waitingSeatQueue.Count < seatTrans.Count;
        }

        /// <summary>
        /// Add into waiting list and returns the seat position
        /// Is return position is Vector.Zero then patient is unable to grab the seat
        /// </summary>
        /// <param name="patientBase"></param>
        /// <returns></returns>
        public Dictionary<string, object> AddIntoWaitingQueue(Management.Patient.PatientBase patientBase)
        {
            Transform seatTransform = GetEmptySeatTrans();
            if (seatTransform == null)
                return null;

            WaitingSeat waitingSeat = new WaitingSeat(seatTransform, patientBase);
            waitingSeatQueue.Enqueue(waitingSeat);

            AnalyzeIfDocorSpeedUpRequired();

            List<Vector3> posList = new List<Vector3>();
            int index = seatTrans.FindIndex(obj => obj == seatTransform);
            float offsetSeat = (index < seatTrans.Count / 2) ? patientSeatPathOffset : -patientSeatPathOffset;

            posList.Add(patientBase.transform.position);
            posList.Add(new Vector3(seatTransform.position.x + offsetSeat, patientBase.transform.position.y, patientBase.transform.position.z));
            posList.Add(new Vector3(seatTransform.position.x + offsetSeat, patientBase.transform.position.y, seatTransform.position.z));
            posList.Add(new Vector3(seatTransform.position.x, patientBase.transform.position.y, seatTransform.position.z));

            Dictionary<string, object> waitingLobbyPositionDictonary = new Dictionary<string, object>();
            waitingLobbyPositionDictonary.Add("WAITING_LOBBY_DIRECTION", posList);
            waitingLobbyPositionDictonary.Add("SEAT_ROTATION", seatTransform.eulerAngles);

            return waitingLobbyPositionDictonary;
        }

        void AnalyzeIfDocorSpeedUpRequired()
        {
            if (seatTrans.Count / 2 < waitingSeatQueue.Count && Management.UI.UiHUD.AddBooster != null)
            {
                    Management.UI.UiHUD.AddBooster(BoosterType.Doctor_Time_Booster);
            }
        }

        Transform GetEmptySeatTrans()
        {
            for (int i = 0; i < seatTrans.Count; i++)
            {
                if (seatTrans[i].gameObject.activeSelf)
                    return seatTrans[i];
            }
            return null;
        }

        /// <summary>
        /// Gets the first patient and resets its list
        /// </summary>
        /// <returns></returns>
        public Management.Patient.PatientBase GetPatient()
        {
            if (waitingSeatQueue.Count <= 0)
                return null;
            WaitingSeat waitingSeat = waitingSeatQueue.Dequeue();
            waitingSeat.Reset();
            return waitingSeat.PatientBase;
        }
    }
}