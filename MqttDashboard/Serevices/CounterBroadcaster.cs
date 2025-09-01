using Microsoft.AspNetCore.SignalR;
using MqttDashboard.Models;
using MqttDashboard.Serevices;


namespace MqttDashboard.Serevices
{
    public class CounterBroadcaster : BackgroundService
    {
        private readonly CounterService _counterService;
        private readonly ILogger<CounterBroadcaster> _logger;
        private int _count = 0;

        public CounterBroadcaster(CounterService counterService, ILogger<CounterBroadcaster> logger)
        {
            _counterService = counterService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 CounterBroadcaster started");

            while (!stoppingToken.IsCancellationRequested)
            {
                _count++;
                _logger.LogInformation("📡 Broadcasting counter value {Count}", _count);

                _counterService.RaiseCounter(_count);   // notify Blazor
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}

