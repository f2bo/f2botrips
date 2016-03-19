namespace SmartTripsService.Models
{
    public class StatusViewModel
    {
        public string IoTDeviceKey { get; set; }

        public string IoTStatus { get; set; }

        public string IoTReason { get; set; }

        public int SqlRowCount { get; set; }

        public string SqlStatus { get; set; }

        public string SqlReason { get; set; }
    }
}