using System.Numerics;
namespace Management.SaveLoad
{
    public abstract class KEYS {
        public const string KEY_MASYER_LOADER = "KEY_MASYER_LOADER";
        public const string KEY_USER_SILENT_LOGIN = "KEY_USER_SILENT_LOGIN";
    }

    #region SUB_SAVE_MODEL_CLASSES

    public interface SaveCore
    {
        /// <summary>
        /// Converts the class data into json format
        /// </summary>
        /// <returns></returns>
        string ToJsonFormat();
    }

    [System.Serializable]
    public class PlayerScoreModel : SaveCore
    {
        private int incomePerMin { get; set; }
        public int IncomePerMin { get => incomePerMin; set => incomePerMin = value; }

        private decimal totalIncome { get; set; }
        public decimal TotalIncome { get => totalIncome; set => totalIncome = value; }

        private int diamondCount { get; set; }
        public int DiamondCount { get => diamondCount; set => diamondCount = value; }

        public int totalPatientTreated { get; set; }

        public int adCampaignAdsWatchCount { get; set; }

        public PlayerScoreModel()
        {
            incomePerMin = 0;
            totalIncome = 0;
            diamondCount = 0;
        }

        public PlayerScoreModel(PlayerScoreModel _refModel)
        {
            incomePerMin = _refModel.incomePerMin;
            totalIncome = _refModel.totalIncome;
            diamondCount = _refModel.diamondCount;
        }

        /// <summary>
        /// Updates patient treatment count
        /// </summary>
        /// <param name="count"></param>
        public void UpdatePatientTreatemtnCount(int count = 1)
        {
            totalPatientTreated += count;
        }

        public bool IsEnoughCash(double value)
        {
            return IsEnoughCash((decimal)value);
        }

        public bool IsEnoughCash(int value)
        {
            return IsEnoughCash((decimal)value);
        }

        public bool IsEnoughCash(decimal value)
        {
            #if TEST_GAME
            if(Management.GameManager.IS_INFINITE_INCOME)
                return true;
            #endif
            return value <= totalIncome;
        }

        public void DeductCash(decimal value)
        {
            #if TEST_GAME
            if(Management.GameManager.IS_INFINITE_INCOME)
                return;
            #endif
            totalIncome -= value;
        }

        public void IncreaseAdWatchCount()
        {
            adCampaignAdsWatchCount += 1;
        }

        public void ResetAdWatchCount()
        {
            adCampaignAdsWatchCount = 0;
        }

        public bool IsEnoughDiamond(int requiredDiamonds)
        {
            return requiredDiamonds <= diamondCount;
        }

        public void AddDiamonds(int diamonds)
        {
            diamondCount += diamonds;
        }

        public void DeductDiamond(int diamonds)
        {
            diamondCount -= diamonds;
            if (diamondCount <= 0)
                diamondCount = 0;
        }

        public string ToJsonFormat()
        {
            string data = "";

            data += Utils.ConvertIntoJsonElement("incomePerMin", incomePerMin.ToString());
            data += Utils.ConvertIntoJsonElement("totalIncome", totalIncome.ToString());
            data += Utils.ConvertIntoJsonElement("diamondCount", diamondCount.ToString());
            data += Utils.ConvertIntoJsonElement("totalPatientTreated", totalPatientTreated.ToString());
            data += Utils.ConvertIntoJsonElement("adCampaignAdsWatchCount", adCampaignAdsWatchCount.ToString(), false);

            return Utils.ConvertIntoJsonRootElement("PlayerScoreModel", data);
        }
    }

    [System.Serializable]
    public class PlayerGameConfig : SaveCore
    {
        bool isFTUEPlayed { get; set; }
        public bool Is_FTUE_Played { get => isFTUEPlayed; }

        public long adCampaignDateTimeStart { get; set; }

        public PlayerGameConfig()
        {
            isFTUEPlayed = false;
            adCampaignDateTimeStart = 0;
        }

        public void SetFTUEPlayed()
        {
            isFTUEPlayed = true;
        }

        public void SetAdCampaignTime(System.DateTime dateTime)
        {
            adCampaignDateTimeStart = dateTime.ToFileTimeUtc();
        }

        public System.DateTime ConvertAdCampaignToDateTime()
        {
            return System.DateTime.FromFileTimeUtc(adCampaignDateTimeStart);
        }

        public string ToJsonFormat()
        {
            string data = "";
            data += Utils.ConvertIntoJsonElement("isFTUEPlayed", isFTUEPlayed.ToString());
            data += Utils.ConvertIntoJsonElement("adCampaignDateTimeStart", adCampaignDateTimeStart.ToString(), false);

            return Utils.ConvertIntoJsonRootElement("PlayerGameConfig", data);
        }
    }

