using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Management.Hospital.Core;
using System.Linq;

/*
 * 1. This is a base class of patient
 * 2. All patient classes should inherit from this
 */

namespace Management.Patient
{
    public class PatientBase : Management.Hospital.Core.BaseEntity
    {
        enum PatientState
        {
            INITILIZE = 0//this is same as get out of car
            ,GOING_HOSPIATL
            ,PAY_CONSULATION_FEES
            ,GOING_TOWARDS_DOCTOR
            ,IN_TREATMENT
            ,WAITING_FOR_DIAGNOSIS
            ,ON_TREATMENT_COMPLETE
            ,WAITING_IN_LOBBY
            ,GO_BACK_HOME
            ,INSPECTION_AREA
            ,INACTIVE //Inside pool queue
        }

        protected const string STR_ANIM_WALK_TRIGGER = "Walk";
        protected const string STR_ANIM_IDLE_TRIGGER = "Idle";
        protected const string STR_ANIM_WAVE_HAND = "HandWave";
        protected const string STR_ANIM_SITTING = "Sitting";

        [SerializeField] protected GameObject model;
        [SerializeField] float moveTime = 0.8f;
        public float MoveTime { get => moveTime; }
        [SerializeField] Animator animator;
        protected Management.Doctor.Core.DoctorType doctorType;
        public Management.Doctor.Core.DoctorType DoctorType { get => doctorType; }
        protected Management.Car.CarBase carBase;
        protected Management.Hospital.Department department;

        [Header("UI")]
        [SerializeField] PatientExpressionBox patinetExpression;
        //[SerializeField] Management.UI.UIUpdator _uiUpdator;

        #region HOSPITAL_CHECKUP
        System.Action cbOnTreatmentComplete;
        public Dictionary<Management.Hospital.PaymentCallbackKey, System.Action> CbOnPaymentCompleteDictonary = new Dictionary<Management.Hospital.PaymentCallbackKey, System.Action>();
        #endregion

        PatientState patientState;
        MoodState moodState;

        int amountNeedToPayInPharmacy = 0;
        public int AmountNeedToPayInPharmacy { get => amountNeedToPayInPharmacy; }

        public virtual void Initilize(Arch.Core.StateController _stateController)
        {
            refStateController = _stateController;
            ChangeState(PatientState.INITILIZE);
        }

        public virtual void SetBaseCar(Management.Car.CarBase _car)
        {
            carBase = _car;
        }

        #if UNITY_EDITOR
        [ContextMenu("Repeat State")]
        public void RepeatState()
        {
            ChangeState(PatientState.GO_BACK_HOME, /*From Pharmacy*/1000);
        }

        [ContextMenu("Print Log")]
        public void PrintLog()
        {
            Debug.Log(GetLog());
        }
        public string GetLog()
        {
            string logMsg = "Name :" + gameObject.name + " | ";
            logMsg += "Position : " + transform.position + " | ";
            logMsg += "Doctor Type : " + doctorType.ToString() + " | ";
            logMsg += "State : " + patientState.ToString() + " | ";
            logMsg += "Mental State :" + moodState.ToString();
            return logMsg;
        }
        #endif

        /// <summary>
        /// Move towards the Hospital Entry gate
        /// </summary>
        /// <param name="position"></param>
        public void MoveTowardsHospital(List<Vector3> positions)
        {
            ChangeState(PatientState.GOING_HOSPIATL, positions);
        }

