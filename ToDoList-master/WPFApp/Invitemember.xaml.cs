using BusinessObjects;
using DataAccessLayer;
using Microsoft.VisualBasic.ApplicationServices;
using Repositories;
using Services;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
namespace WPFApp
{
    /// <summary>
    /// Interaction logic for Addmember.xaml
    /// </summary>
    public partial class Invitemember : Window
    {
        private readonly ITeamService _teamService;
        private readonly int _teamId;
        public delegate void MemberAddedEventHandler(object sender, EventArgs e);
        public event MemberAddedEventHandler MemberAdded;
        // Parameterized constructor
        public Invitemember(ITeamService teamService, int teamId)
        {
            InitializeComponent();
            this.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseLeftButtonDown);
            _teamService = teamService ?? throw new ArgumentNullException(nameof(teamService));
            _teamId = teamId; // Store the team ID
            LoadUsers(_teamId);

        }
        public void LoadUsers(int teamID)
        {
            try
            {
                IEnumerable<BusinessObjects.User> users = _teamService.GetUsersOutTeam(teamID);
                UserListView.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tasks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        // Event handler for the Save button
        private void InviteMemberInTeam(object sender, RoutedEventArgs e)
        {
            // Lấy thông tin của nút đã được nhấn
            Button inviteButton = sender as Button;
            if (inviteButton != null)
            {
                // Lấy thông tin người dùng từ CommandParameter
                var user = inviteButton.CommandParameter as BusinessObjects.User;
                if (user != null)
                {
                    try
                    {
                        // Gọi phương thức để thêm người dùng vào đội
                        _teamService.AddMemberInTeam(_teamId, user);

                        // Hiển thị thông báo thành công
                        NotificationWindow notification = new NotificationWindow($"User {user.Username} invited to the team.");
                        notification.ShowDialog();
                        MemberAdded?.Invoke(this, new MemberAddedEventArgs(user));
                        // Đổi màu nút sau khi thêm thành công
                        inviteButton.Background = new SolidColorBrush(Colors.Gray); // Hoặc màu bạn muốn
                        inviteButton.IsEnabled = false;
                    }
                    catch (Exception ex)
                    {
                        // Hiển thị thông báo lỗi nếu có vấn đề xảy ra
                        MessageBox.Show($"Error inviting user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
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
