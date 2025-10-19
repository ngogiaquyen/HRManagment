using HRMANAGMENT2.Controllers;
using HRMANAGMENT2.Models;
using Microsoft.Win32;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HRMANAGMENT2.Dialogs
{
    public partial class SalaryDialog : Window
    {
        private readonly SalaryController _controller;
        private readonly string _id;
        private readonly bool _isReadOnly;
        private readonly string _basePath = @"C:\data";

        public SalaryDialog(SalaryController controller, string id = null, bool isReadOnly = false)
        {
            InitializeComponent();
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _id = id ?? string.Empty;
            _isReadOnly = isReadOnly;
            BtnSave.IsEnabled = !_isReadOnly;
            var employees = _controller.GetEmployeesForComboBox();
            CbEmployeeId.ItemsSource = employees?.DefaultView;
            CbEmployeeId.DisplayMemberPath = "Name";
            CbEmployeeId.SelectedValuePath = "EmployeeId";
            if (!string.IsNullOrEmpty(id))
            {
                LoadSalary(id);
                Title = "Xem/Sửa Lương";
            }
            else
            {
                Title = "Thêm Lương";
            }
        }

        private void LoadSalary(string id)
        {
            var salary = _controller.GetSalaryById(id);
            if (salary != null)
            {
                TxtSalaryId.Text = salary.SalaryId ?? string.Empty;
                CbEmployeeId.SelectedValue = salary.EmployeeId;
                TxtMonthlySalary.Text = salary.MonthlySalary?.ToString("N0") ?? string.Empty;
                TxtPaySlipPath.Text = salary.PaySlipPath ?? string.Empty;
                TxtSalaryIncreaseDecisionPath.Text = salary.SalaryIncreaseDecisionPath ?? string.Empty;
                TxtBankAccount.Text = salary.BankAccount ?? string.Empty;
                TxtInsuranceInfo.Text = salary.InsuranceInfo ?? string.Empty;
                TxtAllowances.Text = salary.Allowances?.ToString("N0") ?? string.Empty;
                TxtBonuses.Text = salary.Bonuses?.ToString("N0") ?? string.Empty;
                TxtLeavePolicy.Text = salary.LeavePolicy ?? string.Empty;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var salary = new Salary
                {
                    SalaryId = TxtSalaryId.Text ?? string.Empty,
                    EmployeeId = CbEmployeeId.SelectedValue?.ToString() ?? string.Empty,
                    MonthlySalary = decimal.TryParse(TxtMonthlySalary.Text, out decimal ms) ? ms : null,
                    PaySlipPath = TxtPaySlipPath.Text ?? string.Empty,
                    SalaryIncreaseDecisionPath = TxtSalaryIncreaseDecisionPath.Text ?? string.Empty,
                    BankAccount = TxtBankAccount.Text ?? string.Empty,
                    InsuranceInfo = TxtInsuranceInfo.Text ?? string.Empty,
                    Allowances = decimal.TryParse(TxtAllowances.Text, out decimal al) ? al : null,
                    Bonuses = decimal.TryParse(TxtBonuses.Text, out decimal bo) ? bo : null,
                    LeavePolicy = TxtLeavePolicy.Text ?? string.Empty
                };
                if (string.IsNullOrEmpty(_id))
                {
                    _controller.AddSalary(salary);
                }
                else
                {
                    _controller.UpdateSalary(salary);
                }
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private string GetEmployeeFolderPath(string employeeId, string employeeName)
        {
            if (string.IsNullOrEmpty(employeeId) || string.IsNullOrEmpty(employeeName))
                return string.Empty;

            string safeName = Path.GetInvalidFileNameChars().Aggregate(employeeName, (current, c) => current.Replace(c, '_'));
            string employeeFolder = Path.Combine(_basePath, $"{employeeId}-{safeName}");
            Directory.CreateDirectory(employeeFolder);  // Tạo thư mục nếu chưa tồn tại
            string salaryFolder = Path.Combine(employeeFolder, "luong");
            Directory.CreateDirectory(salaryFolder);  // Tạo thư mục lương nếu chưa tồn tại
            return salaryFolder;
        }

        private string GenerateFileName(string baseName, string originalFileName)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string extension = Path.GetExtension(originalFileName);
            return $"{baseName}_{timestamp}{extension}";
        }

        private void CopyAndSetPath(TextBox textBox, string baseName, string originalFileName)
        {
            string employeeId = CbEmployeeId.SelectedValue?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(employeeId))
            {
                MessageBox.Show("Vui lòng chọn nhân viên trước khi chọn file.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lấy tên nhân viên từ SelectedItem
            string employeeName = string.Empty;
            if (CbEmployeeId.SelectedItem is DataRowView rowView)
            {
                employeeName = rowView["Name"]?.ToString() ?? string.Empty;
            }

            string destFolder = GetEmployeeFolderPath(employeeId, employeeName);
            if (string.IsNullOrEmpty(destFolder))
                return;

            string newFileName = GenerateFileName(baseName, originalFileName);
            string destPath = Path.Combine(destFolder, newFileName);

            try
            {
                File.Copy(originalFileName, destPath, true);  // Copy và overwrite nếu tồn tại
                textBox.Text = destPath;  // Lưu đường dẫn mới
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi copy file: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                try
                {
                    Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể mở file: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("File không tồn tại.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Phiếu lương
        private void BtnSelectPaySlip_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn phiếu lương"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtPaySlipPath, "phieu_luong", openFileDialog.FileName);
            }
        }

        private void BtnOpenPaySlip_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtPaySlipPath.Text);
        }

        // Thông tin BH
        private void BtnSelectInsuranceInfo_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn thông tin bảo hiểm"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtInsuranceInfo, "thong_tin_bao_hiem", openFileDialog.FileName);
            }
        }

        private void BtnOpenInsuranceInfo_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtInsuranceInfo.Text);
        }

        // Chính sách nghỉ phép
        private void BtnSelectLeavePolicy_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn chính sách nghỉ phép"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtLeavePolicy, "chinh_sach_nghi_phep", openFileDialog.FileName);
            }
        }

        private void BtnOpenLeavePolicy_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtLeavePolicy.Text);
        }
    }
}