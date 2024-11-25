using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Windows.Annotations.Storage;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace CalendarApp
{
    public class DatabaseConfig
    {
        public string Server { get; set; }
        public string Port { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
    /// <summary>
    /// DbConfigWindow.xaml 
    /// </summary>
    public partial class DbConfigWindow : Window
    {
        private const string _defaultPort = "1433";
        private const string _defaultDatabase = "calendardb";
        public DbConfigWindow()
        {
            InitializeComponent();
        }

        private void Window_OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadConfig();

            TbxPort.Text = string.IsNullOrWhiteSpace(TbxPort.Text) ? _defaultPort : TbxPort.Text;
            TbxDbName.Text = string.IsNullOrWhiteSpace(TbxDbName.Text) ? _defaultDatabase : TbxDbName.Text;
            TbxServer.Text = string.IsNullOrWhiteSpace(TbxServer.Text) ? "localhost" : TbxServer.Text;
            TbxUser.Text = string.IsNullOrWhiteSpace(TbxUser.Text) ? "" : TbxUser.Text;
            TbxPassword.Password = string.IsNullOrWhiteSpace(TbxPassword.Password) ? "" : TbxPassword.Password;
        }

        private void TbxPort_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BtnTestConn_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TbxServer.Text == "")
                {
                    TbxServer.Focus();
                    throw new ArgumentException("Server cannot be empty");

                }
                if (TbxUser.Text == "")
                {
                    TbxUser.Focus();
                    throw new ArgumentException("User name cannot be empty");
                }

                if (TbxPort.Text == "")
                {
                    TbxPort.Text = _defaultPort;
                }
                if (!int.TryParse(TbxPort.Text, out _))
                {
                    TbxPort.Focus();
                    throw new ArgumentException("Port must be a number");
                }
                if (TbxDbName.Text == "")
                {
                    TbxDbName.Text = _defaultDatabase;
                }

                if (TestConnection())
                {
                    MessageBox.Show(this, "Test connection succeeded", "CalendarApp", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(this, $"Database connection couldn't be tested: {ex.Message}", "CalendarApp", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (!TestConnection()) return;

            // Write config.xml
            try
            {
                //AppData\Roaming\
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                string folderPath = Path.Combine(appDataPath, "CalendarApp");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string configFile = Path.Combine(folderPath, "config.json");
                Directory.CreateDirectory(Path.GetDirectoryName(configFile));

                var databaseConfig = new DatabaseConfig
                {
                    Server = TbxServer.Text.Trim(),
                    Port = TbxPort.Text.Trim(),
                    DatabaseName = TbxDbName.Text.Trim(),
                    Username = TbxUser.Text.Trim(),
                    Password = TbxPassword.Password.Trim()
                };

                string json = JsonConvert.SerializeObject(databaseConfig, Newtonsoft.Json.Formatting.Indented);

                string encryptedJson = Encrypt(json, "encrytedPassword2024FSD.12");
                File.WriteAllText(configFile, encryptedJson);
                MessageBox.Show(this, "Configuration saved successfully.", "CalendarApp", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error saving configuration: {ex.Message}", "CalendarApp", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void BtnReset_OnClick(object sender, RoutedEventArgs e)
        {
            TbxServer.Text = "localhost";
            TbxPort.Text = _defaultPort;
            TbxDbName.Text = _defaultDatabase;
            TbxUser.Text = "";
            TbxPassword.Password = "";
        }

        public static string Encrypt(string plainText, string key)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(16).Substring(0, 16));
                aes.IV = new byte[16];
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var writer = new StreamWriter(cs))
                    {
                        writer.Write(plainText);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText, string key)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key.PadRight(16).Substring(0, 16));
                aes.IV = new byte[16]; 
                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private void LoadConfig()
        {
            try
            {
                
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string configFile = Path.Combine(appDataPath, "CalendarApp", "config.json");

                if (File.Exists(configFile))
                {
                    
                    string encryptedJson = File.ReadAllText(configFile);

                    
                    string json = Decrypt(encryptedJson, "encrytedPassword2024FSD.12");

                    
                    var databaseConfig = JsonConvert.DeserializeObject<DatabaseConfig>(json);

                    
                    TbxServer.Text = databaseConfig.Server;
                    TbxPort.Text = databaseConfig.Port;
                    TbxDbName.Text = databaseConfig.DatabaseName;
                    TbxUser.Text = databaseConfig.Username;
                    TbxPassword.Password = databaseConfig.Password;
                    Console.WriteLine(databaseConfig.Server);
                    Console.WriteLine(databaseConfig.DatabaseName);
                }
                else
                {
                    MessageBox.Show(this, "Configuration file not found. Please configure the database.",
                                    "CalendarApp", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Error loading configuration: {ex.Message}",
                                "CalendarApp", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private bool TestConnection()
        {
            string connStr = $@"Server={TbxServer.Text.Trim()},{TbxPort.Text.Trim()};Initial Catalog={TbxDbName.Text.Trim()};User Id={TbxUser.Text.Trim()};Password={TbxPassword.Password.Trim()};Encrypt=True;Connection Timeout=30;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    try
                    {
                        Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                        conn.Open();
                        return true;
                    }
                    catch (SqlException ex)
                    {
                        Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                        MessageBox.Show(this, $"Test connection failed: {ex.Message}", "CalendarApp", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                    finally
                    {
                        Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Test connection failed: {ex.Message}", "CalendarApp", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}

