using MqttDashboard.Components;
using MqttDashboard.Serevices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();



builder.Services.AddSingleton<CounterService>();
builder.Services.AddHostedService<CounterBroadcaster>();

builder.Services.AddScoped(sp =>
    new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(10),
        BaseAddress = new Uri("http://192.168.1.147:5000") // API endpoint
    });

// 👉 Add CORS (even though not mandatory here, it's safe)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// ⚠️ Disable HTTPS redirection (use only HTTP)
app.UseStaticFiles();
app.UseCors(); // 👈 Apply CORS here
app.UseAntiforgery();

// Routing
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
