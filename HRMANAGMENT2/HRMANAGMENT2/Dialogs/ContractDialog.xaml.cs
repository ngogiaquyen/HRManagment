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
    public partial class ContractDialog : Window
    {
        private readonly ContractController _controller;
        private readonly string _id;
        private readonly bool _isReadOnly;
        private readonly string _basePath = @"C:\data";

        public ContractDialog(ContractController controller, string id = null, bool isReadOnly = false)
        {
            InitializeComponent();
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _id = id ?? string.Empty;
            _isReadOnly = isReadOnly;
            BtnSave.IsEnabled = !_isReadOnly;
            var employees = _controller.GetEmployeesForComboBox();
            CbEmployeeId.ItemsSource = employees?.DefaultView ?? null;
            CbEmployeeId.DisplayMemberPath = "Name";
            CbEmployeeId.SelectedValuePath = "EmployeeId";
            if (!string.IsNullOrEmpty(id))
            {
                LoadContract(id);
                Title = "Xem/Sửa Hợp đồng";
            }
            else
            {
                Title = "Thêm Hợp đồng";
            }
        }

        private void LoadContract(string id)
        {
            var contract = _controller.GetContractById(id);
            if (contract != null)
            {
                TxtContractId.Text = contract.ContractId ?? string.Empty;
                CbEmployeeId.SelectedValue = contract.EmployeeId;
                DpStartDate.SelectedDate = contract.StartDate;
                DpEndDate.SelectedDate = contract.EndDate;
                TxtContractType.Text = contract.ContractType ?? string.Empty;
                TxtContractAnnexPath.Text = contract.ContractAnnexPath ?? string.Empty;
                TxtConfidentialityAgreementPath.Text = contract.ConfidentialityAgreementPath ?? string.Empty;
                TxtNonCompeteAgreementPath.Text = contract.NonCompeteAgreementPath ?? string.Empty;
                TxtAppointmentDecisionPath.Text = contract.AppointmentDecisionPath ?? string.Empty;
                TxtSalaryIncreaseDecisionPath.Text = contract.SalaryIncreaseDecisionPath ?? string.Empty;
                TxtRewardDecisionPath.Text = contract.RewardDecisionPath ?? string.Empty;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var contract = new Contract
                {
                    ContractId = TxtContractId.Text ?? string.Empty,
                    EmployeeId = CbEmployeeId.SelectedValue?.ToString() ?? string.Empty,
                    StartDate = DpStartDate.SelectedDate,
                    EndDate = DpEndDate.SelectedDate,
                    ContractType = TxtContractType.Text ?? string.Empty,
                    ContractAnnexPath = TxtContractAnnexPath.Text ?? string.Empty,
                    ConfidentialityAgreementPath = TxtConfidentialityAgreementPath.Text ?? string.Empty,
                    NonCompeteAgreementPath = TxtNonCompeteAgreementPath.Text ?? string.Empty,
                    AppointmentDecisionPath = TxtAppointmentDecisionPath.Text ?? string.Empty,
                    SalaryIncreaseDecisionPath = TxtSalaryIncreaseDecisionPath.Text ?? string.Empty,
                    RewardDecisionPath = TxtRewardDecisionPath.Text ?? string.Empty
                };
                if (string.IsNullOrEmpty(_id))
                {
                    _controller.AddContract(contract);
                }
                else
                {
                    _controller.UpdateContract(contract);
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
            string contractFolder = Path.Combine(employeeFolder, "hop_dong");
            Directory.CreateDirectory(contractFolder);  // Tạo thư mục nếu chưa tồn tại
            return contractFolder;
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

        private void BtnSelectContractAnnex_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn phụ lục hợp đồng"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtContractAnnexPath, "phu_luc_hop_dong", openFileDialog.FileName);
            }
        }

        private void BtnOpenContractAnnex_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtContractAnnexPath.Text);
        }

        private void BtnSelectConfidentiality_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn thỏa thuận bảo mật"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtConfidentialityAgreementPath, "thoa_thuan_bao_mat", openFileDialog.FileName);
            }
        }

        private void BtnOpenConfidentiality_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtConfidentialityAgreementPath.Text);
        }

        private void BtnSelectNonCompete_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn thỏa thuận không cạnh tranh"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtNonCompeteAgreementPath, "thoa_thuan_khong_canh_tranh", openFileDialog.FileName);
            }
        }

        private void BtnOpenNonCompete_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtNonCompeteAgreementPath.Text);
        }

        private void BtnSelectAppointment_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn quyết định bổ nhiệm"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtAppointmentDecisionPath, "quyet_dinh_bo_nhiem", openFileDialog.FileName);
            }
        }

        private void BtnOpenAppointment_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtAppointmentDecisionPath.Text);
        }

        private void BtnSelectSalaryIncrease_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn quyết định tăng lương"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtSalaryIncreaseDecisionPath, "quyet_dinh_tang_luong", openFileDialog.FileName);
            }
        }

        private void BtnOpenSalaryIncrease_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtSalaryIncreaseDecisionPath.Text);
        }

        private void BtnSelectReward_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn quyết định thưởng"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtRewardDecisionPath, "quyet_dinh_thuong", openFileDialog.FileName);
            }
        }

        private void BtnOpenReward_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtRewardDecisionPath.Text);
        }
    }
}