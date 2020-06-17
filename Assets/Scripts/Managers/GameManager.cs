using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Management.Car;
using Management.Parking;
using Arch.Core;

namespace Management.SaveLoad
{
    public enum GameProbabilityType
    {
        CAR_SPAWN_PROBABILITY = 0,
        PATIENT_AT_DOCTOR,
        PATIENT_AT_WAITING_LOBBY,
        PATIENT_AT_FARMACY_QUEUE,
        PATIENT_AT_RECEPTION_QUEUE
    }

    [System.Serializable]
    public class GameLoadProbabilityContainer
    {
        public GameProbabilityType probabilityType;
        [Range(0, 100)]
        public int percentage;
    }

    [System.Serializable]
    public class GameLoadProbability
    {
        public List<GameLoadProbabilityContainer> gameLoadProbability;
    }
}

/*
 * 1. Game Manager is the heart of the game functionlity
 * 2. State controller will call this class to perform any change in the game
 */

namespace Management
{
    public class GameManager : MonoBehaviour, Arch.Core.iManagerBase
    {
        #region INSPECTOR
        [Tooltip("Probability use while loading the game state from the player state")]
        public Management.SaveLoad.GameLoadProbability gameLoadProbability;
        #endregion
        public static readonly string GAME_VERSION = "0.0.1";//Increase every time this version so that it will be easy to write version specific code

        #region SAVE_LOAD
        private SaveLoad.MasterLoader masterLoader;
        public SaveLoad.MasterLoader MasterLoader { get => masterLoader; }

        private Arch.Save.LocalGameSaveAttempHandler localGameSaveAttemptHandler = new Arch.Save.LocalGameSaveAttempHandler();
        public Arch.Save.LocalGameSaveAttempHandler LocalGameSaveAttemptHandler { get => localGameSaveAttemptHandler; }
        #endregion

        Arch.Core.StateController stateController;

        #if TEST_GAME
        public static bool IS_INFINITE_INCOME = false;
        #endif

        #region Game_Configuration
        private bool isAdCampaignRunning;
        public bool IsAdCampaignRunning { get => isAdCampaignRunning; }
        int adCampaignMultiplyer = 2;

        private bool isDoctorTimeBoosterRunning = false;
        public bool IsDoctorTimeBoosterRunning { get => isDoctorTimeBoosterRunning; }
        private bool isCashierTimeBoosterRunning = false;
        public bool IsCashierTimeBoosterRunning { get => isCashierTimeBoosterRunning; }

        public static System.Action<Management.Hospital.BoosterType, System.TimeSpan> OnTimeBoosterUpdate;

        #endregion

        #region Object_Pooling
        private List<Management.Patient.PatientBase> patientPoolList;
        private List<Management.Car.CarBase> carPoolList;
        #endregion

        #region GOOGLE_SAVE_FILE
        GooglePlayGames.BasicApi.SavedGame.ISavedGameMetadata currentGame = null;
        #endregion

        public static System.Action OnPlayerIncomeUpdated;

        #region GAME_PLAY
        public void Initilize(Arch.Core.StateController _stateController)
        {
            //Init Vars
            patientPoolList = new List<Patient.PatientBase>();
            carPoolList = new List<CarBase>();
            stateController = _stateController;

            CheckForInternetConnection();
        }

        public static bool CheckForInternetConnection()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        private void OnEnable()
        {
            Doctor.Doctor.Cb_OnDiagnosisOver += OnDiagnosisOver;
        }

        private void OnDisable()
        {
            Doctor.Doctor.Cb_OnDiagnosisOver -= OnDiagnosisOver;

            SaveGameData();
        }

        void OnDiagnosisOver(Management.Doctor.Core.DoctorType doctorType)
        {
            MasterLoader.PlayerScoreModel.UpdatePatientTreatemtnCount();
        }

        public void OnPaymentComplate(decimal paymentAmount)
        {
            if (IsAdCampaignRunning)
                paymentAmount *= adCampaignMultiplyer;
            MasterLoader.PlayerScoreModel.TotalIncome += paymentAmount;
            stateController.UiHolder.ReloadPlayerScoreInHeaderUI();

            if (OnPlayerIncomeUpdated != null)
                OnPlayerIncomeUpdated();
        }

