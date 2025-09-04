using Microsoft.AspNetCore.Mvc;
using MqttRealTimeWeb.Models;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace MqttApiPro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _connStr = "Server=192.168.1.137;Database=iotdata;User ID=iotuser;Password=iotpass;";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        //------------------------------------ CRUD Håndter Settings --------------------------------------
        // ✅ READ: Show Settings
        [HttpGet]
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
                        DeviceID = reader["deviceID"].ToString(),
                        StartHour = reader["startHour"].ToString(),
                        EndHour = reader["endHour"].ToString(),
                        MinTemp = reader["minTemp"].ToString(),
                        MaxTemp = reader["maxTemp"].ToString(),
                        msgInterval = (int)reader["msgInterval"]
                    });
                }
            }

            return View(settingsList); // → settings.cshtml
        }

        // ✅ CREATE or UPDATE Settings
        [HttpPost]
        public IActionResult Settings(Setting setting)
        {
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();

                // check if deviceID exists
                var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM settings WHERE deviceID = @deviceID", conn);
                checkCmd.Parameters.AddWithValue("@deviceID", setting.DeviceID);

                var exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;

                if (exists)
                {
                    // UPDATE
                    var updateCmd = new MySqlCommand(@"UPDATE settings SET 
                        startHour = @startHour, 
                        endHour = @endHour, 
                        minTemp = @minTemp, 
                        maxTemp = @maxTemp,
                        msgInterval = @msgInterval
                        WHERE deviceID = @deviceID", conn);

                    updateCmd.Parameters.AddWithValue("@startHour", setting.StartHour);
                    updateCmd.Parameters.AddWithValue("@endHour", setting.EndHour);
                    updateCmd.Parameters.AddWithValue("@minTemp", setting.MinTemp);
                    updateCmd.Parameters.AddWithValue("@maxTemp", setting.MaxTemp);
                    updateCmd.Parameters.AddWithValue("@msgInterval", setting.msgInterval);
                    updateCmd.Parameters.AddWithValue("@deviceID", setting.DeviceID);

                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    // INSERT
                    var insertCmd = new MySqlCommand(@"INSERT INTO settings 
                        (deviceID, startHour, endHour, minTemp, maxTemp, msgInterval)
                        VALUES (@deviceID, @startHour, @endHour, @minTemp, @maxTemp, @msgInterval)", conn);

                    insertCmd.Parameters.AddWithValue("@deviceID", setting.DeviceID);
                    insertCmd.Parameters.AddWithValue("@startHour", setting.StartHour);
                    insertCmd.Parameters.AddWithValue("@endHour", setting.EndHour);
                    insertCmd.Parameters.AddWithValue("@minTemp", setting.MinTemp);
                    insertCmd.Parameters.AddWithValue("@maxTemp", setting.MaxTemp);
                    insertCmd.Parameters.AddWithValue("@msgInterval", setting.msgInterval);

                    insertCmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Settings");
        }

        // ✅ DELETE Setting by DeviceID
        [HttpPost]
        public IActionResult DeleteSetting(string deviceID)
        {
            using (var conn = new MySqlConnection(_connStr))
            {
                conn.Open();

                var cmd = new MySqlCommand("DELETE FROM settings WHERE deviceID = @deviceID", conn);
                cmd.Parameters.AddWithValue("@deviceID", deviceID);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Settings");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
