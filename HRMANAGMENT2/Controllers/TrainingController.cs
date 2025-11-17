using HRManagementApp.Controllers;
using HRMANAGMENT2.Models;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Windows;

namespace HRMANAGMENT2.Controllers
{
    public class TrainingController
    {
        private readonly DatabaseManager _dbManager;

        public TrainingController()
        {
            _dbManager = new DatabaseManager();
        }

        private string GenerateTrainingId()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Training";
                int count = Convert.ToInt32(_dbManager.ExecuteScalar(query, null));
                return $"TRA{(count + 1):D4}";
            }
            catch
            {
                return $"TRA{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        public void LoadTrainings()
        {
            try
            {
                string query = @"SELECT t.*, (e.Name + ' - ' + e.CCCD + ' - ' + CONVERT(varchar(10), e.DOB, 120)) AS EmployeeInfo
                                 FROM Training t
                                 LEFT JOIN Employee e ON t.EmployeeId = e.EmployeeId";
                DataTable dt = _dbManager.GetDataTable(query, null);
                // Load data
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách đào tạo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public DataTable GetTrainingsData()
        {
            string query = @"SELECT t.*, (e.Name + ' - ' + e.CCCD + ' - ' + CONVERT(varchar(10), e.DOB, 120)) AS EmployeeInfo
                             FROM Training t
                             LEFT JOIN Employee e ON t.EmployeeId = e.EmployeeId";
            return _dbManager.GetDataTable(query, null);
        }

        public Training GetTrainingById(string trainingId)
        {
            try
            {
                string query = "SELECT * FROM Training WHERE TrainingId = @TrainingId";
                SqlParameter[] parameters = { new SqlParameter("@TrainingId", trainingId) };
                DataTable dt = _dbManager.GetDataTable(query, parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Training
                    {
                        TrainingId = row["TrainingId"].ToString(),
                        EmployeeId = row["EmployeeId"].ToString(),
                        TrainingPlanPath = row["TrainingPlanPath"]?.ToString(),
                        CertificatePath = row["CertificatePath"]?.ToString(),
                        EvaluationPath = row["EvaluationPath"]?.ToString(),
                        CareerPath = row["CareerPath"]?.ToString()
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy đào tạo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public DataTable GetEmployeesForComboBox()
        {
            try
            {
                string query = "SELECT EmployeeId, Name FROM Employee";
                return _dbManager.GetDataTable(query, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách nhân viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return new DataTable();
            }
        }

        public void AddTraining(Training training)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(training.TrainingId))
                {
                    training.TrainingId = GenerateTrainingId();
                }
                string query = @"INSERT INTO Training (TrainingId, EmployeeId, TrainingPlanPath, CertificatePath, EvaluationPath, CareerPath)
                                VALUES (@TrainingId, @EmployeeId, @TrainingPlanPath, @CertificatePath, @EvaluationPath, @CareerPath)";
                SqlParameter[] parameters = {
                    new SqlParameter("@TrainingId", training.TrainingId),
                    new SqlParameter("@EmployeeId", training.EmployeeId),
                    new SqlParameter("@TrainingPlanPath", training.TrainingPlanPath ?? (object)DBNull.Value),
                    new SqlParameter("@CertificatePath", training.CertificatePath ?? (object)DBNull.Value),
                    new SqlParameter("@EvaluationPath", training.EvaluationPath ?? (object)DBNull.Value),
                    new SqlParameter("@CareerPath", training.CareerPath ?? (object)DBNull.Value)
                };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadTrainings();
                MessageBox.Show("Thêm đào tạo thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm đào tạo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateTraining(Training training)
        {
            try
            {
                string query = @"UPDATE Training SET EmployeeId = @EmployeeId, TrainingPlanPath = @TrainingPlanPath, 
                                CertificatePath = @CertificatePath, EvaluationPath = @EvaluationPath, CareerPath = @CareerPath 
                                WHERE TrainingId = @TrainingId";
                SqlParameter[] parameters = {
                    new SqlParameter("@TrainingId", training.TrainingId),
                    new SqlParameter("@EmployeeId", training.EmployeeId),
                    new SqlParameter("@TrainingPlanPath", training.TrainingPlanPath ?? (object)DBNull.Value),
                    new SqlParameter("@CertificatePath", training.CertificatePath ?? (object)DBNull.Value),
                    new SqlParameter("@EvaluationPath", training.EvaluationPath ?? (object)DBNull.Value),
                    new SqlParameter("@CareerPath", training.CareerPath ?? (object)DBNull.Value)
                };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadTrainings();
                MessageBox.Show("Cập nhật đào tạo thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật đào tạo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteTraining(string trainingId)
        {
            try
            {
                string query = "DELETE FROM Training WHERE TrainingId = @TrainingId";
                SqlParameter[] parameters = { new SqlParameter("@TrainingId", trainingId) };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadTrainings();
                MessageBox.Show("Xóa đào tạo thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa đào tạo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ExportTrainingsToExcel()
        {
            try
            {
                DataTable dt = GetTrainingsData();
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Đào tạo");
                    worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel files|*.xlsx",
                        FileName = $"Trainings_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                        OverwritePrompt = false
                    };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        File.WriteAllBytes(saveFileDialog.FileName, package.GetAsByteArray());
                        MessageBox.Show("Xuất file Excel thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}