        public void LoadGameData()
        {
            if (Management.Services.GooglePlayServices.Instance != null && Management.Services.GooglePlayServices.Instance.IsUserAuthenticated())
            {
                StartCoroutine(ReadFromCloud());
            }
            else
            {
                masterLoader = LoadGameData<Management.SaveLoad.MasterLoader>(Management.SaveLoad.KEYS.KEY_MASYER_LOADER);
                StartCoroutine(PostLoadValidation());
            }
        }

        public void OnGameFocused(bool hasFocus)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            SaveGameData();
            #endif
        }

        IEnumerator ReadFromCloud()
        {
            Debug.Log("HOSPITAL_TYCOON: Reading from google cloud");
            bool breakLoop = false;
            int index = 0;
            float timeoutTime = 0;
            while (!breakLoop)
            {
                index++;
                Debug.Log("HOSPITAL_TYCOON: loadingGame: " + index.ToString());
                Management.Services.GooglePlayServices.Instance.ReadFromGoogleCloud((SaveLoad.MasterLoader googleMasterLoader) =>
                {
                    masterLoader = googleMasterLoader;
                    breakLoop = true;
                    Debug.Log("HOSPITAL_TYCOON: Read from google: " + (masterLoader == null ? "NULL" : "Success"));
                });
                yield return new WaitForSeconds(0.1f);
                timeoutTime += 0.1f;
                if (timeoutTime >= 30)//30 Seconds passed
                {
                    breakLoop = true;
                }
            }
            StartCoroutine(PostLoadValidation());
        }

        IEnumerator PostLoadValidation()
        {
            if (masterLoader == null)
                masterLoader = new Management.SaveLoad.MasterLoader();
            stateController.UiHolder.ReloadPlayerScoreInHeaderUI();
            yield return new WaitForEndOfFrame();
            //Change the game state to next state here
            stateController.ChangeSubState(SubStates.PregameInit);
        }

        /// <summary>
        /// Loads the data from binary using template class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keyString"></param>
        /// <returns></returns>
        T LoadGameData<T>(string keyString)
        {
            T saveData = Arch.Save.GameBinarySaveManager.LoadGame<T>(keyString);
            if (saveData == null)
            {
                return default(T);
            }
            return saveData;
        }

        public void SaveGameData()
        {
            if (Management.Services.GooglePlayServices.Instance != null && Management.Services.GooglePlayServices.Instance.IsUserAuthenticated() && masterLoader != null)
            {
                Management.Services.GooglePlayServices.Instance.WriteScore(masterLoader, (GooglePlayGames.BasicApi.SavedGame.SavedGameRequestStatus status) =>
                {
                    if (status == GooglePlayGames.BasicApi.SavedGame.SavedGameRequestStatus.Success)
                    {
                        Debug.Log("Save game success");
                    }
                    else
                    {
                        Debug.Log("Save game failed with error: " + status.ToString());
                    }
                });
            }
            LocalGameSaveAttemptHandler.ProceedToSave(MasterLoader);
        }

        public void OnStateChange(Arch.Core.StateController _stateController)
        {
            switch (_stateController.CurrSubState)
            {
                case Arch.Core.SubStates.PregameLoad:
                    LoadGameData();
                    break;
                case Arch.Core.SubStates.PregameInit:
                    break;
                case Arch.Core.SubStates.PregameLevelSet:
                    if (!_stateController.FTUEManager.IsFTUERunning)
                        ExamineAndLoadGame(_stateController);
                    break;
                case Arch.Core.SubStates.PregameUiUpdate:
                    break;
                case Arch.Core.SubStates.PregameFinished:
                    if (MasterLoader.PlayerScoreModel.TotalIncome != 0 && !stateController.FTUEManager.IsFTUERunning)
                        stateController.UiHolder.ShowWelcomeScreen();
                    if(!stateController.FTUEManager.IsFTUERunning)
                        StartCoroutine(InitiateCarSpawinning(stateController));
                    break;
                case Arch.Core.SubStates.IngameInit:
                    break;
                case SubStates.FTUE_IN_GAME_INIT:
                    break;
                case SubStates.FTUE_PART_ONE_FINISHED:
                    StartCoroutine(InitiateCarSpawinning(stateController));
                    break;
                case Arch.Core.SubStates.IngameFinished:
                    break;
                case Arch.Core.SubStates.PostInit:
                    break;
                case Arch.Core.SubStates.Result:
                    break;
                case Arch.Core.SubStates.PostFinished:
                    break;
            }
        }

