namespace MqttRealTimeWeb.Models
{
    public class Setting
    {
        public int Id { get; set; }
        public string DeviceID { get; set; }
        public string StartHour { get; set; }
        public string EndHour { get; set; }
        public string MinTemp { get; set; }
        public string MaxTemp { get; set; }
    }
}
