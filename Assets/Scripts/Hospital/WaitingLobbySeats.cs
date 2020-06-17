using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Hospital
{
    public class WaitingLobbySeats : MonoBehaviour
    {
        public bool IsUnlocked { get => gameObject.activeSelf; }

        [SerializeField] Transform[] seats;
        public Transform[] Seats { get => seats; }

        public void UnlockSeat()
        {
            gameObject.SetActive(true);
        }
    }
}