        /// <summary>
        /// A Coroutine which responsible to check condition and spwan the cars
        /// </summary>
        /// <param name="_stateController"></param>
        /// <returns></returns>
        public IEnumerator InitiateCarSpawinning(Arch.Core.StateController _stateController)
        {
            while (true)
            {
                yield return new WaitForSeconds(60 / masterLoader.ParkingUnitSaveModel.patientsPerMin);
                SpawnCar(_stateController);
            }
        }

        /// <summary>
        /// Responsible to spawn and set to the parking spot
        /// </summary>
        /// <param name="_stateController"></param>
        void SpawnCar(Arch.Core.StateController _stateController, bool isFTUE = false)
        {
            if (_stateController.ParkingManager.ParkingSlotHandler.IsParkingSlotEmpty())
            {
                CarBase carbase = GetCarBySpawned(_stateController, isFTUE);
                if(carbase != null)
                    carbase.MoveCarToDestination();
            }
        }

        /// <summary>
        /// Spawns and get the car with movable positions
        /// </summary>
        /// <param name="_stateController"></param>
        /// <returns></returns>
        public CarBase GetCarBySpawned(Arch.Core.StateController _stateController, bool isFTUE = false)
        {
            CarBase carbase = _stateController.CarSpawner.SpwanCar(_stateController, GetCarFromPool(), isFTUE:isFTUE);

            ParkingSlot parkingSlot = _stateController.ParkingManager.ParkingSlotHandler.GetParkingSlot();
            if (parkingSlot == null)
                return null;
            carbase.SetParkingInUnderground(parkingSlot.IsUndergroundParkingSlot);
            if (!parkingSlot.IsUndergroundParkingSlot && parkingSlot.CarBase != null)
                return null;
            if (parkingSlot.IsUndergroundParkingSlot)
                _stateController.ParkingManager.UndergroundParking.AddCar(carbase);
            carbase.Initilize(_stateController);
            carbase.ParkingSlot = parkingSlot;
            _stateController.ParkingManager.SetCarToSlot(parkingSlot, carbase);

            List<Vector3> movePositions = new List<Vector3>();
            movePositions.AddRange(_stateController.ParkingManager.GetDirectionTowoardsParking(carbase.Spawn_Point));
            movePositions.AddRange(parkingSlot.GetDirectionPointTowardsThisSlot(/*Entry Gate*/movePositions[movePositions.Count - 1], carbase.transform.position));
            carbase.AddMovablePositions(movePositions.ToArray());

            if(!carPoolList.Contains(carbase))
                carPoolList.Add(carbase);

            return carbase;
        }

        private Management.Car.CarBase GetCarFromPool()
        {
            return carPoolList.Find(obj => obj.IsInactive());
        }

        public Vector3[] RecalculateCarPath(CarBase carbase, ParkingSlot parkingSlot)
        {
            List<Vector3> movePositions = new List<Vector3>();
            movePositions.AddRange(stateController.ParkingManager.GetDirectionTowoardsParking(carbase.Spawn_Point));
            movePositions.AddRange(parkingSlot.GetDirectionPointTowardsThisSlot(/*Entry Gate*/movePositions[movePositions.Count - 1], carbase.transform.position));
            return movePositions.ToArray();
        }

        /// <summary>
        /// Spawns patient
        /// </summary>
        /// <param name="carBase"></param>
        /// <param name="_stateController"></param>

        public void SpawnPatient(Management.Car.CarBase carBase, Arch.Core.StateController _stateController)
        {
            Patient.PatientBase patientInstance = SpawnNewPatient(carBase, _stateController);
            patientInstance.MoveTowardsHospital(_stateController.HospitalManager.HospitalBuilding.GetPathTowardsHospital(patientInstance.transform.position));
        }

        Patient.PatientBase SpawnNewPatient(Management.Car.CarBase carBase, Arch.Core.StateController _stateController)
        {
            Management.Patient.PatientDataHolder patientDataHolder = _stateController.PatientScriptable.GetPatientData(Patient.PatientType.HUMAN);
            if (patientDataHolder == null)
            {
                Debug.LogError("patient not specified in scriptable");
                return null;
            }
            Patient.PatientBase patientInstance = GetPatientFromPool();

            if (patientInstance == null)
            {
                Management.Patient.PatientBase patientPrefab = patientDataHolder.GetRandomPatient();
                if (patientPrefab == null)
                {
                    Debug.LogError("patient base is not specified in spcriptable");
                }
                patientInstance = Instantiate(patientPrefab) as Patient.PatientBase;
            }
            patientInstance.Initilize(_stateController);
            patientInstance.SetBaseCar(carBase);
            patientInstance.transform.position = carBase.ParkingSlot.ActorSpawnPoint.position;
            patientInstance.transform.rotation = carBase.ParkingSlot.ActorSpawnPoint.rotation;

            if (!patientPoolList.Contains(patientInstance))
                patientPoolList.Add(patientInstance);

            return patientInstance;
        }

