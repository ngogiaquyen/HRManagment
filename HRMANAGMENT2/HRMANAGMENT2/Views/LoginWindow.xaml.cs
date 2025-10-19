using System;
using System.Windows;
using HRMANAGMENT2.Views;

namespace HRMANAGMENT2.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = TxtUsername.Text?.Trim();
            string password = PwbPassword.Password;

            // Kiểm tra thông tin đăng nhập (hardcode cho demo)
            if (username == "admin" && password == "123456")
            {
                // Đăng nhập thành công, mở MainWindow
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();  // Đóng LoginWindow
            }
            else
            {
                // Thông báo lỗi
                MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Lỗi đăng nhập",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                TxtUsername.Focus();  // Focus lại vào username
            }
        }
    }
}