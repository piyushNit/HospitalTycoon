using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 1. Container of all car asets prefab
 */
namespace Management.Car
{
    public enum CarType
    {
        NORMAL_CAR,
        AMBULANCE
    }

    [System.Serializable]
    public class CarData
    {
        public string name;
        public CarType carType;
        public CarBase[] carPrefabs;

        public CarBase GetRandomCar()
        {
            if (carPrefabs.Length == 0)
                return null;
            CarBase carBase = carPrefabs[Random.Range(0, carPrefabs.Length)];
            return carBase;
        }
    }

    [CreateAssetMenu(menuName = "HospitalManagement/Car/Car Data Container")]
    public class CarDataContainer : ScriptableObject
    {
        public List<CarData> carDataList = new List<CarData>();

        public CarData GetCarData(CarType _type)
        {
            return carDataList.Find(obj => obj.carType == _type);
        }

        public CarBase GetRandomCar(CarType _type)
        {
            CarData carData = GetCarData(_type);
            if (carData == null)
                return null;
            return carData.GetRandomCar();
        }
    }
}