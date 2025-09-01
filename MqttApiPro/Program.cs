using MqttApiPro.Hubs;
using MqttApiPro.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.PropertyNamingPolicy = null; // keep PascalCase
    });

// ✅ CORS – allow MVC frontend both on localhost + LAN IP
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMvcClient", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5239",
            "http://192.168.1.147:5239",
            "http://192.168.1.140:5239"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});



builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // HTTP
    options.ListenAnyIP(7241, listenOptions =>
    {
        listenOptions.UseHttps(); // HTTPS (optional)
    });
});

//builder.Services.AddHostedService<SensorSimulator>();
builder.Services.AddHostedService<MqttSubscriberService>();
builder.Services.AddHostedService<SensorReadingsMQTT>();



//builder.Services.AddHostedService<CounterBroadcaster>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger (only if needed)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// Middleware
app.UseRouting();

//  apply the CORS policy
app.UseCors("AllowMvcClient");

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<SensorHub>("/sensorhub")
             .RequireCors("AllowMvcClient"); // 👈 enforce policy also here
});
//app.MapControllers();

////  Map SignalR hub
//app.MapHub<SensorHub>("/sensorhub");

app.Run();
