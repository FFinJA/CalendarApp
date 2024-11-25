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

        // "Sign me up" 按钮点击事件
        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 获取用户输入
                string username = UserNameTextBox.Text.Trim();
                string email = EmailTextBox.Text.Trim();
                string password = PasswordTextBox.Password.Trim();
                string confirmPassword = ConfirmPasswordTextBox.Password.Trim();

                // 验证输入
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

                // 加密密码
                string hashedPassword = HashPassword(password);

                // 保存到数据库
                using (var context = DbContextFactory.CreateDbContext())
                {
                    // 检查用户名或邮箱是否已存在
                    if (context.users.Any(u => u.username == username || u.email == email))
                    {
                        MessageBox.Show("Username or email already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // 插入新用户
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
                    this.Close(); // 关闭注册窗口
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // "Login" 按钮点击事件
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // 打开登录窗口
            var loginWindow = new LoginWindow();
            loginWindow.Owner = this.Owner; // 设置父窗口为主窗口
            loginWindow.Show();
            this.Close();
        }

        // 检查邮箱格式
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

        // 密码哈希函数（使用 SHA256）
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

