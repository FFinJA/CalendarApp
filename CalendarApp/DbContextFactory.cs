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
            // 读取配置文件
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configFile = Path.Combine(appDataPath, "CalendarApp", "config.json");

            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException("Configuration file not found.");
            }

            // 解密 JSON 文件并获取数据库配置
            string encryptedJson = File.ReadAllText(configFile);
            string json = DbConfigWindow.Decrypt(encryptedJson, "encrytedPassword2024FSD.12");
            var databaseConfig = JsonConvert.DeserializeObject<DatabaseConfig>(json);

            if (databaseConfig == null)
            {
                throw new InvalidOperationException("Invalid database configuration.");
            }

            // 构建基础的 SQL 连接字符串
            string sqlConnectionString = $@"Server={databaseConfig.Server};Database={databaseConfig.DatabaseName};
                                         User Id={databaseConfig.Username};Password={databaseConfig.Password};";

            // 使用 EntityConnectionStringBuilder 构建 Entity Framework 的连接字符串
            var sqlBuilder = new SqlConnectionStringBuilder(sqlConnectionString);
            var entityBuilder = new EntityConnectionStringBuilder
            {
                Provider = "System.Data.SqlClient",
                ProviderConnectionString = sqlBuilder.ToString(),
                Metadata = "res://*/MyDatabaseModel.csdl|res://*/MyDatabaseModel.ssdl|res://*/MyDatabaseModel.msl"
            };

            // 传递 EntityConnectionString
            return new calendadbEntities(entityBuilder.ToString());
        }
    }

}
    
