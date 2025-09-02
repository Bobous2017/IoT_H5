using Microsoft.AspNetCore.Mvc;
using MqttRealTimeWeb.Models;  // ✅ import model namespace
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Collections.Generic;

namespace MqttApiPro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _connStr =
              "Server=192.168.1.137;Database=iotdata;User ID=iotuser;Password=iotpass;";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Settings()
        {
            var settingsList = new List<Setting>();

            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM settings", conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    settingsList.Add(new Setting
                    {
                        Id = (int)reader["id"],
                        DeviceID = reader["deviceID"].ToString(),
                        StartHour = reader["startHour"].ToString(),
                        EndHour = reader["endHour"].ToString(),
                        MinTemp = reader["minTemp"].ToString(),
                        MaxTemp = reader["maxTemp"].ToString()
                    });
                }
            }

            return View(settingsList); // ✅ pass to Settings.cshtml
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
