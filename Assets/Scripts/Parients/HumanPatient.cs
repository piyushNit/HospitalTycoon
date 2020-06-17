using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management.Patient {
    public class HumanPatient : PatientBase {
        public override void Initilize(Arch.Core.StateController _stateController)
        {
            base.Initilize(_stateController);
            SetDisease();
        }

        /// <summary>
        /// Randamize and set the doctor depend on the availability of the departments
        /// </summary>
        void SetDisease()
        {
            List<Management.Hospital.Department> availableDepartments = refStateController.HospitalManager.GetDepartments;
            Management.Hospital.Department randomDepartment = availableDepartments[Random.Range(0, availableDepartments.Count)];
            doctorType = refStateController.GameManager.WhichDoctor(randomDepartment.DepartmentType);
        }
    }
}
