using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarApp
{
    public static class DatabaseConfigLoader
    {
        public static string LoadConnectionString()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configFile = Path.Combine(appDataPath, "CalendarApp", "config.json");

            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException("Configuration file not found.");
            }

            string encryptedJson = File.ReadAllText(configFile);
            string json = DbConfigWindow.Decrypt(encryptedJson, "encrytedPassword2024FSD.12");
            var databaseConfig = JsonConvert.DeserializeObject<DatabaseConfig>(json);

            if (databaseConfig == null)
            {
                throw new InvalidOperationException("Invalid database configuration.");
            }

            return $@"Server={databaseConfig.Server};Database={databaseConfig.DatabaseName};
                  User Id={databaseConfig.Username};Password={databaseConfig.Password};";
        }
    }

}
