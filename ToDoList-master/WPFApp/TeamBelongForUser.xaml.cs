using System;
using BusinessObjects;
using Repositories;
using Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DataAccessLayer;
using System.Windows.Forms;

namespace WPFApp
{
    public partial class TeamBelongForUser : Window
    {
        private readonly ITeamService _teamService;
        private readonly int _loggedInUserID;
        public ObservableCollection<ToDo> DeletedTodos { get; set; }

        public TeamBelongForUser(int loggedInUserID)
        {
            InitializeComponent();
            _teamService = new TeamService(new TeamRepository(new ToDoListContext()));
            _loggedInUserID = loggedInUserID;
            LoadTeams(_loggedInUserID);
        }
        // Các button thao tác với cửa sổ
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized; // Giảm cửa sổ
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized; // Max
            else
                this.WindowState = WindowState.Normal; // Trở lại trạng thái bình thường
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var teamsWindow = new TeamWindow(_loggedInUserID);
            teamsWindow.Show();
            this.Close(); // Đóng cửa sổ
        }
        public void LoadTeams(int userID)
        {
            var teams = _teamService.GetAllTeams()
                  .Where(t => t.Members.Any(m => m.UserId == userID) && t.DeletedAt == null)
                  .ToList();
            TeamsDataGrid.ItemsSource = teams;

        }
        private void ViewTasks_Button_Click(object sender, RoutedEventArgs e)
        {
            if (TeamsDataGrid.SelectedItem is Team selectedTeam)
            {
                TaskWindow2 taskWindow = new TaskWindow2(selectedTeam.TeamId, _loggedInUserID);
                taskWindow.Show();
                this.Close();
            }    
        }

    }
}
