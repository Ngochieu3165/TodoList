using BusinessObjects;
using DataAccessLayer;
using Repositories;
using Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace WPFApp
{
    public partial class NewTaskWindow : Window
    {
        private readonly ITaskService _taskService;
        private readonly int _teamId;
        public delegate void TaskAddedEventHandler(object sender, EventArgs e);
        public event TaskAddedEventHandler TaskAdded;
        // Parameterized constructor
        public NewTaskWindow(int teamId)
        {
            InitializeComponent();
            this.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseLeftButtonDown);
            _taskService = new TaskService();
            _teamId = teamId; // Store the team ID
        }
        // Event handler for the Save button
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TitleTextBox.Text) ||
                    string.IsNullOrWhiteSpace(DescriptionTextbox.Text))
                {
                    NotificationWindow notificationWindow = new NotificationWindow("Please fill in all fields.");
                    notificationWindow.Show();
                    return;
                }

                var newToDo = new ToDo
                {
                    Title = TitleTextBox.Text,
                    Description = DescriptionTextbox.Text,
                    DueDate = PeriodDatePicker.SelectedDate ?? DateTime.Now,
                    IsCompleted = false
                };

                _taskService.AddToDoForTeam(_teamId, newToDo);

                // Gọi sự kiện TaskAdded, truyền task mới tạo vào
                TaskAdded?.Invoke(this, new TaskAddedEventArgs(newToDo));

                NotificationWindow notification = new NotificationWindow("Task added successfully");
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
