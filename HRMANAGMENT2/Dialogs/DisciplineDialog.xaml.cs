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
    public partial class DisciplineDialog : Window
    {
        private readonly DisciplineController _controller;
        private readonly string _id;
        private readonly bool _isReadOnly;
        private readonly string _basePath = @"C:\data";

        public DisciplineDialog(DisciplineController controller, string id = null, bool isReadOnly = false)
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
                LoadDiscipline(id);
                Title = "Xem/Sửa Kỷ luật";
            }
            else
            {
                Title = "Thêm Kỷ luật";
            }
        }

        private void LoadDiscipline(string id)
        {
            var discipline = _controller.GetDisciplineById(id);
            if (discipline != null)
            {
                TxtDisciplineId.Text = discipline.DisciplineId ?? string.Empty;
                CbEmployeeId.SelectedValue = discipline.EmployeeId;
                TxtViolationPath.Text = discipline.ViolationPath ?? string.Empty;
                TxtDisciplinaryDecisionPath.Text = discipline.DisciplinaryDecisionPath ?? string.Empty;
                TxtResignationLetterPath.Text = discipline.ResignationLetterPath ?? string.Empty;
                TxtTerminationDecisionPath.Text = discipline.TerminationDecisionPath ?? string.Empty;
                TxtHandoverPath.Text = discipline.HandoverPath ?? string.Empty;
                TxtLiquidationPath.Text = discipline.LiquidationPath ?? string.Empty;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var discipline = new Discipline
                {
                    DisciplineId = TxtDisciplineId.Text ?? string.Empty,
                    EmployeeId = CbEmployeeId.SelectedValue?.ToString() ?? string.Empty,
                    ViolationPath = TxtViolationPath.Text ?? string.Empty,
                    DisciplinaryDecisionPath = TxtDisciplinaryDecisionPath.Text ?? string.Empty,
                    ResignationLetterPath = TxtResignationLetterPath.Text ?? string.Empty,
                    TerminationDecisionPath = TxtTerminationDecisionPath.Text ?? string.Empty,
                    HandoverPath = TxtHandoverPath.Text ?? string.Empty,
                    LiquidationPath = TxtLiquidationPath.Text ?? string.Empty
                };
                if (string.IsNullOrEmpty(_id))
                {
                    _controller.AddDiscipline(discipline);
                }
                else
                {
                    _controller.UpdateDiscipline(discipline);
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
            string disciplineFolder = Path.Combine(employeeFolder, "ky_luat");
            Directory.CreateDirectory(disciplineFolder);  // Tạo thư mục kỷ luật nếu chưa tồn tại
            return disciplineFolder;
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

        // Hồ sơ vi phạm
        private void BtnSelectViolation_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn hồ sơ vi phạm"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtViolationPath, "ho_so_vi_pham", openFileDialog.FileName);
            }
        }

        private void BtnOpenViolation_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtViolationPath.Text);
        }

        // Quyết định KL
        private void BtnSelectDisciplinaryDecision_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn quyết định kỷ luật"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtDisciplinaryDecisionPath, "quyet_dinh_ky_luat", openFileDialog.FileName);
            }
        }

        private void BtnOpenDisciplinaryDecision_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtDisciplinaryDecisionPath.Text);
        }

        // Thư từ chức
        private void BtnSelectResignationLetter_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn thư từ chức"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtResignationLetterPath, "thu_tu_chuc", openFileDialog.FileName);
            }
        }

        private void BtnOpenResignationLetter_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtResignationLetterPath.Text);
        }

        // Quyết định chấm dứt
        private void BtnSelectTerminationDecision_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn quyết định chấm dứt"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtTerminationDecisionPath, "quyet_dinh_cham_dut", openFileDialog.FileName);
            }
        }

        private void BtnOpenTerminationDecision_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtTerminationDecisionPath.Text);
        }

        // Hồ sơ bàn giao
        private void BtnSelectHandover_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn hồ sơ bàn giao"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtHandoverPath, "ho_so_ban_giao", openFileDialog.FileName);
            }
        }

        private void BtnOpenHandover_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtHandoverPath.Text);
        }

        // Hồ sơ thanh lý
        private void BtnSelectLiquidation_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn hồ sơ thanh lý"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtLiquidationPath, "ho_so_thanh_ly", openFileDialog.FileName);
            }
        }

        private void BtnOpenLiquidation_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtLiquidationPath.Text);
        }
    }
}