using System.Collections;
using System.Collections.Generic;
using Arch.Core;
using UnityEngine;

namespace Management.Hospital
{
    public class HospitalManager : MonoBehaviour, Arch.Core.iManagerBase
    {
        [SerializeField] HospitalBuilding hospitalBuilding;
        public HospitalBuilding HospitalBuilding { get => hospitalBuilding; }

        public List<Department> GetDepartments { get => hospitalBuilding.GetUnlockedDepartments(); }

        Arch.Core.StateController stateController;
        public Arch.Core.StateController StateController { get => stateController; }

        public void Initilize(Arch.Core.StateController _stateController)
        {
            stateController = _stateController;
            hospitalBuilding.Initilize(this);
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

        private void OnEnable()
        {
            Management.Doctor.Doctor.Cb_OnDiagnosisOver += CbOnDiagnosisComplated;
        }

        private void OnDisable()
        {
            Management.Doctor.Doctor.Cb_OnDiagnosisOver -= CbOnDiagnosisComplated;
        }

        public void OnStateChange(Arch.Core.StateController _stateController)
        {
            HospitalBuilding.OnStateChange(_stateController);
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
        /// Unlocks next level of the hospital
        /// </summary>
        public void UnlockNewDepartment(Management.Hospital.Core.DepartmentType departmentType, System.Action<bool> callbackUnlockedFunc)
        {
            //int currLevel = 1;//Load from memory
            //int unlockLevel = currLevel + 1;
            //hospitalBuilding.UnlockLevel(unlockLevel);
            HospitalBuilding.UnlockDepartment(departmentType, callbackUnlockedFunc);
        }

        /// <summary>
        /// Returns true is the department unlocked
        /// </summary>
        /// <param name="departmentType"></param>
        /// <returns></returns>
        public bool IsDepartmentUnlocked(Management.Hospital.Core.DepartmentType departmentType)
        {
            return GetDepartments.Find(obj => obj.DepartmentType == departmentType) != null;
        }

        /*public void AddDoctorToDeparment(Management.Hospital.Core.DepartmentType departmentType, Management.Doctor.Doctor doctor)
        {
            Department department = hospitalBuilding.Departments.Find(obj => obj.DepartmentType == departmentType);
            department.AddDoctor(doctor);
        }*/

        /// <summary>
        /// Get Doctor if available
        /// </summary>
        /// <param name="doctorType"></param>
        /// <returns></returns>
        public Management.Doctor.Doctor GetDoctor(Management.Doctor.Core.DoctorType doctorType)
        {
            Management.Hospital.Core.DepartmentType departmentType = stateController.GameManager.WhichDepartment(doctorType);

            return GetDoctor(departmentType);
        }

        /// <summary>
        /// Get Doctor if available
        /// </summary>
        /// <param name="departmentType"></param>
        /// <returns></returns>
        public Management.Doctor.Doctor GetDoctor(Management.Hospital.Core.DepartmentType departmentType)
        {
            foreach (Management.Hospital.Department department in hospitalBuilding.Departments)
            {
                if (department.DepartmentType == departmentType)
                    return department.GetDoctorIfAvailable();
            }
            return null;
        }

        /// <summary>
        /// Event Call this when doctor is free to diagnosis next patient
        /// </summary>
        /// <param name="_doctorType"></param>
        private void CbOnDiagnosisComplated(Management.Doctor.Core.DoctorType _doctorType)
        {
            HospitalBuilding.SaveGameDataWithUpdatePatientTreatmentCount(StateController.GameManager.WhichDepartment(_doctorType));
            StartDiagnosisToNextPatient(_doctorType);
            UpdateDepartmentWaitingQueue();
        }

        void StartDiagnosisToNextPatient(Management.Doctor.Core.DoctorType _doctorType)
        {
            Hospital.Core.DepartmentType departmentType = stateController.GameManager.WhichDepartment(_doctorType);
            Department department = hospitalBuilding.Departments.Find(obj => obj.DepartmentType == departmentType);
            if (department != null)
            {
                Management.Patient.PatientBase patientBase = department.GetNextPatient();
                if (patientBase != null)
                    patientBase.DocotorIsFreeNow();
            }
            else
            {
                Debug.LogError("department is null");
                //Management.Errors.GameErrorHandler.LogError<Management.Hospital.HospitalManager>(this, "department is null");
            }
        }


        void UpdateDepartmentWaitingQueue()
        {
            Management.Patient.PatientBase patientBase = hospitalBuilding.WaitingLobby.GetPatient();
            if (patientBase == null)
                return;
            patientBase.GotowardsDoctor();
        }

        /// <summary>
        /// If can stand to the queue then returns the non Vector3.zero value
        /// </summary>
        /// <param name="_patientBase"></param>
        /// <returns></returns>
        public QueueHelper.AddQueueResult StandQueueInPharmacy(Management.Patient.PatientBase _patientBase)
        {
            return StandInQueue(_patientBase, hospitalBuilding.PharmacyDepartments);
        }

        /// <summary>
        /// If can stand to the queue then returns the non Vector3.zero value
        /// </summary>
        /// <param name="_patientBase"></param>
        /// <returns></returns>
        public QueueHelper.AddQueueResult StandQueueInConsultationFees(Management.Patient.PatientBase _patientBase)
        {
            //return StandInQueue(_patientBase, hospitalBuilding.GetLowestQueueConsultationDepartment());
            return StandInQueue(_patientBase, hospitalBuilding.ConsultationDepartments);
        }

        QueueHelper.AddQueueResult StandInQueue(Management.Patient.PatientBase _patientBase, PaymentDepartment _department)
        {
            if (_department.CanAddToWaitQueue())
            {
                return _department.AddToQueue(_patientBase);
            }

            return null;
        }

        #region PRELOAD_GAME
        public void PreloadsDoctorWithPatients(Arch.Core.StateController _stateController, ref Dictionary<string, object> dictonary)
        {
            List<Management.Patient.PatientBase> patientList = (List<Management.Patient.PatientBase>)dictonary["PatientList"];
            List<Management.Patient.PatientBase> patientListCopy = new List<Patient.PatientBase>(patientList);
            List<Management.Patient.PatientBase> usedPatientList = (List<Management.Patient.PatientBase>)dictonary["UsedPatientList"];

            for (int i = 0; i < patientListCopy.Count; ++i)
            {
                if (!_stateController.GameManager.IsProbabilityTrue(SaveLoad.GameProbabilityType.PATIENT_AT_DOCTOR))
                    continue;
                Department department = hospitalBuilding.GetDepartment(patientListCopy[i].DoctorType);
                Vector3 grabbingPosition = department.GetCheckupGrabbedPosition(patientListCopy[i]);
                if (grabbingPosition != Vector3.zero)
                {
                    Management.Patient.PatientBase patient = patientListCopy[i];
                    patientList.Remove(patient);

                    patient.transform.position = grabbingPosition;
                    patient.GotowardsDoctor(true);
                    usedPatientList.Add(patient);
                }
            }

            dictonary["PatientList"] = patientList;
            dictonary["UsedPatientList"] = usedPatientList;
        }

        public void PreloadsInTheWaitingLobby(Arch.Core.StateController _stateController, ref Dictionary<string, object> dictonary)
        {
            List<Management.Patient.PatientBase> patientList = (List<Management.Patient.PatientBase>)dictonary["PatientList"];
            List<Management.Patient.PatientBase> patientListCopy = new List<Patient.PatientBase>(patientList);
            List<Management.Patient.PatientBase> usedPatientList = (List<Management.Patient.PatientBase>)dictonary["UsedPatientList"];

            for (int i = 0; i < patientListCopy.Count; ++i)
            {
                if (!_stateController.GameManager.IsProbabilityTrue(SaveLoad.GameProbabilityType.PATIENT_AT_WAITING_LOBBY))
                    continue;
                Management.Patient.PatientBase patient = patientListCopy[i];
                Dictionary<string, object> waitingLobbyPosDict = HospitalBuilding.WaitingLobby.AddIntoWaitingQueue(patient);
                if (waitingLobbyPosDict == null || waitingLobbyPosDict.Count == 0)
                    break;

                //Add the value of consulation fees here
                patientList.Remove(patient);

                patient.PreloadGameWithWaitingLobby(waitingLobbyPosDict);

                usedPatientList.Add(patient);
            }

            dictonary["PatientList"] = patientList;
            dictonary["UsedPatientList"] = usedPatientList;
        }

        public void PreloadsInConsulationFeesQueue(Arch.Core.StateController _stateController, ref Dictionary<string, object> dictonary)
        {
            List<Management.Patient.PatientBase> patientList = (List<Management.Patient.PatientBase>)dictonary["PatientList"];
            List<Management.Patient.PatientBase> patientListCopy = new List<Patient.PatientBase>(patientList);
            List<Management.Patient.PatientBase> usedPatientList = (List<Management.Patient.PatientBase>)dictonary["UsedPatientList"];

            for (int i = 0; i < patientListCopy.Count; ++i)
            {
                if (!_stateController.GameManager.IsProbabilityTrue(SaveLoad.GameProbabilityType.PATIENT_AT_RECEPTION_QUEUE))
                    continue;
                Management.Patient.PatientBase patient = patientListCopy[i];
                if (patient.PreloadPayConsultationFees())
                {
                    patientList.Remove(patient);
                    usedPatientList.Add(patient);
                }
            }

            dictonary["PatientList"] = patientList;
            dictonary["UsedPatientList"] = usedPatientList;
        }

        public void PreloadsInPharmacyQueue(Arch.Core.StateController _stateController, ref Dictionary<string, object> dictonary)
        {
            List<Management.Patient.PatientBase> patientList = (List<Management.Patient.PatientBase>)dictonary["PatientList"];
            List<Management.Patient.PatientBase> patientListCopy = new List<Patient.PatientBase>(patientList);
            List<Management.Patient.PatientBase> usedPatientList = (List<Management.Patient.PatientBase>)dictonary["UsedPatientList"];

            for (int i = 0; i < patientListCopy.Count; ++i)
            {
                if (!_stateController.GameManager.IsProbabilityTrue(SaveLoad.GameProbabilityType.PATIENT_AT_RECEPTION_QUEUE))
                    continue;
                Management.Patient.PatientBase patient = patientListCopy[i];
                patient.PreloadProceedTowardsPharmacyPayment();
                patientList.Remove(patient);
                usedPatientList.Add(patient);
            }

            dictonary["PatientList"] = patientList;
            dictonary["UsedPatientList"] = usedPatientList;
        }

        public void PreloadInspectAndReturnHome(Arch.Core.StateController _stateController, ref Dictionary<string, object> dictonary)
        {
            List<Management.Patient.PatientBase> patientList = (List<Management.Patient.PatientBase>)dictonary["PatientList"];
            List<Management.Patient.PatientBase> patientListCopy = new List<Patient.PatientBase>(patientList);
            List<Management.Patient.PatientBase> usedPatientList = (List<Management.Patient.PatientBase>)dictonary["UsedPatientList"];

            for (int i = 0; i < patientListCopy.Count; ++i)
            {
                Management.Patient.PatientBase patient = patientListCopy[i];
                patient.PreloadInspectionArea();
                patientList.Remove(patient);
                usedPatientList.Add(patient);
            }

            dictonary["PatientList"] = patientList;
            dictonary["UsedPatientList"] = usedPatientList;

        }
        #endregion

        #region FTUE
        public void CheckForPartTwoFTUE()
        {
            if (stateController.FTUEManager.CurrentFtueType == FTUE.FTUEType.FTUE_END)
                hospitalBuilding.ConsultationDepartments.GetComponent<ConsulationDepartment>().CheckForPartTwoFTUE();
        }
        #endregion
    }
}