        /// <summary>
        /// Get the patient base from pool
        /// </summary>
        /// <returns></returns>
        Management.Patient.PatientBase GetPatientFromPool()
        {
            return patientPoolList.Find(obj => obj.IsInactive());
        }

        public void SpawnDoctor(Management.Hospital.Core.DepartmentType departmentType, Arch.Core.StateController _stateController)
        {
            SpawnDoctor(WhichDoctor(departmentType), _stateController);
        }

        public void SpawnDoctor(Doctor.Core.DoctorType doctorType, Arch.Core.StateController _stateController)
        {
            Management.Hospital.Core.DepartmentType departmentTpe = WhichDepartment(doctorType);
            Management.Hospital.Department department = _stateController.HospitalManager.HospitalBuilding.GetDepartment(departmentTpe);

            if (department == null)
                Debug.LogError("Department not set for : " + departmentTpe.ToString());

            Transform doctorParentTrans = department.GrabTheDoctorPosition();

            Doctor.Doctor doctorPrefab = _stateController.DoctorDataScriptable.GetDoctorData(doctorType).doctorPrefab;
            Doctor.Doctor doctorInstance = Instantiate(doctorPrefab, doctorParentTrans) as Doctor.Doctor;
            doctorInstance.InitStateController(_stateController);
            doctorInstance.Initilize(department, doctorType);
            department.AddDoctor(doctorInstance);
        }

        public void SpawnCashierInConsulationDepartment(Arch.Core.StateController _stateController)
        {
            SpawnCashierInPaymentDepartment(_stateController.HospitalManager.HospitalBuilding.ConsultationDepartments, Management.Hospital.ConsulationDepartment.PaymentCBKey);
        }

        public void SpawnCashierInPharmacyDepartment(Arch.Core.StateController _stateController)
        {
            SpawnCashierInPaymentDepartment(_stateController.HospitalManager.HospitalBuilding.PharmacyDepartments, Management.Hospital.PharmacyDepartment.PaymentCBKey);
        }

        void SpawnCashierInPaymentDepartment(Management.Hospital.PaymentDepartment paymentDepartment, Management.Hospital.PaymentCallbackKey paymentKey)
        {
            Transform cashierParentTransform = paymentDepartment.GrabThePosition();
            Management.Hospital.Cashier cashier = Instantiate(paymentDepartment.CashierPrefab, cashierParentTransform);
            cashier.InitStateController(stateController);
            cashier.Initilize(paymentDepartment);
            paymentDepartment.AddCashier(cashier, (int)paymentKey);
        }

        public Management.Doctor.Core.DoctorType WhichDoctor(Management.Hospital.Core.DepartmentType _departmentType)
        {
            switch (_departmentType)
            {
                case Hospital.Core.DepartmentType.ENT:
                    return Doctor.Core.DoctorType.ENT;
                case Hospital.Core.DepartmentType.DENTIST:
                    return Doctor.Core.DoctorType.DENTIST;
                case Hospital.Core.DepartmentType.SURGEON:
                    return Doctor.Core.DoctorType.SURGEON;
                case Hospital.Core.DepartmentType.GYNAECOLOGIST:
                    return Doctor.Core.DoctorType.GYNAECOLOGIST;
                case Hospital.Core.DepartmentType.PAEDIATRICS:
                    return Doctor.Core.DoctorType.PAEDIATRICS;
                case Hospital.Core.DepartmentType.EYE_SPECILIST:
                    return Doctor.Core.DoctorType.EYE_SPECILIST;
                case Hospital.Core.DepartmentType.ORTHOPAEDICS:
                    return Doctor.Core.DoctorType.ORTHOPAEDICS;
                case Hospital.Core.DepartmentType.NEUROLOGIST:
                    return Doctor.Core.DoctorType.NEUROLOGIST;
                case Hospital.Core.DepartmentType.CARDIOLOGIST:
                    return Doctor.Core.DoctorType.CARDIOLOGIST;
                case Hospital.Core.DepartmentType.PSYCHIATRIST:
                    return Doctor.Core.DoctorType.PSYCHIATRIST;
                case Hospital.Core.DepartmentType.SKIN_SPECIALIST:
                    return Doctor.Core.DoctorType.SKIN_SPECIALIST;
                case Hospital.Core.DepartmentType.V_D:
                    return Doctor.Core.DoctorType.V_D;
                case Hospital.Core.DepartmentType.PLASTIC_SURGEON:
                    return Doctor.Core.DoctorType.PLASTIC_SURGEON;
            }

            Debug.LogError("Doctor case not present in GameManager.WhichDoctor method");
            //Management.Errors.GameErrorHandler.LogError<Management.GameManager>(this, "Doctor case not present in GameManager.WhichDoctor method");
            return Doctor.Core.DoctorType.NONE;
        }

