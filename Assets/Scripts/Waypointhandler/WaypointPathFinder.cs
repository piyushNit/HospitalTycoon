using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Management.Pathfind
{
    public class WaypointPathFinder : MonoBehaviour
    {
        [System.Serializable]
        public class Waypoint
        {
            [SerializeField] string pathName;
            [SerializeField] List<Transform> points;
            public List<Transform> Points { get => points; }

            public bool Contains(Transform node)
            {
                if (points == null)
                    return false;
                return points.Contains(node);
            }

            public List<Transform> GetRange(Transform point1, Transform point2)
            {
                /*123456789
                startIndex = 2
                endIndex = 9
                count = endIndex - startIndex

                startIndex = 9
                endIndex = 2

                count = startIndex - endIndex
                */
                int startIndex = points.IndexOf(point1);
                int endIndex = points.IndexOf(point2);
                List<Transform> newPoints = new List<Transform>(points);

                if (startIndex > endIndex)
                {
                    newPoints.Reverse();
                    int newIndex = startIndex;
                    startIndex = endIndex;
                    endIndex = newIndex;
                }

                int endCont = (endIndex - startIndex) + 1;

                return newPoints.GetRange(startIndex, endCont);
            }
        }

        [SerializeField] List<Waypoint> points;
        [SerializeField] float scatterOffset = 10;

        #if UNITY_EDITOR
        [Header("--- Unity Editor ----")]
        [SerializeField] float sphereSize = 5;
        private void OnDrawGizmos()
        {
            if (points == null)
                return;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i] == null)
                    continue;
                for (int j = 0; j < points[i].Points.Count; ++j)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(points[i].Points[j].position, sphereSize);
                    if (j == 0)
                        continue;
                    Gizmos.DrawLine(points[i].Points[j - 1].position, points[i].Points[j].position);
                }
            }
        }
        #endif

        public virtual List<Vector3> ConvertTransform(List<Transform> waypoints)
        {
            List<Vector3> positions = new List<Vector3>();
            foreach (Transform trans in waypoints)
            {
                positions.Add(trans.position);
            }
            return positions;
        }

        /// <summary>
        /// Get waypoints from starting to end
        /// </summary>
        /// <param name="startingWaypoint"></param>
        /// <param name="endWaypoint"></param>
        /// <param name="currentWaypoint"></param>
        /// <returns></returns>
        public virtual List<Transform> GetWaypoints(Transform startingWaypoint, Transform endWaypoint)
        {
            for (int i = 0; i < points.Count; ++i)
            {
                if (points[i].Contains(startingWaypoint) && points[i].Contains(endWaypoint))
                {
                    return points[i].GetRange(startingWaypoint,endWaypoint);
                }
            }
            return null;
        }

        /// <summary>
        /// Scatters the list element
        /// </summary>
        /// <param name="positionList"></param>
        public void ScatterThePositionList(ref List<Vector3> positionList)
        {
            for (int i = 0; i < positionList.Count; i++)
            {
                positionList[i] = new Vector3(Random.Range(positionList[i].x - scatterOffset, positionList[i].x + scatterOffset), 
                                                positionList[i].y, 
                                                positionList[i].z);
            }
        }

        /// <summary>
        /// Get waypoints in Vector3 list format
        /// </summary>
        /// <param name="startingWaypoint"></param>
        /// <param name="endWaypoint"></param>
        /// <returns></returns>
        public virtual List<Vector3> getWaypointsInVector3List(Transform startingWaypoint, Transform endWaypoint, bool scatter = true)
        {
            List<Transform> transformList = GetWaypoints(startingWaypoint, endWaypoint);
            List<Vector3> positionList = ConvertTransform(transformList);
            if(scatter)
                ScatterThePositionList(ref positionList);
            return positionList;
        }
    }
}