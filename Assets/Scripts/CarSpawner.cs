using System.Collections;
using System.Collections.Generic;
using Arch.Core;
using UnityEngine;

namespace Management
{
    public class CarSpawner : MonoBehaviour, Arch.Core.iManagerBase
    {
        [SerializeField] Transform[] spawnPoints;
        [SerializeField] Transform[] exitPoints;

        //List<ActorMover> carBaseActorMoverList = new List<ActorMover>();

        #if CAR_DEBUG
        [Header("Debug")]
        [Tooltip("This is debug purpose only")]
        [SerializeField] Management.Car.CarType carDebugType;
        #endif

        public void Initilize(Arch.Core.StateController _stateController)
        {
        }

        public void LoadGameData()
        {
        }

        public void SaveGameData()
        {
        }

        public void OnGameFocused(bool hasFocus)
        {
        }

        #if UNITY_EDITOR
        [Header("-----Unity Editor----")]
        [SerializeField] float sphereSize = 5;
        private void OnDrawGizmos()
        {
            if (spawnPoints == null)
                return;
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i] == null)
                    continue;
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(spawnPoints[i].position, sphereSize);
            }
            for (int i = 0; i < exitPoints.Length; i++)
            {
                if (exitPoints[i] == null)
                    continue;
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(exitPoints[i].position, sphereSize);
            }

        }
        #endif

        public void OnStateChange(Arch.Core.StateController _stateController)
        {
            switch (_stateController.CurrSubState)
            {
                case Arch.Core.SubStates.PregameInit:
                    LoadGameData();
                    break;
                /*case Arch.Core.SubStates.PregameLevelSet:
                    break;
                case Arch.Core.SubStates.PregameUiUpdate:
                    break;
                case Arch.Core.SubStates.PregameFinished:
                    break;
                case Arch.Core.SubStates.IngameInit:
                    break;
                case Arch.Core.SubStates.IngameFinished:
                    break;
                case Arch.Core.SubStates.PostInit:
                    break;
                case Arch.Core.SubStates.Result:
                    break;
                case Arch.Core.SubStates.PostFinished:
                    break;*/
            }
        }

        public void ExamineAndLoadGame(StateController _stateController)
        {
            
        }

        /// <summary>
        /// Get transform by the spwan point
        /// </summary>
        /// <param name="spawnPoint"></param>
        /// <returns></returns>
        public Transform GetTransformBySpawnPoint(Management.Car.CarBase.SpawnPoint spawnPoint)
        {
            return spawnPoints[(int)spawnPoint];
        }

        /// <summary>
        /// Spawns and return the CarBase model
        /// </summary>
        /// <returns></returns>
        public Management.Car.CarBase SpwanCar(Arch.Core.StateController _stateController, Management.Car.CarBase carInstanceFromPool = null, bool isFTUE = false)
        {
            #if CAR_DEBUG
            Management.Car.CarData carData = _stateController.CarDataScriptable.GetCarData(carDebugType);
            #else
            Management.Car.CarData carData = _stateController.CarDataScriptable.GetCarData(Car.CarType.NORMAL_CAR);
            #endif
            if (carData == null)
            {
                Debug.LogError("Car is not set in the scriptable");
                return null;
            }
            Management.Car.CarBase carInstance = carInstanceFromPool;
            if (carInstance == null)
            {
                Management.Car.CarBase carBase = carData.GetRandomCar();
                if (carBase == null)
                {
                    Debug.LogError("Car base is not set in the scriptable");
                    return null;
                }
                carInstance = Instantiate(carBase, transform) as Management.Car.CarBase;
            }

            Transform spawnPoint = null;
            if (isFTUE)
            {
                carInstance.Spawn_Point = Car.CarBase.SpawnPoint.FTUE_SPAWN_POINT;
                spawnPoint = spawnPoints[(int)Car.CarBase.SpawnPoint.FTUE_SPAWN_POINT];
            }
            else
            {
                int spawnPointIndex = GetRandomIndex(spawnPoints.Length);
                carInstance.Spawn_Point = (Management.Car.CarBase.SpawnPoint)spawnPointIndex;
                spawnPoint = spawnPoints[spawnPointIndex];
            }
            carInstance.transform.position = spawnPoint.position;
            carInstance.transform.rotation = spawnPoint.rotation;

            return carInstance;
        }

        int GetRandomIndex(int length)
        {
            return Random.Range(0, length);
        }

        /// <summary>
        /// Get random exit point Transform
        /// </summary>
        /// <returns></returns>
        public Transform GetRandomExitPoint()
        {
            return exitPoints[GetRandomIndex(exitPoints.Length)];
        }

        //public void AttachToUpdateList(ActorMover actorMover)
        //{
        //    carBaseActorMoverList.Add(actorMover);
        //}

        //public void DetachToUpdateList(ActorMover actorMover)
        //{
        //    carBaseActorMoverList.Remove(actorMover);
        //}

        //private void Update()
        //{
        //    foreach(ActorMover actor in carBaseActorMoverList)
        //    {
        //        if(actor != null)
        //            actor.OnUpdate();
        //    }
        //}
    }
}