        public Hospital.Core.DepartmentType WhichDepartment(Management.Doctor.Core.DoctorType _doctorType)
        {
            switch (_doctorType)
            {
                case Doctor.Core.DoctorType.ENT:
                    return Hospital.Core.DepartmentType.ENT;
                case Doctor.Core.DoctorType.DENTIST:
                    return Hospital.Core.DepartmentType.DENTIST;
                case Doctor.Core.DoctorType.SURGEON:
                    return Hospital.Core.DepartmentType.SURGEON;
                case Doctor.Core.DoctorType.GYNAECOLOGIST:
                    return Hospital.Core.DepartmentType.GYNAECOLOGIST;
                case Doctor.Core.DoctorType.PAEDIATRICS:
                    return Hospital.Core.DepartmentType.PAEDIATRICS;
                case Doctor.Core.DoctorType.EYE_SPECILIST:
                    return Hospital.Core.DepartmentType.EYE_SPECILIST;
                case Doctor.Core.DoctorType.ORTHOPAEDICS:
                    return Hospital.Core.DepartmentType.ORTHOPAEDICS;
                case Doctor.Core.DoctorType.NEUROLOGIST:
                    return Hospital.Core.DepartmentType.NEUROLOGIST;
                case Doctor.Core.DoctorType.CARDIOLOGIST:
                    return Hospital.Core.DepartmentType.CARDIOLOGIST;
                case Doctor.Core.DoctorType.PSYCHIATRIST:
                    return Hospital.Core.DepartmentType.PSYCHIATRIST;
                case Doctor.Core.DoctorType.SKIN_SPECIALIST:
                    return Hospital.Core.DepartmentType.SKIN_SPECIALIST;
                case Doctor.Core.DoctorType.V_D:
                    return Hospital.Core.DepartmentType.V_D;
                case Doctor.Core.DoctorType.PLASTIC_SURGEON:
                    return Hospital.Core.DepartmentType.PLASTIC_SURGEON;
            }
            Debug.LogError("Department case not set in WhichDepartment ");
            return Hospital.Core.DepartmentType.None;
        }
        #endregion
        #region Boosters
        public void ScheduleAdCampaign()
        {
            StartCoroutine(StartAdCampaign());
        }

        IEnumerator StartAdCampaign()
        {
            WaitForSeconds waitForSec = new WaitForSeconds(1);
            if (MasterLoader.PlayerGameConfig.adCampaignDateTimeStart != 0)
            {
                System.TimeSpan timeSpan = MasterLoader.PlayerGameConfig.ConvertAdCampaignToDateTime() - System.DateTime.Now;
                while (timeSpan.TotalSeconds > 0)
                {
                    isAdCampaignRunning = true;
                    yield return waitForSec;

                    timeSpan = MasterLoader.PlayerGameConfig.ConvertAdCampaignToDateTime() - System.DateTime.Now;
                }
            }
            isAdCampaignRunning = false;
        }

        public void ScheduleBooster(Hospital.BoosterType boosterType)
        {
            StartCoroutine(StartDoctorBoosterTimer(boosterType));
        }

        public bool CanAddBooster(Hospital.BoosterType boosterType)
        {
            switch (boosterType)
            {
                case Hospital.BoosterType.Doctor_Time_Booster:
                    return !isDoctorTimeBoosterRunning;
                case Hospital.BoosterType.Cashier_Time_Booster:
                    return !isCashierTimeBoosterRunning;
            }
            return false;
        }

