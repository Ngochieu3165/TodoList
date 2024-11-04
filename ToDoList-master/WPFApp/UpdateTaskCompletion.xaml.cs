using BusinessObjects;
using Services;
using System;
using System.Windows;

namespace WPFApp
{
    public partial class UpdateTaskCompletion : Window
    {
        private readonly ITaskService _taskService;
        private readonly int _teamId;
        private readonly int _todoId;

        public event EventHandler TaskCompletionUpdated; // Declare an event

        public UpdateTaskCompletion(int teamId, int todoId)
        {
            InitializeComponent();
            _taskService = new TaskService();
            _teamId = teamId;
            _todoId = todoId;

            LoadTaskDetails(); // Load task details
        }

        private void LoadTaskDetails()
        {
            var todo = _taskService.GetToDoById(_teamId, _todoId);
            if (todo != null)
            {
                IsCompletedCheckBox.IsChecked = todo.IsCompleted;
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Update task completion status
                _taskService.UpdateTaskCompletionStatus(_teamId, _todoId, IsCompletedCheckBox.IsChecked ?? false);

                // Raise the event to notify that the task completion status has been updated
                TaskCompletionUpdated?.Invoke(this, EventArgs.Empty);

                NotificationWindow notification = new NotificationWindow("Task completion status updated successfully");
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
    }

}
