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
    public partial class RecruitmentDialog : Window
    {
        private readonly RecruitmentController _controller;
        private readonly string _id;
        private readonly bool _isReadOnly;
        private readonly string _basePath = @"C:\data";

        public RecruitmentDialog(RecruitmentController controller, string id = null, bool isReadOnly = false)
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
                LoadRecruitment(id);
                Title = "Xem/Sửa Tuyển dụng";
            }
            else
            {
                Title = "Thêm Tuyển dụng";
            }
        }

        private void LoadRecruitment(string id)
        {
            var recruitment = _controller.GetRecruitmentById(id);
            if (recruitment != null)
            {
                TxtRecruitmentId.Text = recruitment.RecruitmentId ?? string.Empty;
                CbEmployeeId.SelectedValue = recruitment.EmployeeId;
                TxtJobApplicationPath.Text = recruitment.JobApplicationPath ?? string.Empty;
                TxtResumePath.Text = recruitment.ResumePath ?? string.Empty;
                TxtDegreesPath.Text = recruitment.DegreesPath ?? string.Empty;
                TxtHealthCheckPath.Text = recruitment.HealthCheckPath ?? string.Empty;
                TxtCVPath.Text = recruitment.CVPath ?? string.Empty;
                TxtReferenceLetterPath.Text = recruitment.ReferenceLetterPath ?? string.Empty;
                TxtInterviewMinutesPath.Text = recruitment.InterviewMinutesPath ?? string.Empty;
                TxtOfferLetterPath.Text = recruitment.OfferLetterPath ?? string.Empty;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var recruitment = new Recruitment
                {
                    RecruitmentId = TxtRecruitmentId.Text ?? string.Empty,
                    EmployeeId = CbEmployeeId.SelectedValue?.ToString() ?? string.Empty,
                    JobApplicationPath = TxtJobApplicationPath.Text ?? string.Empty,
                    ResumePath = TxtResumePath.Text ?? string.Empty,
                    DegreesPath = TxtDegreesPath.Text ?? string.Empty,
                    HealthCheckPath = TxtHealthCheckPath.Text ?? string.Empty,
                    CVPath = TxtCVPath.Text ?? string.Empty,
                    ReferenceLetterPath = TxtReferenceLetterPath.Text ?? string.Empty,
                    InterviewMinutesPath = TxtInterviewMinutesPath.Text ?? string.Empty,
                    OfferLetterPath = TxtOfferLetterPath.Text ?? string.Empty
                };
                if (string.IsNullOrEmpty(_id))
                {
                    _controller.AddRecruitment(recruitment);
                }
                else
                {
                    _controller.UpdateRecruitment(recruitment);
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
            string recruitmentFolder = Path.Combine(employeeFolder, "tuyen_dung");
            Directory.CreateDirectory(recruitmentFolder);  // Tạo thư mục tuyển dụng nếu chưa tồn tại
            return recruitmentFolder;
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

        // Đơn ứng tuyển
        private void BtnSelectJobApplication_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn đơn ứng tuyển"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtJobApplicationPath, "don_ung_tuyen", openFileDialog.FileName);
            }
        }

        private void BtnOpenJobApplication_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtJobApplicationPath.Text);
        }

        // Hồ sơ
        private void BtnSelectResume_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn hồ sơ"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtResumePath, "ho_so", openFileDialog.FileName);
            }
        }

        private void BtnOpenResume_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtResumePath.Text);
        }

        // Bằng cấp
        private void BtnSelectDegrees_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn bằng cấp"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtDegreesPath, "bang_cap", openFileDialog.FileName);
            }
        }

        private void BtnOpenDegrees_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtDegreesPath.Text);
        }

        // Khám sức khỏe
        private void BtnSelectHealthCheck_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn giấy khám sức khỏe"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtHealthCheckPath, "kham_suc_khoe", openFileDialog.FileName);
            }
        }

        private void BtnOpenHealthCheck_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtHealthCheckPath.Text);
        }

        // CV
        private void BtnSelectCV_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn CV"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtCVPath, "cv", openFileDialog.FileName);
            }
        }

        private void BtnOpenCV_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtCVPath.Text);
        }

        // Thư giới thiệu
        private void BtnSelectReferenceLetter_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn thư giới thiệu"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtReferenceLetterPath, "thu_gioi_thieu", openFileDialog.FileName);
            }
        }

        private void BtnOpenReferenceLetter_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtReferenceLetterPath.Text);
        }

        // Biên bản phỏng vấn
        private void BtnSelectInterviewMinutes_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn biên bản phỏng vấn"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtInterviewMinutesPath, "bien_ban_phong_van", openFileDialog.FileName);
            }
        }

        private void BtnOpenInterviewMinutes_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtInterviewMinutesPath.Text);
        }

        // Thư mời làm việc
        private void BtnSelectOfferLetter_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "All files (*.*)|*.*",
                Title = "Chọn thư mời làm việc"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CopyAndSetPath(TxtOfferLetterPath, "thu_moi_lam_viec", openFileDialog.FileName);
            }
        }

        private void BtnOpenOfferLetter_Click(object sender, RoutedEventArgs e)
        {
            OpenFile(TxtOfferLetterPath.Text);
        }
    }
}