        IEnumerator StartDoctorBoosterTimer(Hospital.BoosterType boosterType)
        {
            System.DateTime dateTime = System.DateTime.Now;
            dateTime = dateTime.AddMinutes(1);
            System.TimeSpan timeSpan = dateTime - System.DateTime.Now;

            WaitForSeconds waitSecond = new WaitForSeconds(1);

            while (timeSpan.TotalSeconds > 0)
            {
                if (OnTimeBoosterUpdate != null)
                    OnTimeBoosterUpdate(boosterType, timeSpan);
                SetBoosterFlag(boosterType, true);
                yield return waitSecond;
                timeSpan = dateTime - System.DateTime.Now;
            }
            if (OnTimeBoosterUpdate != null)
                OnTimeBoosterUpdate(boosterType, timeSpan);
            SetBoosterFlag(boosterType, false);
        }

        void SetBoosterFlag(Management.Hospital.BoosterType boosterType, bool flag)
        {
            switch (boosterType)
            {
                case Hospital.BoosterType.Doctor_Time_Booster:
                    isDoctorTimeBoosterRunning = flag;
                    break;
                case Hospital.BoosterType.Cashier_Time_Booster:
                    isCashierTimeBoosterRunning = flag;
                    break;
            }
        }

#endregion

        #region PRELOAD_GAME
        /// <summary>
        /// Examing the player current state and preloads the game
        /// This is a entry point of preloading game functionality
        /// </summary>
        /// <param name="_stateController"></param>
        public void ExamineAndLoadGame(StateController _stateController)
        {
            if (!MasterLoader.PlayerGameConfig.Is_FTUE_Played)
                return;
            Dictionary<string, object> dictonary = new Dictionary<string, object>();
            List<Management.Patient.PatientBase> usedPatientList = new List<Patient.PatientBase>();
            dictonary["UsedPatientList"] = usedPatientList;

            _stateController.ParkingManager.ExamineAndLoadGame(_stateController, ref dictonary);
            SpawnsPatientsInArray(_stateController, ref dictonary);
            _stateController.HospitalManager.PreloadsDoctorWithPatients(_stateController, ref dictonary);
            _stateController.HospitalManager.PreloadsInTheWaitingLobby(_stateController, ref dictonary);
            _stateController.HospitalManager.PreloadsInConsulationFeesQueue(_stateController, ref dictonary);
            _stateController.HospitalManager.PreloadsInPharmacyQueue(_stateController, ref dictonary);
            _stateController.HospitalManager.PreloadInspectAndReturnHome(_stateController, ref dictonary);
        }

        public void MultiplyReloadedPlayerIncome(int multiplyBy)
        {
            Debug.Log("Doubles the player income where player wasn't playing");
        }

        void SpawnsPatientsInArray(StateController _stateController, ref Dictionary<string, object> dictonary)
        {
            List<Car.CarBase> carBaseList = (List<Car.CarBase>)dictonary["CarList"];
            List<Patient.PatientBase> patientBaseList = new List<Patient.PatientBase>();
            for (int i = 0; i < carBaseList.Count; ++i)
            {
                Patient.PatientBase patientBase = SpawnNewPatient(carBaseList[i], _stateController);
                patientBaseList.Add(patientBase);
            }

            dictonary.Add("PatientList", patientBaseList);
        }

        /// <summary>
        /// Returns True/False by calculating probability with random number
        /// if probability is not set in the GameManager inspector then returns false
        /// </summary>
        /// <param name="probabilityType"></param>
        /// <returns></returns>
        public bool IsProbabilityTrue(Management.SaveLoad.GameProbabilityType probabilityType)
        {            
            Management.SaveLoad.GameLoadProbabilityContainer probabilityContainer = gameLoadProbability.gameLoadProbability.Find((obj) => obj.probabilityType == probabilityType);
            if (probabilityContainer == null)
                return false;
            int randomValue = Random.Range(0, 100);
            return randomValue <= probabilityContainer.percentage;
        }
        #endregion

        #region FTUE
        public void SpawnFTUECar()
        {
            SpawnCar(stateController, isFTUE:true);
        }

        /// <summary>
        /// While FTUE running there will be only one car in the scene
        /// </summary>
        /// <returns></returns>
        public CarBase GetFTUECar()
        {
            return carPoolList[0];
        }

        /// <summary>
        /// While FTUE running there will be only one Patient in the scene
        /// </summary>
        /// <returns></returns>
        public Patient.PatientBase GetFTUEPatient()
        {
            return patientPoolList[0];
        }
        #endregion
    }
}
