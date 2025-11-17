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
    public partial class TrainingDialog : Window
    {
        private readonly TrainingController _controller;
        private readonly string _id;
        private readonly bool _isReadOnly;
        private readonly string _basePath = @"C:\data";

        public TrainingDialog(TrainingController controller, string id = null, bool isReadOnly = false)
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
                LoadTraining(id);
                Title = "Xem/Sửa Đào tạo";
            }
            else
            {
                Title = "Thêm Đào tạo";
            }
        }

        private void LoadTraining(string id)
        {
            var training = _controller.GetTrainingById(id);
            if (training != null)
            {
                TxtTrainingId.Text = training.TrainingId ?? string.Empty;
                CbEmployeeId.SelectedValue = training.EmployeeId;
                TxtTrainingPlanPath.Text = training.TrainingPlanPath ?? string.Empty;
                TxtCertificatePath.Text = training.CertificatePath ?? string.Empty;
                TxtEvaluationPath.Text = training.EvaluationPath ?? string.Empty;
                TxtCareerPath.Text = training.CareerPath ?? string.Empty;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var training = new Training
                {
                    TrainingId = TxtTrainingId.Text ?? string.Empty,
                    EmployeeId = CbEmployeeId.SelectedValue?.ToString() ?? string.Empty,
                    TrainingPlanPath = TxtTrainingPlanPath.Text ?? string.Empty,
                    CertificatePath = TxtCertificatePath.Text ?? string.Empty,
                    EvaluationPath = TxtEvaluationPath.Text ?? string.Empty,
                    CareerPath = TxtCareerPath.Text ?? string.Empty
                };
                if (string.IsNullOrEmpty(_id))
                {
                    _controller.AddTraining(training);
                }
                else
                {
                    _controller.UpdateTraining(training);
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
            string trainingFolder = Path.Combine(employeeFolder, "dao_tao");
            Directory.CreateDirectory(trainingFolder);  // Tạo thư mục đào tạo nếu chưa tồn tại
            return trainingFolder;
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

        // Kế hoạch ĐT
        private void BtnSelectTrainingPlan_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn kế hoạch đào tạo"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtTrainingPlanPath, "ke_hoach_dao_tao", openFileDialog.FileName);
            }
        }

        private void BtnOpenTrainingPlan_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtTrainingPlanPath.Text);
        }

        // Chứng chỉ
        private void BtnSelectCertificate_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn chứng chỉ"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtCertificatePath, "chung_chi", openFileDialog.FileName);
            }
        }

        private void BtnOpenCertificate_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtCertificatePath.Text);
        }

        // Đánh giá
        private void BtnSelectEvaluation_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn đánh giá"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtEvaluationPath, "danh_gia", openFileDialog.FileName);
            }
        }

        private void BtnOpenEvaluation_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtEvaluationPath.Text);
        }

        // Lộ trình nghề
        private void BtnSelectCareerPath_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn lộ trình nghề"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtCareerPath, "lo_trinh_nghe", openFileDialog.FileName);
            }
        }

        private void BtnOpenCareerPath_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtCareerPath.Text);
        }
    }
}