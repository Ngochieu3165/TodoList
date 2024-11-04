using BusinessObjects;
using DataAccessLayer;
using Repositories;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for TrashWindow.xaml
    /// </summary>
    public partial class TrashWindow : Window
    {
        private readonly IToDoService _todoService;
        private readonly int _loggedInUserID;
        private int _currentTeamID;

        public TrashWindow(int loggedInUserID, int currentTeamID)
        {
            InitializeComponent();
            _todoService = new ToDoService(new ToDoRepository(new ToDoListContext()));
            _loggedInUserID = loggedInUserID;
            _currentTeamID = currentTeamID;
            LoadToDoBins(_currentTeamID);
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
            TaskWindow2 taskWindow = new TaskWindow2(_currentTeamID, _loggedInUserID);
            taskWindow.Show();
            this.Close(); // Đóng cửa sổ
        }

        public void LoadToDoBins(int currentTeamID)
        {
            var todos = _todoService.GetDeletedTodos(currentTeamID);
            TrashDataGrid.ItemsSource = todos;
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (TrashDataGrid.SelectedItem is ToDo selectedToDo)
            {
                if (System.Windows.MessageBox.Show("Are you sure?", "Confirm Restore", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _todoService.RestoreToDo(selectedToDo.Id, _currentTeamID);
                    LoadToDoBins(_loggedInUserID);
                }
            }
           
        }
    }
}
