using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Hospital
{
    public class InspectionArea : MonoBehaviour
    {
        public enum InspectionAreaPoint
        {
            IN_FRONT_GATE_HOSPITAL,
            BESIDE_LOBBY,
            DEPARTMENT_SEARCH_AREA
        }
        [SerializeField] InspectionAreaPoint areaPoint;
        public InspectionAreaPoint InspectinoPoint { get => areaPoint; }
        [SerializeField] Vector3 startPoint;
        [SerializeField] Vector3 endPoint;
        public Vector3 StartPointPos { get => startPoint; }
        public Vector3 EndPointPos { get => endPoint; }

        /// <summary>
        /// Get any random position between start and end point
        /// </summary>
        /// <returns></returns>
        public Vector3 GetRandomPosition()
        {
            Vector3 startP = transform.position + startPoint;
            Vector3 endP = transform.position + endPoint;

            float x = Random.Range(startP.x, endP.x);
            float y = transform.position.y;
            float z = Random.Range(startP.z, endP.z);

            return new Vector3(x, y, z);
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3 startPos = transform.position + StartPointPos;
            Vector3 endPos = transform.position + EndPointPos;
            Vector3[] verts = new Vector3[]
            {
                new Vector3(startPos.x, startPos.y, startPos.z),
                new Vector3(startPos.x, startPos.y, endPos.z),
                new Vector3(endPos.x, endPos.y, endPos.z),
                new Vector3(endPos.x, endPos.y, startPos.z)
            };

            for (int i = 0; i < verts.Length; i++)
            {
                if (i == 0)
                    Gizmos.DrawLine(verts[i], verts[verts.Length - 1]);
                else
                    Gizmos.DrawLine(verts[i - 1], verts[i]);
            }

        }
        #endif
    }
}