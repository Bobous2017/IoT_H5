using Microsoft.AspNetCore.SignalR;

namespace MqttApiPro.Hubs
{
    public class SensorHub : Hub
    {
        // Optional: method to confirm connection
        public async Task Ping() => await Clients.Caller.SendAsync("Pong");
    }
}
