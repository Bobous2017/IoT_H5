using MQTTnet;
using MQTTnet.Client;
using MySql.Data.MySqlClient;
using System.Text;

var factory = new MqttFactory();
var mqttClient = factory.CreateMqttClient();

var options = new MqttClientOptionsBuilder()
    .WithTcpServer("192.168.1.137", 1883) // Use Pi IP if needed
    .WithClientId("csharp-subscriber")
    .Build();

mqttClient.ApplicationMessageReceivedAsync += async e =>
{
    string topic = e.ApplicationMessage.Topic;
    //string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
    var raw = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
    var payload = raw.Trim('\0', '\r', '\n');


    Console.WriteLine($"[{topic}] {payload}");

    // Save to MySQL
    try
    {
        string connStr = "Server=192.168.1.137;Database=iotdata;User ID=iotuser;Password=iotpass;";
        using var conn = new MySqlConnection(connStr);
        await conn.OpenAsync();

        var cmd = new MySqlCommand("INSERT INTO sensor_readings (topic, payload) VALUES (@t, @p)", conn);
        cmd.Parameters.AddWithValue("@t", topic);
        cmd.Parameters.AddWithValue("@p", payload);
        await cmd.ExecuteNonQueryAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine("DB error: " + ex.Message);
    }

    return;
};


await mqttClient.ConnectAsync(options);
await mqttClient.SubscribeAsync("#"); // Subscribe to all topics

Console.WriteLine("Subscribed. Press any key to exit.");
Console.ReadKey();
