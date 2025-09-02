namespace MqttRealTimeWeb.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MySql.Data.MySqlClient;
    using System.Collections.Generic;

    namespace MqttApiPro.Controllers
    {
        public class SettingsController : Controller
        {
            private readonly string _connStr =
                "Server=192.168.1.137;Database=iotdata;User ID=iotuser;Password=iotpass;";

            public IActionResult Index()
            {
                var settingsList = new List<dynamic>();

                using (var conn = new MySqlConnection(_connStr))
                {
                    conn.Open();
                    var cmd = new MySqlCommand("SELECT * FROM settings", conn);
                    var reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        settingsList.Add(new
                        {
                            Id = reader["id"],
                            DeviceID = reader["deviceID"],
                            StartHour = reader["startHour"],
                            EndHour = reader["endHour"],
                            MinTemp = reader["minTemp"],
                            MaxTemp = reader["maxTemp"]
                        });
                    }
                }

                return View(settingsList);
            }
        }
    }

}
