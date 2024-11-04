using System.Windows;

namespace WPFApp
{
    public partial class ConfirmationWindow : Window
    {
        public bool IsConfirmed { get; private set; }

        public ConfirmationWindow()
        {
            InitializeComponent();
            IsConfirmed = false; // Mặc định là không xác nhận
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = true; // Đã xác nhận
            this.Close(); // Đóng cửa sổ
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            IsConfirmed = false; // Không xác nhận
            this.Close(); // Đóng cửa sổ
        }
    }
}
