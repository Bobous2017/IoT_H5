namespace MqttDashboard.Models
{
    public class SensorReading
    {
        public int Id { get; set; }
        public string Topic { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
    }
}
