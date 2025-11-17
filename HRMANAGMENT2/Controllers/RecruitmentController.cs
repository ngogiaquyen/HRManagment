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
    public class RecruitmentController
    {
        private readonly DatabaseManager _dbManager;

        public RecruitmentController()
        {
            _dbManager = new DatabaseManager();
        }

        private string GenerateRecruitmentId()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Recruitment";
                int count = Convert.ToInt32(_dbManager.ExecuteScalar(query, null));
                return $"REC{(count + 1):D4}";
            }
            catch
            {
                return $"REC{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        public void LoadRecruitments()
        {
            try
            {
                string query = @"SELECT r.*, (e.Name + ' - ' + e.CCCD + ' - ' + CONVERT(varchar(10), e.DOB, 120)) AS EmployeeInfo
                                 FROM Recruitment r
                                 LEFT JOIN Employee e ON r.EmployeeId = e.EmployeeId";
                DataTable dt = _dbManager.GetDataTable(query, null);
                // Load data
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách tuyển dụng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public DataTable GetRecruitmentsData()
        {
            string query = @"SELECT r.*, (e.Name + ' - ' + e.CCCD + ' - ' + CONVERT(varchar(10), e.DOB, 120)) AS EmployeeInfo
                             FROM Recruitment r
                             LEFT JOIN Employee e ON r.EmployeeId = e.EmployeeId";
            return _dbManager.GetDataTable(query, null);
        }

        public Recruitment GetRecruitmentById(string recruitmentId)
        {
            try
            {
                string query = "SELECT * FROM Recruitment WHERE RecruitmentId = @RecruitmentId";
                SqlParameter[] parameters = { new SqlParameter("@RecruitmentId", recruitmentId) };
                DataTable dt = _dbManager.GetDataTable(query, parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Recruitment
                    {
                        RecruitmentId = row["RecruitmentId"].ToString(),
                        EmployeeId = row["EmployeeId"].ToString(),
                        JobApplicationPath = row["JobApplicationPath"]?.ToString(),
                        ResumePath = row["ResumePath"]?.ToString(),
                        DegreesPath = row["DegreesPath"]?.ToString(),
                        HealthCheckPath = row["HealthCheckPath"]?.ToString(),
                        CVPath = row["CVPath"]?.ToString(),
                        ReferenceLetterPath = row["ReferenceLetterPath"]?.ToString(),
                        InterviewMinutesPath = row["InterviewMinutesPath"]?.ToString(),
                        OfferLetterPath = row["OfferLetterPath"]?.ToString()
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy tuyển dụng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public void AddRecruitment(Recruitment recruitment)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(recruitment.RecruitmentId))
                {
                    recruitment.RecruitmentId = GenerateRecruitmentId();
                }
                string query = @"INSERT INTO Recruitment (RecruitmentId, EmployeeId, JobApplicationPath, ResumePath, 
                                DegreesPath, HealthCheckPath, CVPath, ReferenceLetterPath, InterviewMinutesPath, OfferLetterPath)
                                VALUES (@RecruitmentId, @EmployeeId, @JobApplicationPath, @ResumePath, @DegreesPath, 
                                @HealthCheckPath, @CVPath, @ReferenceLetterPath, @InterviewMinutesPath, @OfferLetterPath)";
                SqlParameter[] parameters = {
                    new SqlParameter("@RecruitmentId", recruitment.RecruitmentId),
                    new SqlParameter("@EmployeeId", recruitment.EmployeeId),
                    new SqlParameter("@JobApplicationPath", recruitment.JobApplicationPath ?? (object)DBNull.Value),
                    new SqlParameter("@ResumePath", recruitment.ResumePath ?? (object)DBNull.Value),
                    new SqlParameter("@DegreesPath", recruitment.DegreesPath ?? (object)DBNull.Value),
                    new SqlParameter("@HealthCheckPath", recruitment.HealthCheckPath ?? (object)DBNull.Value),
                    new SqlParameter("@CVPath", recruitment.CVPath ?? (object)DBNull.Value),
                    new SqlParameter("@ReferenceLetterPath", recruitment.ReferenceLetterPath ?? (object)DBNull.Value),
                    new SqlParameter("@InterviewMinutesPath", recruitment.InterviewMinutesPath ?? (object)DBNull.Value),
                    new SqlParameter("@OfferLetterPath", recruitment.OfferLetterPath ?? (object)DBNull.Value)
                };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadRecruitments();
                MessageBox.Show("Thêm tuyển dụng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm tuyển dụng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateRecruitment(Recruitment recruitment)
        {
            try
            {
                string query = @"UPDATE Recruitment SET EmployeeId = @EmployeeId, JobApplicationPath = @JobApplicationPath, 
                                ResumePath = @ResumePath, DegreesPath = @DegreesPath, HealthCheckPath = @HealthCheckPath, 
                                CVPath = @CVPath, ReferenceLetterPath = @ReferenceLetterPath, InterviewMinutesPath = @InterviewMinutesPath, 
                                OfferLetterPath = @OfferLetterPath 
                                WHERE RecruitmentId = @RecruitmentId";
                SqlParameter[] parameters = {
                    new SqlParameter("@RecruitmentId", recruitment.RecruitmentId),
                    new SqlParameter("@EmployeeId", recruitment.EmployeeId),
                    new SqlParameter("@JobApplicationPath", recruitment.JobApplicationPath ?? (object)DBNull.Value),
                    new SqlParameter("@ResumePath", recruitment.ResumePath ?? (object)DBNull.Value),
                    new SqlParameter("@DegreesPath", recruitment.DegreesPath ?? (object)DBNull.Value),
                    new SqlParameter("@HealthCheckPath", recruitment.HealthCheckPath ?? (object)DBNull.Value),
                    new SqlParameter("@CVPath", recruitment.CVPath ?? (object)DBNull.Value),
                    new SqlParameter("@ReferenceLetterPath", recruitment.ReferenceLetterPath ?? (object)DBNull.Value),
                    new SqlParameter("@InterviewMinutesPath", recruitment.InterviewMinutesPath ?? (object)DBNull.Value),
                    new SqlParameter("@OfferLetterPath", recruitment.OfferLetterPath ?? (object)DBNull.Value)
                };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadRecruitments();
                MessageBox.Show("Cập nhật tuyển dụng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật tuyển dụng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteRecruitment(string recruitmentId)
        {
            try
            {
                string query = "DELETE FROM Recruitment WHERE RecruitmentId = @RecruitmentId";
                SqlParameter[] parameters = { new SqlParameter("@RecruitmentId", recruitmentId) };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadRecruitments();
                MessageBox.Show("Xóa tuyển dụng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa tuyển dụng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ExportRecruitmentsToExcel()
        {
            try
            {
                DataTable dt = GetRecruitmentsData();
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Tuyển dụng");
                    worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel files|*.xlsx",
                        FileName = $"Recruitments_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
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