using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CalendarApp
{
    /// <summary>
    /// LoginWindow.xaml 
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

       
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                string email = EmailTextBox.Text.Trim();
                string password = PasswordBox.Password.Trim();

                
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Email and password are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                
                string hashedPassword = HashPassword(password);

                //get dbcontext from factory class
                using (var context = DbContextFactory.CreateDbContext())
                {
                    //check if existing user with same email
                    var user = context.users.FirstOrDefault(u => u.email == email && u.password == hashedPassword);

                    if (user != null)
                    {
                        MessageBox.Show($"Welcome, {user.username}!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);

                       
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Invalid email or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // open register window
            var registerWindow = new RegisterWindow();
            registerWindow.Owner = this.Owner; // set parent window as main window
            registerWindow.Show();
            this.Close();
        }

        
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
