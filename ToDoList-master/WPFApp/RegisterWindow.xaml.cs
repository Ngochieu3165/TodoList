using BusinessObjects;
using DataAccessLayer;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace WPFApp.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly ToDoListContext _dbContext;

        public RegisterWindow()
        {
            InitializeComponent();
            _dbContext = new ToDoListContext();
            RegisterButton.Click += RegisterButton_Click;
            BackToLoginButton.Click += BackToLoginButton_Click;
        }
        private void BackToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is LoginWindow loginWindow)
                {
                    loginWindow.Activate();
                    this.Close();
                    return;
                }
            }

            LoginWindow newLoginWindow = new LoginWindow();
            newLoginWindow.Show();
            this.Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Collect input data
            string username = UsernameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            string fullName = FullNameTextBox.Text.Trim();
            DateTime? dayOfBirth = DayOfBirthDatePicker.SelectedDate;
            string phone = PhoneTextBox.Text.Trim();

            // Validate inputs
            if (!ValidateInputs(username, email, password, confirmPassword))
            {
                return; // Early return if validation fails
            }

            // Hash the password
            string passwordHash = HashPassword(password);

            // Check if user exists
            if (IsUserExists(username, email))
            {
                // Check if a NotificationWindow is already open
                if (!Application.Current.Windows.OfType<NotificationWindow>().Any())
                {
                    NotificationWindow errorNotification = new NotificationWindow("Username or Email already exists.");
                    errorNotification.Show();
                }
                return;
            }

            // Create and save new user
            CreateUser(username, email, passwordHash, fullName, dayOfBirth, phone);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close(); // Close window
        }

        #region Helper Methods

        private bool ValidateInputs(string username, string email, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                // Show notification for empty fields
                if (!Application.Current.Windows.OfType<NotificationWindow>().Any())
                {
                    NotificationWindow emptyFieldNotification = new NotificationWindow("Please fill in all required fields.");
                    emptyFieldNotification.Show();
                }
                return false;
            }

            if (password != confirmPassword)
            {
                // Show notification for password mismatch
                if (!Application.Current.Windows.OfType<NotificationWindow>().Any())
                {
                    NotificationWindow passwordMismatchNotification = new NotificationWindow("Passwords do not match.");
                    passwordMismatchNotification.Show();
                }
                return false;
            }

            return true;
        }

        private bool IsUserExists(string username, string email)
        {
            return _dbContext.Users.Any(u => u.Username == username || u.Email == email);
        }

        private void CreateUser(string username, string email, string passwordHash, string fullName, DateTime? dayOfBirth, string phone)
        {
            var newUser = new User
            {
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                FullName = fullName,
                DayOfBirth = dayOfBirth ?? DateTime.MinValue, // Use default if not provided
                Phone = phone,
                Role = GetRoleForUser(username)
            };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            // Show success notification
            if (!Application.Current.Windows.OfType<NotificationWindow>().Any())
            {
                NotificationWindow successNotification = new NotificationWindow("Registration successful!");
                successNotification.Show();
            }

            LoginWindow newLoginWindow = new LoginWindow();
            newLoginWindow.Show();
            Close();
        }

        private int GetRoleForUser(string username)
        {
            // Default role as User (0); Admin (1) based on conditions
            return username.Equals("admin", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();

                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Convert byte to hexadecimal string
                }

                return builder.ToString();
            }
        }

        #endregion
    }
}