    //-------------------------------------------- Hospital Department Model
    [System.Serializable]
    public class HospitalDepartentSave : SaveCore
    {
        Hospital.Core.DepartmentType departmentType;
        public Hospital.Core.DepartmentType DepartmentType { get => departmentType; set => departmentType = value; }

        private bool isDepartmentUnlocked { get; set; }
        public bool IsDepartmentUnlocked { get => isDepartmentUnlocked; }

        private int currLevel { get; set; }
        public int CurrLevel { get => currLevel; }

        private int staffCount { get; set; }
        public int StaffCount { get => staffCount; }

        private int patientTreated { get; set; }
        public int PatientTreated { get => patientTreated; }

        private int salaryIncreaseIndexCount { get; set; }
        public int SalaryIncreaseIndexCount { get => salaryIncreaseIndexCount; }

        private System.Collections.Generic.List<Doctor.DoctorEntity> doctorEntityList;
        public System.Collections.Generic.List<Doctor.DoctorEntity> DoctorEntityList { get => doctorEntityList; }

        public void SetDepartmentUnlockd()
        {
            isDepartmentUnlocked = true;
        }
        public void UpdateCurrLevel(int updateBy = 1)
        {
            currLevel += updateBy;
        }

        /// <summary>
        /// Updates staff count to save
        /// </summary>
        /// <param name="updateCount"></param>
        public void UpdateStaffCount(int updateCount = 1)
        {
            staffCount += updateCount;
        }

        public void IncreaseSalaryIndex()
        {
            salaryIncreaseIndexCount++;
        }

        public HospitalDepartentSave(Hospital.Core.DepartmentType departmentType)
        {
            this.departmentType = departmentType;
            isDepartmentUnlocked = true;
            currLevel = 1;//Default level is always 1
            staffCount = 1;//value 1 because, we are appointing new staff after unlocking
            patientTreated = 0;
            doctorEntityList = new System.Collections.Generic.List<Doctor.DoctorEntity>();
        }

        /// <summary>
        /// Updates the doctor entity list for save
        /// </summary>
        /// <param name="doctorEntity"></param>
        /// <param name="index"></param>
        public void UpdateEntityToList(Doctor.DoctorEntity doctorEntity, int index = -1)
        {
            if (doctorEntityList == null)
                doctorEntityList = new System.Collections.Generic.List<Doctor.DoctorEntity>();
            if (index == -1)
                doctorEntityList.Add(doctorEntity);
            else
                if(index < doctorEntityList.Count - 1)
                    doctorEntityList[index] = doctorEntity;
            else
                doctorEntityList.Add(doctorEntity);
        }

        /// <summary>
        /// Update patient treated count
        /// </summary>
        /// <param name="updateBy"></param>
        public void UpdatePatientTreated(int updateBy = 1)
        {
            patientTreated += updateBy;
        }

        public string ToJsonFormat()
        {
            string data = "";
            data += Utils.ConvertIntoJsonElement("departmentName", departmentType.ToString());
            data += Utils.ConvertIntoJsonElement("isDepartmentUnlocked", isDepartmentUnlocked.ToString());
            data += Utils.ConvertIntoJsonElement("currLevel", currLevel.ToString());
            data += Utils.ConvertIntoJsonElement("staffCount", staffCount.ToString());
            data += Utils.ConvertIntoJsonElement("patientTreated", patientTreated.ToString());
            data += Utils.ConvertIntoJsonElement("salaryIncreaseIndexCount", salaryIncreaseIndexCount.ToString(), DoctorEntityList != null ? true : false);

            if (DoctorEntityList != null)
            {
                string doctorData = "";
                for (int i = 0; i < DoctorEntityList.Count; ++i)
                {
                    doctorData += Utils.ConvertIntoJsonArrayElement(doctorEntityList[i].ToJsonFormat());
                }

                data += Utils.ConvertToJsonArrayRootElement("DoctorEntity", doctorData, false);
            }

            return data;
        }
    }
    
    [System.Serializable]
    public class HospitalDepartmentSaveModel : SaveCore
    {
        System.Collections.Generic.List<HospitalDepartentSave> hospitalDepartmentData;
        public System.Collections.Generic.List<HospitalDepartentSave> HospitalDepartmentData { get => hospitalDepartmentData; }

        public HospitalDepartmentSaveModel()
        {
            hospitalDepartmentData = new System.Collections.Generic.List<HospitalDepartentSave>();
        }

