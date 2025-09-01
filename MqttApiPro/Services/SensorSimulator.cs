using Microsoft.AspNetCore.SignalR;
using MqttApiPro.Hubs;

namespace MqttApiPro.Services
{
    public class SensorSimulator : BackgroundService
    {
        private readonly IHubContext<SensorHub> _hub;
        private readonly ILogger<SensorSimulator> _logger;
        private readonly Random _rand = new();

        public SensorSimulator(IHubContext<SensorHub> hub, ILogger<SensorSimulator> logger)
        {
            _hub = hub;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("✅ MqttApiPro is broadcasting data in realtime...");

            // send one-time welcome message to all connected clients
            await _hub.Clients.All.SendAsync("BroadcastStatus",
                "✅ You are connected with SignalR. Your data is broadcasting in realtime to the console and the web browser.",
                cancellationToken: stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                var reading = new
                {
                    Id = _rand.Next(1000),
                    Topic = "Simulated/MQTT",
                    Payload = _rand.NextDouble().ToString("F2"),
                    Timestamp = DateTime.Now.ToString("s")
                };

                _logger.LogInformation("📡 Broadcasting ID={Id}, Payload={Payload}, Timestamp={Timestamp}",
                    reading.Id, reading.Payload, reading.Timestamp);

                await _hub.Clients.All.SendAsync("NewReading", reading, cancellationToken: stoppingToken);

                await Task.Delay(2000, stoppingToken);
            }
        }

    }
}
