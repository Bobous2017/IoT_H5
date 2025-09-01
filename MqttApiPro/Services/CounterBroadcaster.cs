using Microsoft.AspNetCore.SignalR;
using MqttApiPro.Hubs;

public class CounterBroadcaster : BackgroundService
{
    private readonly IHubContext<SensorHub> _hub;
    private int _count = 0;

    public CounterBroadcaster(IHubContext<SensorHub> hub)
    {
        _hub = hub;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _count++;

            await _hub.Clients.All.SendAsync("CounterUpdated", _count, stoppingToken);
            await Task.Delay(1000, stoppingToken); // every second
        }
    }
}
