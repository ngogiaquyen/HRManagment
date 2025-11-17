using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HRMANAGMENT2.Views; // Import Views2 (thay nếu namespace khác)

namespace HRMANAGMENT2
{
    public partial class MainWindow : Window
    {
        private bool _isCollapsed = false;

        public MainWindow()
        {
            InitializeComponent();
            LoadPage("EmployeePage"); // Trang mặc định
        }

        private void LoadPage(string pageName)
        {
            UserControl page = pageName switch
            {
                "EmployeePage" => new EmployeePage(),
                "ContractPage" => new ContractPage(),
                "AttendancePage" => new AttendancePage(),
                "RecruitmentPage" => new RecruitmentPage(),
                "SalaryPage" => new SalaryPage(),
                "TrainingPage" => new TrainingPage(),
                "DisciplinePage" => new DisciplinePage(),
                _ => new EmployeePage()
            };
            MainContent.Content = page;

            // Highlight active button
            var buttons = new[] { BtnEmployees, BtnContracts, BtnAttendance, BtnRecruitment, BtnSalary, BtnTraining, BtnDiscipline };
            foreach (var btn in buttons)
            {
                btn.Background = (btn.Name.Contains(pageName.Replace("Page", ""))) ? new SolidColorBrush(Colors.LightBlue) : null;
            }
        }

        private void BtnEmployees_Click(object sender, RoutedEventArgs e) => LoadPage("EmployeePage");
        private void BtnContracts_Click(object sender, RoutedEventArgs e) => LoadPage("ContractPage");
        private void BtnAttendance_Click(object sender, RoutedEventArgs e) => LoadPage("AttendancePage");
        private void BtnRecruitment_Click(object sender, RoutedEventArgs e) => LoadPage("RecruitmentPage");
        private void BtnSalary_Click(object sender, RoutedEventArgs e) => LoadPage("SalaryPage");
        private void BtnTraining_Click(object sender, RoutedEventArgs e) => LoadPage("TrainingPage");
        private void BtnDiscipline_Click(object sender, RoutedEventArgs e) => LoadPage("DisciplinePage");

        private void HamburgerBtn_Click(object sender, RoutedEventArgs e)
        {
            _isCollapsed = !_isCollapsed;
            SidebarColumn.Width = _isCollapsed ? new GridLength(60) : new GridLength(250);

            var buttons = new[] { BtnEmployees, BtnContracts, BtnAttendance, BtnRecruitment, BtnSalary, BtnTraining, BtnDiscipline };
            foreach (var btn in buttons)
            {
                if (_isCollapsed)
                {
                    btn.Content = btn.Tag.ToString().Split(' ')[0]; // Chỉ icon
                    btn.Padding = new Thickness(10, 0, 10, 0); // Fix: 4 params
                }
                else
                {
                    btn.Content = btn.Tag; // Text đầy đủ
                    btn.Padding = new Thickness(20, 0, 20, 0); // Fix: 4 params
                }
            }
            HamburgerBtn.Content = _isCollapsed ? "☰" : "✕";
        }

        private void ToggleDarkMode_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Dark mode toggle - Implement later!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}