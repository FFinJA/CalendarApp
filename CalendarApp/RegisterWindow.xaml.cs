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
    /// RegisterWindow.xaml 
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // get input
                string username = UserNameTextBox.Text.Trim();
                string email = EmailTextBox.Text.Trim();
                string password = PasswordTextBox.Password.Trim();
                string confirmPassword = ConfirmPasswordTextBox.Password.Trim();

                // validation
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||
                    string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
                {
                    MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (password != confirmPassword)
                {
                    MessageBox.Show("Passwords do not match.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!IsValidEmail(email))
                {
                    MessageBox.Show("Invalid email format.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // encrypt password
                string hashedPassword = HashPassword(password);

                // save to database
                using (var context = DbContextFactory.CreateDbContext())
                {
                    // check if username or email already exists
                    if (context.users.Any(u => u.username == username || u.email == email))
                    {
                        MessageBox.Show("Username or email already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // new a user
                    var newUser = new users
                    {
                        username = username,
                        email = email,
                        password = hashedPassword,
                        created_at = DateTime.Now
                    };

                    context.users.Add(newUser);
                    context.SaveChanges();

                    MessageBox.Show("Registration successful!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close(); // close the window
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // open the login window
            var loginWindow = new LoginWindow();
            loginWindow.Owner = this.Owner; // set the owner of the window
            loginWindow.Show();
            this.Close();
        }

        // check if the email is valid
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // hash the password
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

