using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarApp
{
    using System.Data.Entity.Core.EntityClient;
    using System.Data.SqlClient;

    public static class DbContextFactory
    {
        public static calendadbEntities CreateDbContext()
        {
            // load the configuration file
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configFile = Path.Combine(appDataPath, "CalendarApp", "config.json");

            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException("Configuration file not found.");
            }

            // decrypt the configuration file
            string encryptedJson = File.ReadAllText(configFile);
            string json = DbConfigWindow.Decrypt(encryptedJson, "encrytedPassword2024FSD.12");
            var databaseConfig = JsonConvert.DeserializeObject<DatabaseConfig>(json);

            if (databaseConfig == null)
            {
                throw new InvalidOperationException("Invalid database configuration.");
            }

            // create the connection string
            string sqlConnectionString = $@"Server={databaseConfig.Server};Database={databaseConfig.DatabaseName};
                                         User Id={databaseConfig.Username};Password={databaseConfig.Password};";

            // using EntityConnectionStringBuilder to build the EntityConnectionString
            var sqlBuilder = new SqlConnectionStringBuilder(sqlConnectionString);
            var entityBuilder = new EntityConnectionStringBuilder
            {
                Provider = "System.Data.SqlClient",
                ProviderConnectionString = sqlBuilder.ToString(),
                Metadata = "res://*/MyDatabaseModel.csdl|res://*/MyDatabaseModel.ssdl|res://*/MyDatabaseModel.msl"
            };

            // pass the EntityConnectionString to the DbContext
            return new calendadbEntities(entityBuilder.ToString());
        }
    }

}
    
