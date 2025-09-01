//using MqttRealTimeWeb.Hubs;
//using MqttRealTimeWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(10),
        BaseAddress = new Uri("http://192.168.1.147:5000") // API endpoint
    });

// 👉 Add CORS (even though not mandatory here, it's safe)
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

/*builder.Services.AddHostedService<SensorSimulator>();*/ // background data generator
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}
    
//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseCors("AllowMvcClient");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//app.MapHub<SensorHub>("/sensorhub");  // ✅ register hub
app.Run();
