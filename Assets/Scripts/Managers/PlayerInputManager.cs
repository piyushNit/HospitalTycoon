using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.core
{
    public class PlayerInputManager : MonoBehaviour
    {
        [Header("Camera Panning")]
        [SerializeField] Transform cameraTrans;
        public Transform CameraTrans { get => cameraTrans; }
        [SerializeField] float cameraPanningSpeed = 0.8f;
        Vector3 cameraPanPosition;
        [SerializeField] Vector3 cameraLimitA;
        [SerializeField] Vector3 cameraLimitB;

        [Header("Player Touch")]
        [SerializeField] float tapTime = 0.5f;
        [SerializeField] float holdTime = 1f;
        float currTime = 0;

        [Header("Game Environment")]
        RaycastHit hit;
        [SerializeField] LayerMask layerMask;
        [SerializeField] Arch.Core.StateController _stateController;

        private Vector3 Origin;
        private Vector3 difference;
        private bool Drag = false;
        private bool isDragging = false;

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Vector3[] verts = new Vector3[]
            {
                new Vector3(cameraLimitA.x, cameraLimitA.y, cameraLimitA.z),
                new Vector3(cameraLimitA.x, cameraLimitA.y, cameraLimitB.z),
                new Vector3(cameraLimitB.x, cameraLimitB.y, cameraLimitB.z),
                new Vector3(cameraLimitB.x, cameraLimitB.y, cameraLimitA.z)
            };

            for (int i = 0; i < verts.Length; i++)
            {
                if (i == 0)
                    Gizmos.DrawLine(verts[i], verts[verts.Length - 1]);
                else
                    Gizmos.DrawLine(verts[i - 1], verts[i]);
            }

        }
        #endif

        private void Start()
        {
            cameraPanPosition = cameraTrans.position;
        }

        Vector3 initialHitPoint;
        Vector3 initTouchPos;
        Vector3 finalPos;
        Ray ray;
        //RaycastHit dragRaycastHit;
        public void Update()
        {
            if (_stateController.UiHolder.UiUtilities.IsLoadingSpinnerActivated
                || (_stateController.UiHolder.CurrActiveUI != UI.UiType.HUD && !_stateController.FTUEManager.IsFTUERunning)
                || _stateController.UiHolder.IsPopupActive()
                || _stateController.UiHolder.ignoreTouchWhenNotificationClicked)
                return;

            #if TEST_GAME
            if(_stateController.UiHolder.uiDebugPrinter.IsDebugActive())
                return;
            #endif

            if (Input.GetButtonDown("Fire1"))
            {
                if (!_stateController.FTUEManager.IsCameraDrag)
                {
                    currTime = 0;
                    return;
                }
                currTime = 0;
                isDragging = false;
                initTouchPos = Input.mousePosition;
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Ground")))
                {
                    initialHitPoint = hit.point;
                }
            }

            if (Input.GetButton("Fire1"))
            {
                if (!_stateController.FTUEManager.IsCameraDrag)
                    return;
                currTime += Time.deltaTime;
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                finalPos = initTouchPos - Input.mousePosition;
                if (Mathf.Round(finalPos.x) != 0 || Mathf.Round(finalPos.y) != 0)
                {
                    Drag = true;
                    isDragging = true;
                    if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Ground")))
                    {
                        difference = initialHitPoint - hit.point;
                    }
                }
                else
                {
                    if (currTime > holdTime)
                    {
                        if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Ground")))
                        {
                            difference = initialHitPoint - hit.point;
                        }
                        if (Drag == false)
                        {
                            Drag = true;
                            isDragging = true;
                        }
                    }
                }
            }
            else
            {
                Drag = false;
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (currTime < tapTime && isDragging == false && (_stateController.UiHolder.CurrActiveUI == UI.UiType.HUD || _stateController.FTUEManager.IsFTUERunning))
                {
                    FindDepartmentTouch();
                    currTime = 0;
                }
                isDragging = false;
            }
        }

        public void UpdateCameraPosition(Vector3 cameraPos)
        {
            cameraPanPosition = new Vector3(cameraPos.x, cameraPanPosition.y, cameraPos.z);
            cameraTrans.position = cameraPanPosition;
        }

        public void FindDepartmentTouch()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100, layerMask))
            {
                if (OpenDepartmentUI()) { }
                else if (OpenWaitingSeats()) { }
                else if (OpenPayementDepartment()) { }
                else if (OpenParkingUI()) { }
                else if (OpenAdminUI()) { }
            }
        }

        bool OpenWaitingSeats()
        {
            if (hit.transform.parent == null || _stateController.FTUEManager.IsFTUERunning)
                return false;
            Management.Hospital.WaitingLobby waitingLobby = hit.transform.parent.GetComponent<Management.Hospital.WaitingLobby>();

            if (waitingLobby != null)
            {
                _stateController.UiHolder.ShowUi(UI.UiType.Waiting_Lobby);
                return true;
            }
            return false;
        }

        bool OpenDepartmentUI()
        {
            Management.Hospital.Department department = hit.transform.GetComponent<Management.Hospital.Department>();
            if (department != null)
            {
                if (!AllowOpeningUIInFTUE(department))
                    return false;

                if (department.UiType == UI.UiType.Admin_Department)
                    _stateController.UiHolder.ShowUi(department.UiType);
                else
                    _stateController.UiHolder.ShowUiWithDepartmentInfo(department.UiType, department);
                return true;
            }
            return false;
        }

        bool OpenPayementDepartment()
        {
            if (_stateController.FTUEManager.IsFTUERunning)
                return false;

            Management.Hospital.PaymentDepartment paymentDepartment = hit.transform.GetComponent<Management.Hospital.PaymentDepartment>();
            if (paymentDepartment != null)
            {
                _stateController.UiHolder.ShowUiWithDepartmentInfo(paymentDepartment.UiType, paymentDepartment);
                return true;
            }
            return false;
        }

        bool OpenParkingUI()
        {
            if (_stateController.FTUEManager.IsFTUERunning && _stateController.FTUEManager.CurrentFtueType != FTUE.FTUEType.FTUE_PARKING_CLICK)
                return false;
            Management.Parking.ParkingUnit parkingUnit = hit.transform.GetComponent<Management.Parking.ParkingUnit>();
            if (parkingUnit != null)
            {
                _stateController.UiHolder.ShowParkingInfo();
                return true;
            }
            return false;
        }

        bool OpenAdminUI()
        {
            if (_stateController.FTUEManager.IsFTUERunning)
                return false;
            Management.Hospital.AdminDepartment adminDepartment = hit.transform.GetComponent<Management.Hospital.AdminDepartment>();
            if (adminDepartment != null)
            {
                _stateController.UiHolder.ShowUi(adminDepartment.UiType);
                return true;
            }
            return false;
        }

        Vector3 velocity;
        void LateUpdate()
        {
            if (Drag == true)
            {
                cameraPanPosition += new Vector3(difference.x, 0, difference.z);

                if (cameraPanPosition.x >= cameraLimitA.x)
                    cameraPanPosition.x = cameraLimitA.x;
                if (cameraPanPosition.x <= cameraLimitB.x)
                    cameraPanPosition.x = cameraLimitB.x;

                if (cameraPanPosition.z >= cameraLimitA.z)
                    cameraPanPosition.z = cameraLimitA.z;
                if (cameraPanPosition.z <= cameraLimitB.z)
                    cameraPanPosition.z = cameraLimitB.z;
            }

            //cameraTrans.position = Vector3.SmoothDamp(cameraTrans.position, cameraPanPosition, ref velocity, cameraPanningSpeed);
            cameraTrans.position = Vector3.Lerp(cameraTrans.position, cameraPanPosition, cameraPanningSpeed * Time.deltaTime);
        }

        #region FTUE
        bool AllowOpeningUIInFTUE(Hospital.Department department)
        {
            if (!_stateController.FTUEManager.IsFTUERunning)
                return false;

            if (_stateController.FTUEManager.CurrentFtueType == FTUE.FTUEType.FTUE_IMPROVE_ENT_CLINIC
                && department.UiType == UI.UiType.Hospital_Department
                && department.DepartmentType == Hospital.Core.DepartmentType.ENT)
                return true;
            if (_stateController.FTUEManager.CurrentFtueType == FTUE.FTUEType.FTUE_PARKING_CLICK
                && department.UiType == UI.UiType.Hospital_Department
                && department.DepartmentType == Hospital.Core.DepartmentType.ENT)
                return true;
            return false;
        }
        #endregion
    }
}