        /// <summary>
        /// Check if the save list contains the data for the perticular department
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(Hospital.Core.DepartmentType key)
        {
            return hospitalDepartmentData.Find((obj)=>obj.DepartmentType == key) != null;
        }

        /// <summary>
        /// Gets the department data based
        /// </summary>
        /// <param name="_departmentType"></param>
        /// <returns></returns>
        public HospitalDepartentSave GetData(Hospital.Core.DepartmentType _departmentType)
        {
            return hospitalDepartmentData.Find((obj)=>obj.DepartmentType == _departmentType);
        }

        /// <summary>
        /// Unlocks the department into save model
        /// </summary>
        /// <param name="dType"></param>
        public void UnlockDepartment(Hospital.Core.DepartmentType dType)
        {
            if (ContainsKey(dType))
                return;
            hospitalDepartmentData.Add(new HospitalDepartentSave(dType));
        }

        /// <summary>
        /// Make sure that the department is unlocked
        /// </summary>
        /// <param name="dType"></param>
        /// <param name="departmentData"></param>
        public void UpdateDataIntoList(Hospital.Core.DepartmentType dType, HospitalDepartentSave departmentData)
        {
            for (int i = 0; i < hospitalDepartmentData.Count; i++)
            {
                if(hospitalDepartmentData[i].DepartmentType == dType)
                    hospitalDepartmentData[i] = departmentData;
            }
        }

        /// <summary>
        /// Updates the staff count of the department. Default upgrade count is 1
        /// </summary>
        /// <param name="departmentType"></param>
        /// <param name="updateStaffCount"></param>
        public void UpdateStaff(Management.Hospital.Core.DepartmentType departmentType, int updateStaffCount = 1)
        {
            for (int i = 0; i < hospitalDepartmentData.Count; i++)
            {
                if (hospitalDepartmentData[i].DepartmentType == departmentType)
                {
                    HospitalDepartentSave departmentData = hospitalDepartmentData[i];
                    departmentData.UpdateStaffCount(updateStaffCount);
                }
            }
        }

        /// <summary>
        /// Gets the current count of staff
        /// </summary>
        /// <param name="departmentType"></param>
        /// <returns></returns>
        public int GetTotalStaffCount(Management.Hospital.Core.DepartmentType departmentType)
        {
            return hospitalDepartmentData.Find(obj => obj.DepartmentType == departmentType).StaffCount;
        }

        /// <summary>
        /// Gets the current salary index of the department staff
        /// </summary>
        /// <param name="departmentType"></param>
        /// <returns></returns>
        public int GetSalaryIndex(Management.Hospital.Core.DepartmentType departmentType)
        {
            return hospitalDepartmentData.Find(obj => obj.DepartmentType == departmentType).SalaryIncreaseIndexCount;
        }

        /// <summary>
        /// Saves the patients treated by department
        /// </summary>
        /// <param name="departmentType"></param>
        /// <param name="updateBy"></param>
        public void UpdatePatientTreatment(Management.Hospital.Core.DepartmentType departmentType, int updateBy = 1)
        {
            for (int i = 0; i < hospitalDepartmentData.Count; i++)
            {
                if (hospitalDepartmentData[i].DepartmentType == departmentType)
                {
                    HospitalDepartentSave departmentData = hospitalDepartmentData[i];
                    departmentData.UpdatePatientTreated(updateBy);
                }
            }
        }

        /// <summary>
        /// Saves the entity data to the department_type
        /// </summary>
        /// <param name="departmentType"></param>
        /// <param name="doctorEntityList"></param>
        public void UpdateDoctorEntityUpgrade(Management.Hospital.Core.DepartmentType departmentType, System.Collections.Generic.List<Doctor.DoctorEntity> doctorEntityList)
        {
            HospitalDepartentSave departmentData = null;
            for (int i = 0; i < hospitalDepartmentData.Count; i++)
            {
                if (hospitalDepartmentData[i].DepartmentType == departmentType)
                {
                    departmentData = hospitalDepartmentData[i];
                }
            }

            for (int i = 0; i < doctorEntityList.Count && departmentData != null; ++i)
            {
                departmentData.UpdateEntityToList(doctorEntityList[i], i);
            }
        }

        public string ToJsonFormat()
        {
            string data = "";
            for (int index = 0; index < hospitalDepartmentData.Count; ++index)
            {
                data += Utils.ConvertIntoJsonArrayElement(
                    hospitalDepartmentData[index].ToJsonFormat(),
                    index < hospitalDepartmentData.Count - 1
                    );
                index++;
            }

            data = Utils.ConvertToJsonArrayRootElement("hospitalDepartmentData", data, false);

            return Utils.ConvertIntoJsonRootElement("HospitalDepartmentSaveModel", data);
        }
    }

