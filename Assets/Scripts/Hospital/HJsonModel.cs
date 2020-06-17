namespace Management.Hospital.Json
{
    public class ParkingJsonModel
    {
        //Parking
        public int ParkingBaseCost { get; set; }
        public int ParkingCostIncreasePercent { get; set; }
        public int UnlockParkingPerUpgrade { get; set; }
        public int MaxUndergroundParkingSlots { get; set; }
        public int MaxParkingSlots { get; set; }
        //Advertisement
        public int AdvertisementBaseCost { get; set; }
        public int AdvertisementCostIncreasePercent { get; set; }
        public int AdvertisementMaxPatientsPerMinute { get; set; }
    }

    public class DepartmenStaffAndSalarytData
    {
        public double PharmacyStaffHireBaseCost { get; set; }
        public double PharmacyStaffHireCostMultiplyBy { get; set; }
        public double PharmacySalaryHikeBaseCost { get; set; }
        public double PharmacySalaryHikeCostMultiplyBy { get; set; }
        public double ConsulationStaffHireBaseCost { get; set; }
        public double ConsulationStaffHireCostMultiplyBy { get; set; }
        public double ConsulationSalaryHikeBaseCost { get; set; }
        public double ConsulationSalaryHikeCostMultiplyBy { get; set; }
        public double ENTSalaryHikeBaseCost { get; set; }
        public double ENTSalaryHikeMultiplyBy { get; set; }
        public double EyeSalaryHikeBaseCost { get; set; }
        public double EyeSalaryHikeMultiplyBy { get; set; }
        public double DentalSalayHikeBaseCost { get; set; }
        public double DentalHikeMultiplyBy { get; set; }
        public double SkinSalayHikeBaseCost { get; set; }
        public double SkinHikeMultiplyBy { get; set; }
        public double CardiologySalaryHikeBaseCost { get; set; }
        public double CardiologySalaryHikeMultiplyBy { get; set; }
        public double GynecologySalaryHikeBaseCost { get; set; }
        public double GynecologySalaryHikeMultiplyBy { get; set; }
        public double OrthopedicsSalayHikeBaseCost { get; set; }
        public double OrthopedicsHikeMultiplyBy { get; set; }
        public double NurologySalayHikeBaseCost { get; set; }
        public double NurologyHikeMultiplyBy { get; set; }
        public double PaediatricsSalaryHikeBaseCost { get; set; }
        public double PaediatricsSalaryHikeMultiplyBy { get; set; }
        public double PlasticSurgenSalaryHikeBaseCost { get; set; }
        public double PlasticSurgenSalaryHikeMultiplyBy { get; set; }
        public double SurgeonSalayHikeBaseCost { get; set; }
        public double SurgeonHikeMultiplyBy { get; set; }
    }

    public class WaitingLobbyUnlockModel
    {
        public double WaitingLobbyBaseCost { get; set; }
        public double WaitingLobbyMultuplyBy { get; set; }
    }
}