using System;

namespace DeviceSimulator.Model
{
    public class MeasurementModel
    {
        public int VendorId { get; set; }
        public DateTime PickupDateTime { get; set; }
        public DateTime DropOffDateTime { get; set; }
        public int PassengerCount { get; set; }
        public double TripDistance { get; set; }
        public int RateCodeId { get; set; }
        public bool StoreAndFwdFlag { get; set; }
        public int PULocationID { get; set; }
        public int DOLocationID { get; set; }
        public int PaymentType { get; set; }
        public double FareAmount { get; set; }
        public double Extra { get; set; }
        public double MtaTax { get; set; }
        public double TipAmount { get; set; }
        public double TollsAmount { get; set; }
        public double ImprovementSurcharge { get; set; }
        public double TotalAmount { get; set; }
    }
}
