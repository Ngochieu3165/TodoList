using BusinessObjects;
using DataAccessLayer;
using Repositories;
using Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace WPFApp
{
    public partial class NewTeamWindow : Window
    {
        private readonly ITeamService _teamService;
        private readonly int _loggedInUserID; // Lưu người dùng đã đăng nhập
        public delegate void TeamAddedEventHandler(object sender, EventArgs e);
        public event TeamAddedEventHandler TeamAdded;
        // Parameterized constructor
        public NewTeamWindow(ITeamService teamService, int loggedInUserID)
        {
            InitializeComponent();
            this.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseLeftButtonDown);
            _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
            _loggedInUserID = loggedInUserID;
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TeamNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(DesciptionTextBox.Text))
                {
                    NotificationWindow notificationWindow = new NotificationWindow("Please fill in all fields.");
                    notificationWindow.Show();
                    return;
                }

                var newTeam = new Team
                {
                    Name = TeamNameTextBox.Text,
                    Description = DesciptionTextBox.Text,
                    AdminUserId = _loggedInUserID,
                    DeletedAt = null,
                };
                var existingTeam = _teamService.GetAllTeams()
                                      .FirstOrDefault(t => t.Name.Equals(newTeam.Name, StringComparison.OrdinalIgnoreCase) && t.DeletedAt == null);

                if (existingTeam != null)
                {
                    NotificationWindow notification = new NotificationWindow("A team with the same name already exists.");
                    notification.Show();
                    return;
                }

                _teamService.CreateTeam(newTeam, newTeam.AdminUserId);

                // Gọi sự kiện TeamAdded, truyền team mới tạo vào
                TeamAdded?.Invoke(this, new TeamAddedEventArgs(newTeam));

                NotificationWindow successNotification = new NotificationWindow("Team added successfully");
                successNotification.Show();
                Close();
            }
            catch (Exception ex)
            {
                NotificationWindow notification = new NotificationWindow($"Error: {ex.Message}");
                notification.Show();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void DesciptionTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
