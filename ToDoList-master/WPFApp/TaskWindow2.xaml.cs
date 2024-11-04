using BusinessObjects;
using DataAccessLayer;
using Repositories;
using Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPFApp
{
    public partial class TaskWindow2 : Window
    {
        private readonly ITaskService _taskService;
        private int _currentTaskID;
        private int _currentTeamID;
        private int _currentUserID;
        private bool _isAdmin; // Flag to store admin status

        // Constructor with service and teamId parameters
        public TaskWindow2(int teamId, int userId)
        {
            InitializeComponent();
            _currentTeamID = teamId; // Store teamId
            _currentUserID = userId;
            _taskService = new TaskService();
            _isAdmin = CheckIfAdmin();

            // Check if user is admin
            SetButtonVisibility();

            // Load task list for provided teamId
            LoadTeamTasks(_currentTeamID);
        }

        private bool CheckIfAdmin()
        {
            using var context = new ToDoListContext();
            Team currentTeam = context.Teams.FirstOrDefault(t => t.TeamId == _currentTeamID);
            return currentTeam != null && currentTeam.AdminUserId == _currentUserID;
        }
        private void SetButtonVisibility()
        {
            // Hide or disable buttons if the user is not an admin
            DeleteButton.Visibility = _isAdmin ? Visibility.Visible : Visibility.Collapsed;
            UpdateButton.Visibility = _isAdmin ? Visibility.Visible : Visibility.Collapsed;
            NewTaskButton.Visibility = _isAdmin ? Visibility.Visible : Visibility.Collapsed;
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = this.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckIfAdmin())
            {
                // Người dùng là admin, mở InsideTeam window
                var insideTeamWindow = new InsideTeam(_currentTeamID, _currentUserID);
                insideTeamWindow.Show();
            }
            else
            {
                // Người dùng không phải admin, chuyển hướng về TeamBelongForUser window
                var teamBelongForUserWindow = new TeamBelongForUser(_currentUserID);
                teamBelongForUserWindow.Show();
            }
            this.Close();
        }

        public void LoadTeamTasks(int teamID)
        {
            try
            {
                IEnumerable<ToDo> tasks = _taskService.GetToDosByTeam(teamID);
                TaskListView.ItemsSource = tasks;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tasks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TaskListItem_TaskSelected(object sender, TaskSelectedEventArgs e)
        {
            TaskTitleTextBlock.Text = e.Title;

            var taskDescriptionTextBlock = TaskDescriptionScrollViewer.Content as TextBlock;
            if (taskDescriptionTextBlock != null)
            {
                taskDescriptionTextBlock.Text = e.Description;
            }

            DueDateTextBlock.Text = $"Due: {e.DueDate}";
            _currentTaskID = e.Id;
            _currentTeamID = e.TeamId;
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isAdmin && _currentTaskID > 0)
            {
                // Show confirmation window
                ConfirmationWindow confirmationWindow = new ConfirmationWindow();
                confirmationWindow.Owner = this;
                confirmationWindow.ShowDialog();

                if (confirmationWindow.IsConfirmed)
                {
                    try
                    {
                        _taskService.DeleteToDo(_currentTaskID);
                        NotificationWindow notification = new NotificationWindow("Task deleted successfully!");
                        notification.ShowDialog();

                        TaskTitleTextBlock.Text = string.Empty;
                        var taskDescriptionTextBlock = TaskDescriptionScrollViewer.Content as TextBlock;
                        if (taskDescriptionTextBlock != null)
                        {
                            taskDescriptionTextBlock.Text = string.Empty;
                        }
                        DueDateTextBlock.Text = string.Empty;
                        _currentTaskID = 0;

                        LoadTeamTasks(_currentTeamID);
                    }
                    catch (Exception ex)
                    {
                        NotificationWindow notification = new NotificationWindow($"Error deleting task: {ex.Message}");
                        notification.ShowDialog();
                    }
                }
            }
            else if (!_isAdmin)
            {
                MessageBox.Show("You do not have permission to delete tasks.", "Unauthorized", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTitle = TaskSearchBox.Text.Trim();

            if (!string.IsNullOrEmpty(searchTitle))
            {
                try
                {
                    IEnumerable<ToDo> tasks = _taskService.GetToDoByTitleAsync(searchTitle, _currentTeamID);
                    TaskListView.ItemsSource = tasks;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error searching tasks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                LoadTeamTasks(_currentTeamID);
            }
        }

        private void NewTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isAdmin)
            {
                NewTaskWindow newTaskWindow = new NewTaskWindow(_currentTeamID);

                newTaskWindow.TaskAdded += (s, args) =>
                {
                    var taskArgs = args as TaskAddedEventArgs;
                    if (taskArgs != null)
                    {
                        var newTask = taskArgs.NewTask;

                        TaskTitleTextBlock.Text = newTask.Title;
                        var taskDescriptionTextBlock = TaskDescriptionScrollViewer.Content as TextBlock;
                        if (taskDescriptionTextBlock != null)
                        {
                            taskDescriptionTextBlock.Text = newTask.Description;
                        }
                        DueDateTextBlock.Text = $"Due: {newTask.DueDate}";
                        _currentTaskID = newTask.Id;
                        _currentTeamID = _currentTeamID;
                    }

                    LoadTeamTasks(_currentTeamID);
                };

                newTaskWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("You do not have permission to add new tasks.", "Unauthorized", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isAdmin && _currentTaskID > 0)
            {
                UpdateTask updateWindow = new UpdateTask(_currentTeamID, _currentTaskID);
                updateWindow.TaskUpdated += (s, args) =>
                {
                    LoadTeamTasks(_currentTeamID);
                    var updateTask = _taskService.GetToDoById(_currentTeamID, _currentTaskID);
                    if (updateTask != null)
                    {
                        TaskTitleTextBlock.Text = updateTask.Title;
                        var taskDescriptionBlock = TaskDescriptionScrollViewer.Content as TextBlock;
                        if (taskDescriptionBlock != null)
                        {
                            taskDescriptionBlock.Text = updateTask.Description;
                        }
                        DueDateTextBlock.Text = $"Due: {updateTask.DueDate}";
                    }
                };
                updateWindow.ShowDialog();
            }
            else if (!_isAdmin)
            {
                MessageBox.Show("You do not have permission to update tasks.", "Unauthorized", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TaskSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchTitle = TaskSearchBox.Text.Trim();
            if (string.IsNullOrEmpty(searchTitle))
            {
                LoadTeamTasks(_currentTeamID);
            }
        }

        private void CheckStateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTaskID > 0)
            {
                var updateCompletionWindow = new UpdateTaskCompletion(_currentTeamID, _currentTaskID);

                updateCompletionWindow.TaskCompletionUpdated += (s, args) =>
                {
                    LoadTeamTasks(_currentTeamID);
                };

                updateCompletionWindow.ShowDialog();
            }
            else
            {
                NotificationWindow notification = new NotificationWindow("Please select a task before checking the state.");
                notification.ShowDialog();
            }
        }

        private void BinButton_Click(object sender, RoutedEventArgs e)
        {
            var trash = new TrashWindow(_currentUserID,  _currentTeamID);
            trash.Show();
            this.Close();
        }
    }
}
