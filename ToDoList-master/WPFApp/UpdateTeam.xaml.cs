using BusinessObjects;
using DataAccessLayer;
using Repositories;
using Services;
using System;
using System.Windows;
using System.Windows.Input;
namespace WPFApp
{
    /// <summary>
    /// Interaction logic for UpdateTeam.xaml
    /// </summary>
    public partial class UpdateTeam : Window
    {
        private readonly ITeamService _teamService;
        private readonly int _teamId;

        public delegate void TeamUpdatedEventHandler(object sender, EventArgs e);
        public event TeamUpdatedEventHandler TeamUpdated;

        public UpdateTeam(ITeamService teamService, int teamId)
        {
            InitializeComponent();
            this.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseLeftButtonDown);
            _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
            _teamId = teamId;
            LoadTeamDetails(); // Load the details of the team to be updated
        }

        private void LoadTeamDetails()
        {
            var team = _teamService.GetTeamById(_teamId);
            if (team != null)
            {
                TeamNameTextBox.Text = team.Name;
                StatusTextBox.Text = team.Status.ToString();
                AdminIdTextBox.Text = team.AdminUserId.ToString();
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TeamNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(StatusTextBox.Text)  ||
                    string.IsNullOrWhiteSpace(AdminIdTextBox.Text))

                {
                    NotificationWindow notificationWindow = new NotificationWindow("Please fill in all fields.");
                    notificationWindow.Show();
                    return;
                }
                var updateTeam = _teamService.GetTeamById(_teamId);
                updateTeam.Name = TeamNameTextBox.Text;
                updateTeam.Status = (TeamStatus)Enum.Parse(typeof(TeamStatus), StatusTextBox.Text);
                updateTeam.AdminUserId = int.Parse(AdminIdTextBox.Text);
                _teamService.UpdateTeam(updateTeam);
                TeamUpdated?.Invoke(this, new TeamAddedEventArgs(updateTeam));
                NotificationWindow notification = new NotificationWindow("Team updated successfully");
                notification.Show();
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
    }
}
