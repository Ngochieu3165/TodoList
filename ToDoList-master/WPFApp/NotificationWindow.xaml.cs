using System.Windows;

namespace WPFApp
{
    public partial class NotificationWindow : Window
    {
        public NotificationWindow(string message)
        {
            InitializeComponent();
            NotificationTextBlock.Text = message; // Hiển thị thông báo
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