    //-------------------------------------------- Parking Save model
    //[System.Serializable]
    //public class ParkingUnitSave
    //{
    //    public bool isUnlocked;
    //    public int unlockedSlotCount;
    //    public ParkingUnitSave()
    //    {
    //        isUnlocked = false;
    //        unlockedSlotCount = 0;
    //    }

    //}
    [System.Serializable]
    public class ParkingUnitSaveModel : SaveCore
    {
        public int parkingRevealIndex;
        public int undergroundParkingSlots;
        public int patientsPerMin;
        public int GetPatientPerMinWithoutOffset { get => patientsPerMin - 5; }//removing default value

        public ParkingUnitSaveModel()
        {
            parkingRevealIndex = 0;
            undergroundParkingSlots = 0;

            patientsPerMin = 5;//We are keeping patients per min is 5
        }

        public void IncreasePatientsPerMinBy(int increaseBy = 1)
        {
            patientsPerMin += increaseBy;
        }

        public string ToJsonFormat()
        {
            string data = "";

            data += Utils.ConvertIntoJsonElement("parkingRevealIndex", parkingRevealIndex.ToString());
            data += Utils.ConvertIntoJsonElement("undergroundParkingSlots", undergroundParkingSlots.ToString());
            data += Utils.ConvertIntoJsonElement("patientsPerMin", patientsPerMin.ToString(), false);

            return Utils.ConvertIntoJsonRootElement("ParkingUnitSaveModel", data);
        }
    }

    //-------------------------------------------- Waiting lobby Save model
    [System.Serializable]
    public class WaitingLobySaveModel : SaveCore
    {
        public int numberOfSeatsUnlocked = 1;//default always 1 lobby is unlocked

        public void UnlockNew()
        {
            numberOfSeatsUnlocked++;
        }

        public string ToJsonFormat()
        {
            string data = "";
            data += Utils.ConvertIntoJsonElement("numberOfSeatsUnlocked", numberOfSeatsUnlocked.ToString(), false);

            return Utils.ConvertIntoJsonRootElement("WaitingLobySaveModel", data, false);
        }
    }

    #endregion

    [System.Serializable]
    public class MasterLoader : SaveCore
    {
        private PlayerScoreModel playerScoreModel;
        public PlayerScoreModel PlayerScoreModel{ get => playerScoreModel; set => playerScoreModel = value; }

        private PlayerGameConfig playerGameConfig;
        public PlayerGameConfig PlayerGameConfig { get => playerGameConfig; set => playerGameConfig = value; }

        private HospitalDepartmentSaveModel hospitalDepartmentSaveModel;
        public HospitalDepartmentSaveModel HospitalDepartmentSaveModel { get => hospitalDepartmentSaveModel; set => hospitalDepartmentSaveModel = value; }

        private ParkingUnitSaveModel parkingUnitSaveModel;
        public ParkingUnitSaveModel ParkingUnitSaveModel { get => parkingUnitSaveModel; set => parkingUnitSaveModel = value; }

        private WaitingLobySaveModel waitingLobbySaveModel;
        public WaitingLobySaveModel WaitingLobbySaveModel { get => waitingLobbySaveModel; set => waitingLobbySaveModel = value; }

        public MasterLoader()
        {
            playerScoreModel = new PlayerScoreModel();
            playerGameConfig = new PlayerGameConfig();
            hospitalDepartmentSaveModel = new HospitalDepartmentSaveModel();
            parkingUnitSaveModel = new ParkingUnitSaveModel();
            waitingLobbySaveModel = new WaitingLobySaveModel();
        }

        public string ToJsonFormat()
        {
            string data = "";
            data += PlayerScoreModel.ToJsonFormat();
            data += PlayerGameConfig.ToJsonFormat();
            data += HospitalDepartmentSaveModel.ToJsonFormat();
            data += ParkingUnitSaveModel.ToJsonFormat();
            data += WaitingLobbySaveModel.ToJsonFormat();

            return "{" + data + "}";
        }

        public bool LoadFromJsonLoader(string json)
        {
            MasterLoader loader = Arch.Json.JsonReader.LoadJson<MasterLoader>(json);
            if (loader != null)
            {
                playerScoreModel = loader.PlayerScoreModel;
                playerGameConfig = loader.PlayerGameConfig;
                hospitalDepartmentSaveModel = loader.HospitalDepartmentSaveModel;
                parkingUnitSaveModel = loader.ParkingUnitSaveModel;
                waitingLobbySaveModel = loader.WaitingLobbySaveModel;
                return true;
            }
            return false;
        }
    }
}