        void ChangeState(PatientState _state, object data = null)
        {
            patientState = _state;
            switch (patientState)
            {
                case PatientState.INITILIZE:
                    if (!gameObject.activeSelf)
                        gameObject.SetActive(true);
                    break;
                case PatientState.GOING_HOSPIATL:
                    MoveTowards((List<Vector3>)data, CbOnReachedTowardsHospitalMainGate);
                    break;
                case PatientState.PAY_CONSULATION_FEES:
                    PayConsultationFees();
                    break;
                case PatientState.GOING_TOWARDS_DOCTOR:
                    {
                        bool isPreloadGame = (bool)data;
                        FindDepartment();
                        if (isPreloadGame)
                        {
                            ReachTowardsDepartment();
                        }
                        else
                        {
                            TowardsDepartment();
                        }
                    }
                    break;
                case PatientState.WAITING_FOR_DIAGNOSIS:
                    ChangeMoodState(MoodState.FRUSTRATE);
                    break;
                case PatientState.ON_TREATMENT_COMPLETE:
                    {
                        bool isPreloadGame = false;
                        if (data != null)
                            isPreloadGame = (bool)data;

                        UpdatePharmacyAmountToPay();
                        ChangeMoodState(MoodState.RELEAVED);
                        if (isPreloadGame)
                        {
                            ProceedTowardsPayment(isPreloadGame);
                        }
                        else
                        {
                            ProceedTowardsPharmacy();
                        }

                        if (refStateController.FTUEManager.IsFTUERunning && refStateController.FTUEManager.CurrentFtueType == FTUE.FTUEType.FTUE_PATIENT_CONSULTING)
                            refStateController.FTUEManager.ShowFTUE(FTUE.FTUEType.FTUE_PATIENT_TREATED);
                    }
                    break;
                case PatientState.IN_TREATMENT:
                    if (!refStateController.GameManager.IsDoctorTimeBoosterRunning)
                    {
                        animator.SetTrigger(STR_ANIM_WAVE_HAND);
                    }
                    ChangeMoodState(MoodState.SICK);

                    if (refStateController.FTUEManager.IsFTUERunning && refStateController.FTUEManager.CurrentFtueType == FTUE.FTUEType.FTUE_PATIENT_CAR_PARKING)
                        refStateController.FTUEManager.ShowFTUE(FTUE.FTUEType.FTUE_PATIENT_CONSULTING);
                    break;
                case PatientState.WAITING_IN_LOBBY:
                    {
                        ChangeMoodState(MoodState.CONFUSED);
                        Dictionary<string, object> dictonary = (Dictionary<string, object>)data;
                        List<Vector3> waitingLobbyList = (List<Vector3>)dictonary["WAITING_LOBBY_DIRECTION"];

                        if (dictonary.ContainsKey("IS_PRELOAD") && (bool)dictonary["IS_PRELOAD"] == true)
                        {
                            transform.position = waitingLobbyList[waitingLobbyList.Count - 1];
                            CbOnWaitingLobyReached((Vector3)dictonary["SEAT_ROTATION"]);
                        }
                        else {
                            MoveTowards(waitingLobbyList, ()=> {
                                CbOnWaitingLobyReached((Vector3)dictonary["SEAT_ROTATION"]);
                            });
                        }
                    }
                    break;
                case PatientState.INSPECTION_AREA:
                    {
                        bool isPreloadGame = false;
                        if (data != null)
                            isPreloadGame = (bool)data;

                        InspectTheFromHospitlGateArea(isPreloadGame);
                    }
                    break;
                case PatientState.GO_BACK_HOME:
                    GoingBackHome((int)data);
                    if (refStateController.FTUEManager.IsFTUERunning && refStateController.FTUEManager.CurrentFtueType == FTUE.FTUEType.FTUE_PATIENT_TREATED)
                    {
                        refStateController.FTUEManager.ShowFTUE(FTUE.FTUEType.FTUE_IMPROVE_ENT_CLINIC);
                    }
                    break;
                case PatientState.INACTIVE:
                    ResetVars();
                    break;
            }
        }

        void CbOnWaitingLobyReached(Vector3 seatRotation)
        {
            transform.eulerAngles = seatRotation;
            animator.SetTrigger(STR_ANIM_SITTING);
        }

        void UpdatePharmacyAmountToPay()
        {
            amountNeedToPayInPharmacy += department.DeaprtmentUpgradeJson.base_income * department.UpgradeContent.income_multiplyer;
        }

        void ResetVars()
        {
            refStateController = null;
            department = null;
            carBase = null;
            gameObject.SetActive(false);
        }

        protected override void ChangeMoodState(MoodState _moodState)
        {
            moodState = _moodState;
            //switch (moodState)
            //{
            //    case MoodState.NORMAL:
            //        break;
            //    case MoodState.RELEAVED:
            //        break;
            //    case MoodState.CONFUSED:
            //        break;
            //    case MoodState.FRUSTRATE:
            //        break;
            //    case MoodState.ANGRY:
            //        break;
            //    case MoodState.SICK:
            //        break;
            //}
            Sprite sprite = refStateController.PatientScriptable.GetExpressionSprite(_moodState);
            if(sprite != null)
                patinetExpression.ShowExpression(sprite);
        }

