using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;
using System.Text.Json;

namespace MqttApiPro.Services
{
    public class SettingsDb : BackgroundService
    {
        private readonly ILogger<SensorReadingsMQTT> _logger;

        private readonly string _connStr = "Server=192.168.1.137;Database=iotdata;User ID=iotuser;Password=iotpass;";

        public SettingsDb(ILogger<SensorReadingsMQTT> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var conn = new MySqlConnection(_connStr);
                    await conn.OpenAsync(stoppingToken);

                    var cmd = new MySqlCommand("SELECT * FROM settings", conn);


                    var reader = await cmd.ExecuteReaderAsync(stoppingToken);

                    if (await reader.ReadAsync(stoppingToken))
                    {
                        var reading = new
                        {
                            Id = reader["id"],
                            deviceID = reader["deviceID"],
                            startHour = reader["startHour"],
                            endHour = reader["endHour"],
                            minTemp = reader["minTemp"],
                            maxTemp = reader["maxTemp"]
                        };

                        _logger.LogInformation($"📢 Broadcasting reading ID {reading.Id}");

                        // Push to all SignalR clients
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading from DB or pushing to SignalR");
                }

                await Task.Delay(8000, stoppingToken); // Poll every 3 seconds
            }
        }
    }
}
