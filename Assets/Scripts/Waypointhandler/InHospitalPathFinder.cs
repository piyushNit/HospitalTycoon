using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Pathfind
{
    public class InHospitalPathFinder : WaypointPathFinder
    {
        [System.Serializable]
        public class DepartmentMainDoor
        {
            public Management.Hospital.Core.DepartmentType departmentType;
            public Transform mainDoorEntryPoint;
        }

        [Header("In Hospital class")]
        [SerializeField] Management.Hospital.InspectionAreaManger inspectionAreaManager;
        [SerializeField] Transform startPointNearConsulation;
        [SerializeField] Transform searchPointForExitDoor;
        [SerializeField] Transform hospitalExitGate;
        [SerializeField] List<DepartmentMainDoor> departmentMainDoorList;

        /// <summary>
        /// Gets the transform of department main door
        /// </summary>
        /// <param name="departmentType"></param>
        /// <returns></returns>
        private Transform GetDepartmentMainDoorTransform(Management.Hospital.Core.DepartmentType departmentType)
        {
            return departmentMainDoorList.Find(obj => obj.departmentType == departmentType).mainDoorEntryPoint;
        }

        /// <summary>
        /// Gets the path towards department
        /// </summary>
        /// <param name="departmentType"></param>
        /// <returns></returns>
        public List<Vector3> GetPath(Management.Hospital.Core.DepartmentType departmentType, bool scatter = true)
        {
            Transform entryDoor = GetDepartmentMainDoorTransform(departmentType);
            return getWaypointsInVector3List(startPointNearConsulation, entryDoor, scatter);
        }

        /// <summary>
        /// Gets the Path towards hospital main exit door
        /// </summary>
        /// <param name="currentNearestDepartment"></param>
        /// <returns></returns>
        public List<Vector3> GetPathTowardsExitDoor(Management.Hospital.Core.DepartmentType currentNearestDepartment)
        {
            Transform entryDoor = GetDepartmentMainDoorTransform(currentNearestDepartment);

            List<Vector3> path = getWaypointsInVector3List(entryDoor, startPointNearConsulation);
            path.AddRange(getWaypointsInVector3List(searchPointForExitDoor, hospitalExitGate));
            return path;
        }

        /// <summary>
        /// Gets the Path towards hospital main exit door
        /// </summary>
        /// <param name="currentNearestDepartment"></param>
        /// <returns></returns>
        public List<Vector3> GetPathTowardsExitDoor()
        {
            return getWaypointsInVector3List(searchPointForExitDoor, hospitalExitGate);
        }
    }
}