        void TowardsDepartment()
        {
            List<Vector3> posInDoctorCabin = department.GetCheckupGrabbedPositionInList(this);
            if (posInDoctorCabin == null)
            {
                Management.Hospital.WaitingLobby waitingLobby = refStateController.HospitalManager.HospitalBuilding.WaitingLobby;
                if (waitingLobby.IsWaitingSeatAvailable())
                {
                    Dictionary<string, object> dictonary = waitingLobby.AddIntoWaitingQueue(this);
                    if (dictonary != null)
                    {
                        ChangeState(PatientState.WAITING_IN_LOBBY, dictonary);
                    }
                    else
                    {
                        Vector3 inspectionPosition = refStateController.InspectionAreaManager.GetRandomPositionFromArea(Hospital.InspectionArea.InspectionAreaPoint.BESIDE_LOBBY);
                        MoveTowards(inspectionPosition, () => {
                            ReturnAfterInspection(Hospital.InspectionArea.InspectionAreaPoint.BESIDE_LOBBY);
                    });
                    }
                }
                else
                {
                    Vector3 inspectionPosition = refStateController.InspectionAreaManager.GetRandomPositionFromArea(Hospital.InspectionArea.InspectionAreaPoint.BESIDE_LOBBY);
                    MoveTowards(inspectionPosition, () => {
                        ReturnAfterInspection(Hospital.InspectionArea.InspectionAreaPoint.BESIDE_LOBBY);
                    });
                }
            }
            else
            {
                MoveTowards(posInDoctorCabin, ReachTowardsDepartment);
            }
        }

        void KillAnimation()
        {
            if (DOTween.IsTweening(transform))
            {
                transform.DOKill();
                animator.SetTrigger(STR_ANIM_IDLE_TRIGGER);
            }
        }

        void PayConsultationFees()
        {
            Management.Hospital.QueueHelper.AddQueueResult queueResult = refStateController.HospitalManager.StandQueueInConsultationFees(this);
            if (queueResult != null)
            {
                MoveTowards(queueResult.position, ()=> {
                    if (queueResult.funcCallback != null)
                        queueResult.funcCallback(this);
                });
            }
            else
            {
                ChangeState(PatientState.INSPECTION_AREA);
            }

            if(!refStateController.GameManager.MasterLoader.PlayerGameConfig.Is_FTUE_Played)
                refStateController.HospitalManager.CheckForPartTwoFTUE();
        }

        /// <summary>
        /// Go to the inspection area waits for sometime and then return back home
        /// </summary>
        /// <param name="isPreloadGame"></param>
        void InspectTheFromHospitlGateArea(bool isPreloadGame = false)
        {
            Vector3 inspectionPosition = refStateController.InspectionAreaManager.GetRandomPositionFromArea(Hospital.InspectionArea.InspectionAreaPoint.IN_FRONT_GATE_HOSPITAL);
            if (isPreloadGame)
            {
                transform.position = inspectionPosition;
                ReturnAfterInspection(Hospital.InspectionArea.InspectionAreaPoint.IN_FRONT_GATE_HOSPITAL);
            }
            else
            {
                MoveTowards(inspectionPosition, ()=> {
                    ReturnAfterInspection(Hospital.InspectionArea.InspectionAreaPoint.IN_FRONT_GATE_HOSPITAL);
                    });
            }
        }

        void ReturnAfterInspection(Hospital.InspectionArea.InspectionAreaPoint inspectionPoint)
        {
            StartCoroutine(ReturnAfterWait(2.00f, inspectionPoint));
        }

        IEnumerator ReturnAfterWait(float inspectionWaitTime, Hospital.InspectionArea.InspectionAreaPoint inspectionPoint)
        {
            ChangeMoodState(MoodState.CONFUSED);
            yield return new WaitForSeconds(inspectionWaitTime);
            ChangeMoodState(MoodState.ANGRY);
            ChangeState(PatientState.GO_BACK_HOME, /*From Pharmacy*/inspectionPoint);
        }

        /// <summary>
        /// Move towards any position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="callback"></param>
        public virtual void MoveTowards(Vector3 position, TweenCallback callback = null)
        {
            KillAnimation();
            animator.SetTrigger(STR_ANIM_WALK_TRIGGER);
            float time = Utils.GetTimeFromDistance(moveTime, Vector3.Distance(transform.position, position));
            transform.LookAt(position);
            transform.DOMove(position, time).OnComplete(() =>
            {
                animator.SetTrigger(STR_ANIM_IDLE_TRIGGER);
                if (callback != null)
                    callback();
            });
        }

