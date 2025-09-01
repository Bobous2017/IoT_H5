using MySql.Data.MySqlClient;

namespace MqttRealTimeWeb.Models
{
    public class SettingsViewModel
    {
        public int id { get; set; }
        public string deviceID { get; set; }
        public string startHour { get; set; }
        public string endHour { get; set; }
        public string minTemp {  get; set; }
        public string maxTemp {  get; set; }

    }
}
