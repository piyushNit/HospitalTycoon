using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Management.Hospital
{
    /*
     * Seperating hospital levelwise in terms of unlocking new levels
     * This is not a game levels this is just a entire hospital seperated into small chunks:
     * The idea behind this level wise seperation is the following :
     *          1. Easy understanding to unlock which part of building need to unlock
     *          2. Easy to save level number : For loading the game just unlock all levels <= saved level
     *          3. Easy to build and increase
     */
    /*[System.Serializable]
    public class HospitalLevelsWise
    {
        public int level;
        public GameObject levelParent;

        public T GetChildObject<T>()
        {
            return levelParent.GetComponentInChildren<T>();
        }

        /// <summary>
        /// Call the base class function of load department and keep references
        /// </summary>
        public void LoadDeparment(Arch.Core.StateController stateController)
        {
            Management.Hospital.BaseDepartment[] baseDepartments = levelParent.GetComponentsInChildren<Management.Hospital.BaseDepartment>();
            for (int i = 0; i < baseDepartments.Length; i++)
            {
                baseDepartments[i].LoadDepartment(stateController);
            }
        }
    }*/

    public class HospitalBuilding : MonoBehaviour
    {
        [SerializeField] Transform entryGate;
        public Transform EntryGate { get => entryGate; }

        HospitalManager refHospitalManager;

        //[SerializeField] List<HospitalLevelsWise> hostpitalBuildingLevelList;
        [SerializeField] List<Department> departments;
        public List<Department> Departments { get => departments; }

        //TODO : for make game run I am setting values in the inspector
        // But the values must get assigned by itself later
        [SerializeField] PaymentDepartment pharmacyDepartments;
        public PaymentDepartment PharmacyDepartments { get => pharmacyDepartments; }

        //TODO : for make game run I am setting values in the inspector
        // But the values must get assigned by itself later
        [SerializeField] PaymentDepartment consulationDepartments;
        public PaymentDepartment ConsultationDepartments { get => consulationDepartments; }

        [SerializeField] AdminDepartment adminDepartment;

        [SerializeField] WaitingLobby waitingLobby;
        public WaitingLobby WaitingLobby { get => waitingLobby; }

        [SerializeField] Management.Hospital.Revealer.HospitalBuildingRevealer hospitalBuildingRevealer;
        [SerializeField] Management.Pathfind.InHospitalPathFinder inHospitalpathFinder;
        public Management.Pathfind.InHospitalPathFinder InHospitalPathFinder { get => inHospitalpathFinder; }

        public void Initilize(HospitalManager _hospitalManager)
        {
            refHospitalManager = _hospitalManager;
        }

        public void OnStateChange(Arch.Core.StateController _stateController)
        {
            switch (_stateController.CurrSubState)
            {
                case Arch.Core.SubStates.PregameInit:
                    LoadSavedData();
                    UnlockDepartment(Core.DepartmentType.ENT);

                    PharmacyDepartments.LoadDepartment(_stateController);
                    PharmacyDepartments.Initilize();
                    pharmacyDepartments.LoadUpgradeContent();

                    consulationDepartments.LoadDepartment(_stateController);
                    consulationDepartments.Initilize();
                    consulationDepartments.LoadUpgradeContent();

                    adminDepartment.LoadDepartment(_stateController);

                    LoadGameData();
                    waitingLobby.LoadGameData();
                    LoadDepartmentData();
                    break;

                    case Arch.Core.SubStates.PregameLevelSet:
                        
                    break;
                    /*case Arch.Core.SubStates.PregameUiUpdate:
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

        void LoadSavedData()
        {
        }

        void LoadDepartmentData()
        {
            SaveLoad.HospitalDepartmentSaveModel hospitalDepartmentSaveModel = refHospitalManager.StateController.GameManager.MasterLoader.HospitalDepartmentSaveModel;
            if (hospitalDepartmentSaveModel == null)
            {
                Debug.LogError("Game Error!! Cannot load department data");
                return;
            }
            for (int i = 0; i < Departments.Count; ++i)
            {
                if (Departments[i].gameObject.activeSelf && hospitalDepartmentSaveModel.ContainsKey(Departments[i].DepartmentType))
                {
                    Departments[i].LoadUpgradeContent(hospitalDepartmentSaveModel.GetData(Departments[i].DepartmentType).CurrLevel);
                    departments[i].UpgradeDepartmentAssetsWhileGameLoad();
                }
            }
        }

        /// <summary>
        /// Get a list of available departments
        /// </summary>
        /// <returns></returns>
        public List<Department> GetUnlockedDepartments()
        {
            List<Department> newUnlockedList = new List<Department>();
            for (int i = 0; i < departments.Count; i++)
            {
                if(departments[i].IsUnlocked)
                    newUnlockedList.Add(departments[i]);
            }
            return newUnlockedList;
        }

        /// <summary>
        /// reload doctor entities from saved data
        /// </summary>
        /// <param name="departmentType"></param>
        /// <param name="entityList"></param>
        public void LoadDoctorEntityFromSavedFile(Management.Hospital.Core.DepartmentType departmentType, List<Doctor.DoctorEntity> entityList)
        {
            if (entityList == null)
                return;

            Department department = departments.Find(obj => obj.DepartmentType == departmentType);
            for (int i = 0; i < entityList.Count; i++)
            {
                department.SetDoctorEntityExternally(entityList[i], i);
            }
        }

        /// <summary>
        /// Loads the references and make game obeject active
        /// </summary>
        /// <param name="departmentType"></param>
        /// <param name="callbackUnlockedFunc"></param>
        /// <param name="isLoadGame"></param>
        public void UnlockDepartment(Management.Hospital.Core.DepartmentType departmentType, System.Action<bool> callbackUnlockedFunc = null, bool isLoadGame = false)
        {
            Department department = departments.Find(obj => obj.DepartmentType == departmentType);

            if (isLoadGame && department.IsUnlocked)
                return;

            if (department == null)
            {
                Debug.LogError("Game Error!! Department not set in the departent list for unlock");
                if (callbackUnlockedFunc != null)
                    callbackUnlockedFunc(false);
                return;
            }
            department.LoadDepartment(refHospitalManager.StateController);
            department.UnlockDepartment();
            hospitalBuildingRevealer.RevealBuilding(department.DepartmentType, Departments);
            department.LoadUpgradeContent();
            if (!isLoadGame)
            {
                if (!refHospitalManager.StateController.GameManager.MasterLoader.HospitalDepartmentSaveModel.ContainsKey(departmentType))
                {
                    refHospitalManager.StateController.GameManager.MasterLoader.HospitalDepartmentSaveModel.UnlockDepartment(departmentType);
                    SaveGameData();
                }
            }
            if (callbackUnlockedFunc != null)
                callbackUnlockedFunc(true);
        }

        void UpdateSavedDataIntoDepartment(Management.SaveLoad.HospitalDepartentSave entry)
        {
            Department department = departments.Find(obj => obj.DepartmentType == entry.DepartmentType);
            if (department == null)
            {
                Debug.LogError("GAME_ERROR: Department: '" + entry.DepartmentType + "' Not found in the department list");
                return;
            }
        }

        /// <summary>
        /// Unlock the payment department
        /// </summary>
        /// <param name="departmentType"></param>
        public void UnlockPaymentDepartment(Management.Hospital.Core.DepartmentType departmentType)
        {
            if (!refHospitalManager.StateController.GameManager.MasterLoader.HospitalDepartmentSaveModel.ContainsKey(departmentType))
            {
                refHospitalManager.StateController.GameManager.MasterLoader.HospitalDepartmentSaveModel.UnlockDepartment(departmentType);
            }
        }

        /// <summary>
        /// Get hospital department if it is unlocked
        /// </summary>
        /// <param name="departmentType"></param>
        /// <returns></returns>
        public Department GetDepartment(Management.Hospital.Core.DepartmentType departmentType)
        {
            return departments.Find(obj => obj.DepartmentType == departmentType && obj.IsUnlocked);
        }

        /// <summary>
        /// Get hospital department if it is unlocked
        /// </summary>
        /// <param name="doctorType"></param>
        /// <returns></returns>
        public Department GetDepartment(Management.Doctor.Core.DoctorType doctorType)
        {
            Management.Hospital.Core.DepartmentType departmentType = refHospitalManager.StateController.GameManager.WhichDepartment(doctorType);
            return departments.Find(obj => obj.DepartmentType == departmentType && obj.IsUnlocked);
        }

        public void LoadGameData()
        {
            SaveLoad.HospitalDepartmentSaveModel hospitalSaveModel = refHospitalManager.StateController.GameManager.MasterLoader.HospitalDepartmentSaveModel;
            for (int i = 0; i < hospitalSaveModel.HospitalDepartmentData.Count; ++i)
            {
                if (hospitalSaveModel.HospitalDepartmentData[i].DepartmentType == Core.DepartmentType.CONSULTATION_FEES || hospitalSaveModel.HospitalDepartmentData[i].DepartmentType == Core.DepartmentType.PHARMASY)
                {
                    LoadSavedDataOnPaymentDepartment(hospitalSaveModel.HospitalDepartmentData[i]);
                    
                }else if (hospitalSaveModel.HospitalDepartmentData[i].IsDepartmentUnlocked)
                {
                    UnlockDepartment(hospitalSaveModel.HospitalDepartmentData[i].DepartmentType, isLoadGame: true);
                    // i=2 because when new department is unlocked one doctor spawns by default
                    // we only need to spawn if the count is greater than 1.
                    for (int j = 2; j <= hospitalSaveModel.HospitalDepartmentData[i].StaffCount; j++)
                    {
                        refHospitalManager.StateController.GameManager.SpawnDoctor(hospitalSaveModel.HospitalDepartmentData[i].DepartmentType, refHospitalManager.StateController);
                    }

                    if (hospitalSaveModel.HospitalDepartmentData[i].DoctorEntityList != null)
                    {
                        LoadDoctorEntityFromSavedFile(hospitalSaveModel.HospitalDepartmentData[i].DepartmentType, hospitalSaveModel.HospitalDepartmentData[i].DoctorEntityList);
                    }
                    UpdateSavedDataIntoDepartment(hospitalSaveModel.HospitalDepartmentData[i]);

                }
            }
        }

        void LoadSavedDataOnPaymentDepartment(Management.SaveLoad.HospitalDepartentSave entry)
        {
            Management.Hospital.Core.DepartmentType departmentType = entry.DepartmentType;

            PaymentDepartment paymentDepartment = departmentType == Core.DepartmentType.CONSULTATION_FEES ? ConsultationDepartments : PharmacyDepartments;

            paymentDepartment.LoadUpgradeContent(entry.CurrLevel);
            // i=2 because when new department is unlocked one doctor spawns by default
            // we only need to spawn if the count is greater than 1.
            for (int i = 2; i <= entry.StaffCount; i++)
            {
                if (departmentType == Core.DepartmentType.CONSULTATION_FEES)
                    refHospitalManager.StateController.GameManager.SpawnCashierInConsulationDepartment(refHospitalManager.StateController);
                else
                    refHospitalManager.StateController.GameManager.SpawnCashierInPharmacyDepartment(refHospitalManager.StateController);
            }
        }

        public void SaveGameDataWithUpdatePatientTreatmentCount(Management.Hospital.Core.DepartmentType departmentType)
        {
            refHospitalManager.StateController.GameManager.MasterLoader.HospitalDepartmentSaveModel.UpdatePatientTreatment(departmentType);
        }

        public void SaveGameDataWithStaffHiring(Management.Hospital.Core.DepartmentType departmentType)
        {
            refHospitalManager.StateController.GameManager.MasterLoader.HospitalDepartmentSaveModel.UpdateStaff(departmentType);
        }

        public void SaveGameData()
        {
        }

        private PaymentDepartment GetLowestQueue(List<PaymentDepartment> _paymentDepartment)
        {
            int lowestIndex = 0;
            int listCount = 1000;
            for (int i = 0; i < _paymentDepartment.Count; i++)
            {
                if (_paymentDepartment.Count < listCount)
                {
                    lowestIndex = i;
                    listCount = _paymentDepartment.Count;
                }
            }
            return _paymentDepartment[lowestIndex];
        }

        /// <summary>
        /// Gets the path towards the hospital main gate
        /// </summary>
        /// <param name="patientPosition"></param>
        /// <returns></returns>
        public List<Vector3> GetPathTowardsHospital(Vector3 patientPosition)
        {
            List<Vector3> path = new List<Vector3>();

            path.Add(patientPosition);//Patient Current position
            path.Add(new Vector3(patientPosition.x, patientPosition.y, entryGate.position.z));//Position towards the foothpath of the hospital
            path.Add(entryGate.position);//Entry gate of the hospital

            return path;
        }

        /// <summary>
        /// Gets the path towards the patient's car
        /// </summary>
        /// <param name="patientCarPosition"></param>
        /// <returns></returns>
        public List<Vector3> GetPathTowrdsCar(Vector3 patientCarPosition)
        {
            List<Vector3> path = new List<Vector3>();
            path.Add(entryGate.transform.position);//Entry gate of the hospital
            path.Add(new Vector3(patientCarPosition.x, patientCarPosition.y, entryGate.position.z));//Position towards the foothpath of the hospital
            path.Add(patientCarPosition);//Patient Current position

            return path;
        }
    }
}
