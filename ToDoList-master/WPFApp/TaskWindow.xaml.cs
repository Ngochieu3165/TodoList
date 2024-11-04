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
    public partial class TaskWindow : Window
    {
        private readonly ITeamService _teamService;
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        private int _currentTaskID;
        private int _currentTeamID;

        // Constructor không tham số, khởi tạo với teamId mặc định là -1
        public TaskWindow() : this(new ToDoService(new ToDoRepository(new ToDoListContext())), -1)
        {
        }

        // Constructor với tham số cho service và teamId
        public TaskWindow(IToDoService toDoService, int teamId)
        {
            InitializeComponent();
            _toDoService = toDoService ?? throw new ArgumentNullException(nameof(toDoService));
            _currentTeamID = teamId; // Lưu teamId
            LoadTeamTasks(_currentTeamID); // Tải danh sách tác vụ cho teamId đã được truyền vào
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
            this.Close(); // Đóng cửa sổ
        }

        public void LoadTeamTasks(int teamID)
        {
            try
            {
                IEnumerable<ToDo> tasks = _toDoService.GetToDosForTeam(teamID);
                TaskListView.ItemsSource = tasks;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tasks: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TaskListItem_TaskSelected(object sender, TaskSelectedEventArgs e)
        {
            // Cập nhật dữ liệu cho Task Viewer
            TaskTitleTextBlock.Text = e.Title; // TextBlock cho tiêu đề task

            // Gán nội dung cho TextBlock bên trong ScrollViewer
            var taskDescriptionTextBlock = TaskDescriptionScrollViewer.Content as TextBlock;
            if (taskDescriptionTextBlock != null)
            {
                taskDescriptionTextBlock.Text = e.Description;
            }

            DueDateTextBlock.Text = $"Due: {e.DueDate}"; // TextBlock cho ngày hết hạn
            _currentTaskID = e.Id;
            _currentTeamID = e.TeamId;
        }

        // Sự kiện xóa task
        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTaskID > 0)
            {
                // Hiển thị cửa sổ xác nhận xóa
                ConfirmationWindow confirmationWindow = new ConfirmationWindow();
                confirmationWindow.Owner = this; // Thiết lập chủ sở hữu của cửa sổ xác nhận là TaskWindow
                confirmationWindow.ShowDialog(); // Hiển thị cửa sổ dưới dạng dialog

                // Kiểm tra nếu người dùng xác nhận xóa
                if (confirmationWindow.IsConfirmed)
                {
                    try
                    {
                        _toDoService.DeleteToDoForTeam(_currentTeamID, _currentTaskID); // Gọi service để xóa task

                        // Hiển thị thông báo thành công qua NotificationWindow
                        NotificationWindow notification = new NotificationWindow("Task deleted successfully!");
                        notification.ShowDialog();

                        // Xóa dữ liệu hiển thị trên Task Viewer
                        TaskTitleTextBlock.Text = string.Empty; // Xóa tiêu đề task
                        var taskDescriptionTextBlock = TaskDescriptionScrollViewer.Content as TextBlock;
                        if (taskDescriptionTextBlock != null)
                        {
                            taskDescriptionTextBlock.Text = string.Empty; // Xóa nội dung task
                        }
                        DueDateTextBlock.Text = string.Empty; // Xóa ngày hết hạn

                        // Reset ID của task hiện tại về 0
                        _currentTaskID = 0;
                        // Cập nhật danh sách tasks sau khi xóa
                        LoadTeamTasks(_currentTeamID);
                    }
                    catch (Exception ex)
                    {
                        // Hiển thị thông báo lỗi qua NotificationWindow
                        NotificationWindow notification = new NotificationWindow($"Error deleting task: {ex.Message}");
                        notification.ShowDialog();
                    }
                }
            }
            else
            {
                // Hiển thị thông báo khi không có task được chọn
                NotificationWindow notification = new NotificationWindow("No task selected for deletion.");
                notification.ShowDialog();
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTitle = TaskSearchBox.Text.Trim();

            if (!string.IsNullOrEmpty(searchTitle))
            {
                try
                {
                    IEnumerable<ToDo> tasks = await _toDoService.GetToDoByTitleAsync(searchTitle, _currentTeamID);
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
            //NewTaskWindow newTaskWindow = new NewTaskWindow(_toDoService, _currentTeamID);

            //// Lắng nghe sự kiện TaskAdded để cập nhật giao diện task detail
            //newTaskWindow.TaskAdded += (s, args) =>
            //{
            //    var taskArgs = args as TaskAddedEventArgs;
            //    if (taskArgs != null)
            //    {
            //        var newTask = taskArgs.NewTask;

            //        // Cập nhật task detail UI
            //        TaskTitleTextBlock.Text = newTask.Title;
            //        var taskDescriptionTextBlock = TaskDescriptionScrollViewer.Content as TextBlock;
            //        if (taskDescriptionTextBlock != null)
            //        {
            //            taskDescriptionTextBlock.Text = newTask.Description;
            //        }
            //        DueDateTextBlock.Text = $"Due: {newTask.DueDate}";
            //        _currentTaskID = newTask.Id;
            //        _currentTeamID = _currentTeamID; // Giữ teamId hiện tại
            //    }

            //    // Tải lại danh sách tasks
            //    LoadTeamTasks(_currentTeamID);
            //};

            //newTaskWindow.ShowDialog();
        }

        private void UpdateTaskButton_Click(object sender, RoutedEventArgs e)
        {
            //if (_currentTaskID > 0)
            //{
            //    UpdateTask updateWindow = new UpdateTask(_toDoService, _currentTeamID, _currentTaskID);
            //    updateWindow.TaskUpdated += (s, args) =>
            //    {
            //        LoadTeamTasks(_currentTeamID);
            //        var updateTask = _toDoService.GetToDoDetails(_currentTeamID, _currentTaskID);
            //        if (updateTask != null)
            //        {
            //            TaskTitleTextBlock.Text = updateTask.Title;
            //            var taskDescriptionBlock = TaskDescriptionScrollViewer.Content as TextBlock;
            //            if (taskDescriptionBlock != null)
            //            {
            //                taskDescriptionBlock.Text = updateTask.Description;
            //            }
            //            DueDateTextBlock.Text = $"Due: {updateTask.DueDate}";
            //        }
            //    };
            //    updateWindow.ShowDialog();
            //}
            //else
            //{
            //    NotificationWindow notification = new NotificationWindow("Please choose a task before pressing update.");
            //    notification.ShowDialog();
            //}
        }

        private void TaskSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchTitle = TaskSearchBox.Text.Trim();
            if (string.IsNullOrEmpty(searchTitle))
            {
                LoadTeamTasks(_currentTeamID);
            }
        }

        private void BinButton_Click(object sender, RoutedEventArgs e)
        {

            //    var trashWindow = new TrashWindow(_toDoService, _currentTeamID); 
            //    trashWindow.Show(); 
        }

        private void ShowTeamsButton_Click(object sender, RoutedEventArgs e)
        {
            
            var context = new ToDoListContext();

           
            IUserRepository userRepository = new UserRepository(context);
            ITeamRepository teamRepository = new TeamRepository(context); 

         
            IUserService userService = new UserService(userRepository);
            ITeamService teamService = new TeamService(teamRepository); 
            IToDoService todoService = new ToDoService(new ToDoRepository(context)); 

            int currentTeamId = _currentTeamID;

            var teamsWindow = new TeamWindow(currentTeamId);
            teamsWindow.Show();
        }







        private void CheckStateButton_Click(object sender, RoutedEventArgs e)
        {
            //if (_currentTaskID > 0)
            //{
            //    var updateCompletionWindow = new UpdateTaskCompletion(_toDoService, _currentTeamID, _currentTaskID);

            //    // Subscribe to the event
            //    updateCompletionWindow.TaskCompletionUpdated += (s, args) =>
            //    {
            //        LoadTeamTasks(_currentTeamID); // Reload the task list
            //    };

            //    updateCompletionWindow.ShowDialog();
            //}
            //else
            //{
            //    NotificationWindow notification = new NotificationWindow("Please select a task before checking the state.");
            //    notification.ShowDialog();
            //}
        }

    }

}
