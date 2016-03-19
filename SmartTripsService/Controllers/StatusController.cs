using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using SmartTripsService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace SmartTripsService.Controllers
{
    public class StatusController : Controller
    {
        static RegistryManager registryManager;
        static string deviceId = "SmartTripsMobileApp";

        // GET: Status
        public async Task<ActionResult> Index()
        {
            var model = new StatusViewModel();
            await TestIoTHubConnection(model);
            await TestDatabaseConnection(model);

            return View(model);
        }

        private async static Task<string> AddDeviceAsync()
        {
            Device device;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
            }
            catch (DeviceAlreadyExistsException)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }

            return device.Authentication.SymmetricKey.PrimaryKey;
        }

        private static async Task TestIoTHubConnection(StatusViewModel model)
        {
            try
            {
                var connectionString = WebConfigurationManager.AppSettings["IoTHubConnectionString"];

                Debug.WriteLine("Registering the simulated device...");
                registryManager = RegistryManager.CreateFromConnectionString(connectionString);
                var deviceKey = await AddDeviceAsync();
                Debug.WriteLine("Device was registered...");
                model.IoTStatus = "SUCCESS";
                model.IoTDeviceKey = deviceKey.Substring(0, 8) + new string('*', deviceKey.Length - 16) + deviceKey.Substring(deviceKey.Length - 8);
                model.IoTReason = "";
            }
            catch (Exception ex)
            {
                model.IoTStatus = "FAILURE";
                model.IoTDeviceKey = "<unknown>";
                model.IoTReason = ex.Message;
            }
        }

        private static async Task TestDatabaseConnection(StatusViewModel model)
        {
            var rows = 0;

            try
            {
                var connectionString = WebConfigurationManager.ConnectionStrings["MS_TableConnectionString"].ConnectionString;

                using (var connection = new SqlConnection())
                {
                    connection.ConnectionString = connectionString;
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM dbo.UserProfiles";
                    command.CommandType = CommandType.Text;
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            rows++;
                        }
                    }
                }

                model.SqlStatus = "SUCCESS";
                model.SqlRowCount = rows;
                model.SqlReason = "";
            }
            catch (Exception ex)
            {
                model.SqlStatus = "FAILURE";
                model.SqlRowCount = -1;
                model.SqlReason = ex.Message;
            }
        }
    }
}