        /// <summary>
        /// Move towards any position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="callback"></param>
        public virtual void MoveTowards(List<Vector3> positions, TweenCallback callback = null)
        {
            KillAnimation();
            animator.SetTrigger(STR_ANIM_WALK_TRIGGER);
            Vector3[] pathArray = positions.ToArray();
            DG.Tweening.Plugins.Core.PathCore.Path path = new DG.Tweening.Plugins.Core.PathCore.Path(PathType.Linear, pathArray, 2);
            float time = Management.Utils.ConvertTimeFromPathArray(pathArray, moveTime);
            transform.DOPath(path, time).SetEase(Ease.Linear).SetLookAt(0).OnComplete(() =>
            {
                animator.SetTrigger(STR_ANIM_IDLE_TRIGGER);
                if (callback != null)
                    callback();
            });
        }

        protected virtual void CbOnReachedTowardsHospitalMainGate()
        {
            ChangeState(PatientState.PAY_CONSULATION_FEES);
        }

        public void PayConsulationFees()
        {
            ChangeState(PatientState.PAY_CONSULATION_FEES);
        }

        void FindDepartment()
        {
            Management.Hospital.Core.DepartmentType departmentType = refStateController.GameManager.WhichDepartment(doctorType);
            department = refStateController.HospitalManager.HospitalBuilding.GetDepartment(departmentType);
            if (department == null)
                Debug.LogError("Department not set for : " + departmentType.ToString());
        }

        protected void ReachTowardsDepartment()
        {
            Management.Doctor.Doctor doctor = refStateController.HospitalManager.GetDoctor(doctorType);
            if (doctor == null)
            {
                ChangeState(PatientState.WAITING_FOR_DIAGNOSIS);
                return;
            }
            doctor.InitPatient(this);
            doctor.ReadyAndStartDiagonis();
            ChangeState(PatientState.IN_TREATMENT);
        }

        /// <summary>
        /// Call When doctor is available
        /// </summary>
        public virtual void DocotorIsFreeNow()
        {
            ReachTowardsDepartment();
        }

        protected override void UpdateImageAmount(float time)
        {
            base.UpdateImageAmount(time);
            //_uiUpdator.UpdateImageAmount(time);
        }

        protected override void ResetUI()
        {
            base.ResetUI();
           // _uiUpdator.ResetUI();
        }

        /// <summary>
        /// Call when complete treatment
        /// </summary>
        public void CompleteTreatment()
        {
            if (cbOnTreatmentComplete != null)
                cbOnTreatmentComplete();
            ChangeState(PatientState.ON_TREATMENT_COMPLETE);
        }

        protected virtual void ProceedTowardsPharmacy()
        {
            List<Vector3> departmentTowardsSearchPath = refStateController.HospitalManager.HospitalBuilding.InHospitalPathFinder.GetPath(department.DepartmentType, false);
            departmentTowardsSearchPath.Reverse();
            List<Vector3> searchPointToPharmacyPath = refStateController.HospitalManager.HospitalBuilding.InHospitalPathFinder.GetPath(DepartmentType.PHARMASY, false);

            //Here need to remove all duplicate items
            for (int i = 0; i < searchPointToPharmacyPath.Count; ++i)
            {
                if (departmentTowardsSearchPath.Contains(searchPointToPharmacyPath[i]))
                    departmentTowardsSearchPath.Remove(searchPointToPharmacyPath[i]);
                else
                    departmentTowardsSearchPath.Add(searchPointToPharmacyPath[i]);
            }

            refStateController.HospitalManager.HospitalBuilding.InHospitalPathFinder.ScatterThePositionList(ref departmentTowardsSearchPath);

            MoveTowards(departmentTowardsSearchPath, () => {
                ProceedTowardsPayment();
            });
        }

        /// <summary>
        /// Call when treatment is finished and now time to pay
        /// </summary>
        protected virtual void ProceedTowardsPayment(bool isPreloadGame = false)
        {
            Management.Hospital.QueueHelper.AddQueueResult queueResult = refStateController.HospitalManager.StandQueueInPharmacy(this);
            if (queueResult != null)
            {
                if (isPreloadGame)
                {
                    transform.position = queueResult.position;
                    if (queueResult.funcCallback != null)
                        queueResult.funcCallback(this);
                }
                else
                {
                    MoveTowards(queueResult.position, () =>
                    {
                        if (queueResult.funcCallback != null)
                            queueResult.funcCallback(this);
                    });
                }
            }
            else
            {
                ChangeState(PatientState.INSPECTION_AREA);
            }
        }

        /// <summary>
        /// Call when consulation fees transation is completed
        /// </summary>
        public virtual void GotowardsDoctor(bool isPreloadGame = false)
        {
            CallbackIfContainsKey(Management.Hospital.ConsulationDepartment.PaymentCBKey);
            ChangeState(PatientState.GOING_TOWARDS_DOCTOR, isPreloadGame);
        }

