using BusinessObjects;
using Microsoft.VisualBasic;
using Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for UpdateTask.xaml
    /// </summary>
    public partial class UpdateTask : Window
    {
        private readonly ITaskService _taskService;
        private readonly int _teamId;
        private readonly int _todoId; // ID of the ToDo to be updated

        public delegate void TaskUpdatedEventHandler(object sender, EventArgs e);
        public event TaskUpdatedEventHandler TaskUpdated;

        public UpdateTask(int teamId, int todoId)
        {
            InitializeComponent();
            this.MouseLeftButtonDown += new MouseButtonEventHandler(Window_MouseLeftButtonDown);
            _taskService = new TaskService();
            _teamId = teamId;
            _todoId = todoId;

            LoadTaskDetails(); // Load the details of the task to be updated
        }

        private void LoadTaskDetails()
        {
            var todo = _taskService.GetToDoById(_teamId, _todoId);
            if (todo != null)
            {
                TitleTextBox.Text = todo.Title;
                DescriptionTextbox.Text = todo.Description;
                PeriodDatePicker.SelectedDate = todo.DueDate;
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
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

                var updateToDo = new ToDo
                {
                    Id = _todoId, // Set the ID of the task being updated
                    Title = TitleTextBox.Text,
                    Description = DescriptionTextbox.Text,
                    DueDate = PeriodDatePicker.SelectedDate ?? DateTime.Now,
                    IsCompleted = false // or set according to your logic
                };

                _taskService.UpdateToDoForTeam(_teamId, updateToDo);
                TaskUpdated?.Invoke(this, EventArgs.Empty);

                NotificationWindow notification = new NotificationWindow("Task updated successfully");
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
