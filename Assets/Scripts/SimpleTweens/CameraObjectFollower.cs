using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.SimpleTween
{
    public class CameraObjectFollower : MonoBehaviour
    {
        [SerializeField] Transform target;
        public Transform TARGET { get => target; set => target = value; }
        [SerializeField] float followTime = 12;
        public float FOLLOWTIME { get => followTime; set => followTime = value; }

        Vector3 offset;


        public void CalculateOffset()
        {
            offset = transform.position - target.position;
        }

        private void LateUpdate()
        {
            if (target == null)
                return;
            transform.position = target.position + offset;
        }
    }
}