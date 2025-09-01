using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MySql.Data.MySqlClient;
using System.Text;

namespace MqttApiPro.Services
{
    public class MqttSubscriberService : BackgroundService
    {
        private readonly ILogger<MqttSubscriberService> _logger;
        private readonly string _connStr = "Server=192.168.1.137;Database=iotdata;User ID=iotuser;Password=iotpass;";
        private IMqttClient? _mqttClient;

        public MqttSubscriberService(ILogger<MqttSubscriberService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("192.168.1.137", 1883)
                .WithClientId("ApiSubscriber")
                .Build();

            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                try
                {
                    var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    var topic = e.ApplicationMessage.Topic;

                    _logger.LogInformation("📩 MQTT Received: Topic={Topic}, Payload={Payload}", topic, payload);

                    using var conn = new MySqlConnection(_connStr);
                    await conn.OpenAsync(stoppingToken);

                    var cmd = new MySqlCommand(
                        "INSERT INTO sensor_readings (topic, payload, timestamp) VALUES (@topic, @payload, @timestamp)",
                        conn);

                    cmd.Parameters.AddWithValue("@topic", topic);
                    cmd.Parameters.AddWithValue("@payload", payload);
                    cmd.Parameters.AddWithValue("@timestamp", DateTime.UtcNow);

                    var rows = await cmd.ExecuteNonQueryAsync(stoppingToken);

                    if (rows > 0)
                        _logger.LogInformation("✅ Insert succeeded → {Rows} row(s) added. Topic={Topic}, Payload={Payload}", rows, topic, payload);
                    else
                        _logger.LogWarning("⚠️ Insert executed but no rows were added! Topic={Topic}, Payload={Payload}", topic, payload);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error inserting MQTT message into DB");
                }
            };

            await _mqttClient.ConnectAsync(options, stoppingToken);

            await _mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter("Topic:OpsætningMQTT")   // <-- match the Pi publishes
            //.WithTopicFilter("#")   // subscribe to all topics
            .Build(), stoppingToken);


            _logger.LogInformation("✅ Subscribed to MQTT topic: sensors/#");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
