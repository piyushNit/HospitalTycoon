//using Newtonsoft.Json;
using System.Collections.Generic;

namespace Management.Hospital.Json
{
    [System.Serializable]
    public class UpgradableContent
    {
        public string upgrade_name { get; set; }
        public string upgrade_type { get; set; }
        public int level { get; set; }
        public int income_multiplyer { get; set; }
        public double time { get; set; }
        public string description { get; set; }
    }

    public class DepartmentUpgrades
    {
        public string department { get; set; }
        public string department_type { get; set; }
        public int base_income { get; set; }
        public int initital_cost { get; set; }
        public int salary_initial_cost { get; set; }
        public string description { get; set; }
        public List<UpgradableContent> upgradable_contents { get; set; }
    }

    public enum ENT_UpgradeType
    {
        All_Clear_Ear_Drops,
        Silencer_Cough_Syrup,
        All_Clear_Cold_Tablets,
        All_Clear_Nasal_Spray,
        Forceps,
        Mouth_Opener,
        ENT_Instruments,
        Micro_Wick_Drug_Delivery,
        Automatic_Patients_Chair,
        Automatic_Doctor_Stool,
        Automatic_Forceps,
        Hitech_Patients_Chair,
        Stethoscope,
        Surgical_Mirror,
        Surgical_Head_Mirror,
        Surgical_Headlights,
        Neck_Belt,
        Hearing_Aid,
        Endoscope,
        Portable_Monitor,
        Portable_Suction,
        Otoscope,
        Fiber_Otoscope,
        ENT_Microscopes,
        ENT_Workstation,
        Worlds_Best_ENT_Setup
    }
}