        /// <summary>
        /// Call when payment is completed to switch to next state
        /// </summary>
        public virtual void PharmacyPaymentFinished()
        {
            PaymentTransationCompleted();
        }

        /// <summary>
        /// Call when payment is completed and now return towars the car
        /// Responsible to animate towars the car
        /// </summary>
        protected virtual void PaymentTransationCompleted()
        {
            CallbackIfContainsKey(Management.Hospital.PharmacyDepartment.PaymentCBKey);
            ChangeState(PatientState.GO_BACK_HOME, /*From Pharmacy*/1000);
        }

        protected virtual void GoingBackHome(int areaFrom)
        {
            List<Vector3> pathTowardsCar = new List<Vector3>();
            if (areaFrom == 1000)
            {
                pathTowardsCar.AddRange(refStateController.HospitalManager.HospitalBuilding.InHospitalPathFinder.GetPathTowardsExitDoor(DepartmentType.PHARMASY));
            }
            else
            {
                Hospital.InspectionArea.InspectionAreaPoint inspectionPoint = (Hospital.InspectionArea.InspectionAreaPoint)areaFrom;
                if(inspectionPoint == Hospital.InspectionArea.InspectionAreaPoint.BESIDE_LOBBY || inspectionPoint == Hospital.InspectionArea.InspectionAreaPoint.DEPARTMENT_SEARCH_AREA)
                    pathTowardsCar.AddRange(refStateController.HospitalManager.HospitalBuilding.InHospitalPathFinder.GetPathTowardsExitDoor());
            }
            pathTowardsCar.AddRange(refStateController.HospitalManager.HospitalBuilding.GetPathTowrdsCar(carBase.ParkingSlot.ActorSpawnPoint.position));
            MoveTowards(pathTowardsCar, CbOnReachedNearCar);
        }

        /// <summary>
        /// Callback function if key contains
        /// </summary>
        /// <param name="indexKey"></param>
        void CallbackIfContainsKey(Management.Hospital.PaymentCallbackKey _paymentCallbackKey, bool removeFromDictonary = true)
        {
            if (CbOnPaymentCompleteDictonary != null)
            {
                if (CbOnPaymentCompleteDictonary.ContainsKey(_paymentCallbackKey))
                {
                    System.Action callBack = CbOnPaymentCompleteDictonary[_paymentCallbackKey];
                    if (callBack != null)
                    {
                        callBack();
                        if (removeFromDictonary)
                            CbOnPaymentCompleteDictonary.Remove(_paymentCallbackKey);
                    }
                }
            }
        }

        /// <summary>
        /// Reached towards the car now time to go back to home
        /// </summary>
        protected virtual void CbOnReachedNearCar()
        {
            carBase.CarReturnToExit();
            ChangeState(PatientState.INACTIVE);
        }

        /// <summary>
        /// Checks if the patient is inactive or waiting inside the patient pool
        /// </summary>
        /// <returns></returns>
        public bool IsInactive()
        {
            return patientState == PatientState.INACTIVE;
        }

        public virtual void SetPatientPositionCallback(System.Action _patientPosRelCallback)
        {
            cbOnTreatmentComplete = _patientPosRelCallback;
        }

        public virtual void MoveTowardsQueue()
        {

        }

        #region PRELOAD_GAME
        /// <summary>
        /// Changes the state to waiting lobby and sets patient position to the waiting seat position
        /// </summary>
        /// <param name="waitingSeatPos"></param>
        public void PreloadGameWithWaitingLobby(Dictionary<string, object> dictonary)
        {
            dictonary.Add("IS_PRELOAD", true);
            ChangeState(PatientState.WAITING_IN_LOBBY, dictonary);
        }

        public bool PreloadPayConsultationFees()
        {
            Management.Hospital.QueueHelper.AddQueueResult queueResult = refStateController.HospitalManager.StandQueueInConsultationFees(this);

            if (queueResult == null)
                return false;
            transform.position = queueResult.position;
            if (queueResult.funcCallback != null)
                queueResult.funcCallback(this);
            return true;
        }

        public void PreloadProceedTowardsPharmacyPayment()
        {
            FindDepartment();
            ChangeState(PatientState.ON_TREATMENT_COMPLETE, true);
        }

        public void PreloadInspectionArea()
        {
            ChangeState(PatientState.INSPECTION_AREA, true);
        }
        #endregion
    }
}
