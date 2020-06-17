using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Hospital
{
    public abstract class StaffPosition
    {
        public Transform transform;
        public bool isPositionAvailable;
    }

    //Script only for Doctor departments
    public class Department : BaseDepartment
    {
        [Header("Doctor Department")]
        [SerializeField] DepartmentDataScriptable departmentDataScriptable;
        public bool IsUnlocked { get => gameObject.activeSelf; }

        #region PATIENT_TREATMENT_POSITIONS
        //This handles the position where Patient can interact with doctors
        //We cannot give random position its better to arrange positions beforehand

        [SerializeField] Transform positionsParentTransform;
        class DepartmentDoctor : StaffPosition
        {
            public DepartmentDoctor()
            {
                isPositionAvailable = true;
                transform = null;
            }
        }

        class PatientWaitingQueue
        {
            public Transform transform;
            public bool isAvailable;
            Management.Patient.PatientBase patientBase;
            public Management.Patient.PatientBase PatientBase { get => patientBase; }

            /// <summary>
            /// Releases the position where patient was holding
            /// </summary>
            public void ReleasePosition()
            {
                isAvailable = true;
                patientBase = null;
            }

            public void InitPatient(Management.Patient.PatientBase _patientBase)
            {
                patientBase = _patientBase;
                isAvailable = false;
                patientBase.SetPatientPositionCallback(ReleasePosition);
            }

            public PatientWaitingQueue()
            {
                isAvailable = true;
            }
        }
        [SerializeField] Transform doctorPosParentTrandform;

        PatientWaitingQueue[] patientWaitingQueueList;
        DepartmentDoctor[] arrDoctorPositions;
        #endregion

        public void UnlockDepartment()
        {
            gameObject.SetActive(true);
            departmentUpgradeJson = refStateController.DepartmentUpgradeScriptable.GetJsonModel<Management.Hospital.Json.DepartmentUpgrades>(DepartmentType);
            if (DeaprtmentUpgradeJson == null)
            {
                Debug.LogError("Game Error!! Json for department type :" + DepartmentType.ToString() + " is not set");
            }

            refStateController.HospitalManager.HospitalBuilding.Departments.Add(this);
            refStateController.GameManager.SpawnDoctor(DepartmentType, refStateController);
        }

        /// <summary>
        /// Get count of total staff
        /// </summary>
        /// <returns></returns>
        public int GetTotalStaffSlots()
        {
            return arrDoctorPositions.Length;
        }

        /// <summary>
        /// Get treatment time
        /// </summary>
        /// <returns></returns>
        public float GetTreatmentTime()
        {
            SaveLoad.HospitalDepartentSave hospitalDepartment = refStateController.GameManager.MasterLoader.HospitalDepartmentSaveModel.GetData(DepartmentType);
            if (hospitalDepartment != null)
                return GetWorkTime(hospitalDepartment.SalaryIncreaseIndexCount);
            else
                return GetWorkTime(0);//Load default time
        }

        List<Management.Doctor.Doctor> doctorList;

        /// <summary>
        /// Get percent 0% - 100%
        /// </summary>
        /// <returns></returns>
        public float GetDoctorAvailibilityPercent()
        {
            int totalDoctor = arrDoctorPositions.Length;
            int currDoctorCount = doctorList.Count;

            float per = ((float)currDoctorCount / (float)totalDoctor) * 100;
            return per;
        }

        #region CONSTRUCTORS
        public Department() { }

        public Department(Management.Doctor.Doctor _doctor)
        {
            //doctorRef = _doctor;
        }

        public Department(Management.Doctor.Doctor _doctor, Transform[] transList)
        {
            //doctorRef = _doctor;
            //departmentPositions = transList;
        }
        #endregion

        public void Awake()
        {
            patientWaitingQueueList = new PatientWaitingQueue[positionsParentTransform.childCount];
            for (int i = 0; i < patientWaitingQueueList.Length; i++)
            {
                patientWaitingQueueList[i] = new PatientWaitingQueue();
                patientWaitingQueueList[i].transform = positionsParentTransform.GetChild(i);
            }

            arrDoctorPositions = new DepartmentDoctor[doctorPosParentTrandform.childCount];
            for (int i = 0; i < arrDoctorPositions.Length; i++)
            {
                arrDoctorPositions[i] = new DepartmentDoctor();
                arrDoctorPositions[i].transform = doctorPosParentTrandform.GetChild(i);
            }
            //Management.Doctor.BaseDoctor
        }

        /// <summary>
        /// Checks the doctor slot and returns the result
        /// </summary>
        /// <returns></returns>
        public bool CanAddDoctor()
        {
            return doctorList.Count < arrDoctorPositions.Length;
        }

        public void AddDoctor(Management.Doctor.Doctor doctor)
        {
            if (doctorList == null)
                doctorList = new List<Doctor.Doctor>();
            doctorList.Add(doctor);
        }

        /// <summary>
        /// Assign saved entity to doctors
        /// </summary>
        /// <param name="doctorEntity"></param>
        /// <param name="doctorIndex"></param>
        public void SetDoctorEntityExternally(Doctor.DoctorEntity doctorEntity, int doctorIndex)
        {
            if (doctorIndex < doctorList.Count)
            {
                doctorList[doctorIndex].LoadEntityExternally(doctorEntity);
            }
        }

        /// <summary>
        /// Show UI of the correndepondence department
        /// </summary>
        public void ShowDepartmentUI()
        {

        }

        /// <summary>
        /// Grabs the position near to the doctor
        /// If return value is (0, 0, 0) then patient is unable to grab the position
        /// </summary>
        /// <param name="patientBase"></param>
        /// <returns></returns>
        public Vector3 GetCheckupGrabbedPosition(Management.Patient.PatientBase patientBase)
        {
            int count = doctorList.Count;
            for (int i = 0; i < count; i++)
            {
                if (i > count)
                    break;
                if (patientWaitingQueueList[i].isAvailable)
                {
                    patientWaitingQueueList[i].InitPatient(patientBase);

                    
                    return patientWaitingQueueList[i].transform.position;
                }
            }
            return Vector3.zero;
        }

        /// <summary>
        /// Get the path towards department in list
        /// </summary>
        /// <param name="patientBase"></param>
        /// <returns></returns>
        public List<Vector3> GetCheckupGrabbedPositionInList(Management.Patient.PatientBase patientBase)
        {
            Vector3 positionNearDoctor = GetCheckupGrabbedPosition(patientBase);
            if (positionNearDoctor == Vector3.zero)
                return null;
            List<Vector3> path = refStateController.HospitalManager.HospitalBuilding.InHospitalPathFinder.GetPath(DepartmentType);
            if (path == null)
                return null;
            path.Add(positionNearDoctor);
            return path;
        }

        /// <summary>
        /// Grabs the position of the empty slot to instantiate new doctor in the position
        /// If return value is (0, 0, 0) then there is no place for instantiate new doctor
        /// </summary>
        /// <param name="patientBase"></param>
        /// <returns></returns>
        public Transform GrabTheDoctorPosition()
        {
            for (int i = 0; i < arrDoctorPositions.Length; i++)
            {
                if (arrDoctorPositions[i].isPositionAvailable)
                {
                    arrDoctorPositions[i].isPositionAvailable = false;
                    return arrDoctorPositions[i].transform;
                }
            }
            return null;
        }

        /// <summary>
        /// Get next patient who is waiting in the queue
        /// </summary>
        /// <returns></returns>
        public Management.Patient.PatientBase GetNextPatient()
        {
            for (int i = 0; i < patientWaitingQueueList.Length; i++)
            {
                if (patientWaitingQueueList[i].isAvailable == false)
                {
                    return patientWaitingQueueList[i].PatientBase;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the doctor if they are not busy
        /// </summary>
        /// <returns></returns>
        public Management.Doctor.Doctor GetDoctorIfAvailable()
        {
            foreach (Management.Doctor.Doctor doctorElement in doctorList)
            {
                if (!doctorElement.IsDoctorBusy())
                    return doctorElement;
            }
            return null;
        }

        public override void LoadDepartment(Arch.Core.StateController stateController)
        {
            base.LoadDepartment(stateController);
        }

    }
}