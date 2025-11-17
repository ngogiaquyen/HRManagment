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
    public class DisciplineController
    {
        private readonly DatabaseManager _dbManager;

        public DisciplineController()
        {
            _dbManager = new DatabaseManager();
        }

        private string GenerateDisciplineId()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Discipline";
                int count = Convert.ToInt32(_dbManager.ExecuteScalar(query, null));
                return $"DIS{(count + 1):D4}";
            }
            catch
            {
                return $"DIS{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        public void LoadDisciplines()
        {
            try
            {
                string query = @"SELECT d.*, (e.Name + ' - ' + ISNULL(e.CCCD,'') + ' - ' + ISNULL(CONVERT(varchar(10), e.DOB, 120),'') ) AS EmployeeInfo
                                 FROM Discipline d
                                 LEFT JOIN Employee e ON e.EmployeeId = d.EmployeeId";
                DataTable dt = _dbManager.GetDataTable(query, null);
                // Load data
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách kỷ luật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public DataTable GetDisciplinesData()
        {
            string query = @"SELECT d.*, (e.Name + ' - ' + ISNULL(e.CCCD,'') + ' - ' + ISNULL(CONVERT(varchar(10), e.DOB, 120),'') ) AS EmployeeInfo
                             FROM Discipline d
                             LEFT JOIN Employee e ON e.EmployeeId = d.EmployeeId";
            return _dbManager.GetDataTable(query, null);
        }

        public Discipline GetDisciplineById(string disciplineId)
        {
            try
            {
                string query = "SELECT * FROM Discipline WHERE DisciplineId = @DisciplineId";
                SqlParameter[] parameters = { new SqlParameter("@DisciplineId", disciplineId) };
                DataTable dt = _dbManager.GetDataTable(query, parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Discipline
                    {
                        DisciplineId = row["DisciplineId"].ToString(),
                        EmployeeId = row["EmployeeId"].ToString(),
                        ViolationPath = row["ViolationPath"]?.ToString(),
                        DisciplinaryDecisionPath = row["DisciplinaryDecisionPath"]?.ToString(),
                        ResignationLetterPath = row["ResignationLetterPath"]?.ToString(),
                        TerminationDecisionPath = row["TerminationDecisionPath"]?.ToString(),
                        HandoverPath = row["HandoverPath"]?.ToString(),
                        LiquidationPath = row["LiquidationPath"]?.ToString()
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy kỷ luật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public void AddDiscipline(Discipline discipline)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(discipline.DisciplineId))
                {
                    discipline.DisciplineId = GenerateDisciplineId();
                }
                string query = @"INSERT INTO Discipline (DisciplineId, EmployeeId, ViolationPath, DisciplinaryDecisionPath, 
                                ResignationLetterPath, TerminationDecisionPath, HandoverPath, LiquidationPath)
                                VALUES (@DisciplineId, @EmployeeId, @ViolationPath, @DisciplinaryDecisionPath, 
                                @ResignationLetterPath, @TerminationDecisionPath, @HandoverPath, @LiquidationPath)";
                SqlParameter[] parameters = {
                    new SqlParameter("@DisciplineId", discipline.DisciplineId),
                    new SqlParameter("@EmployeeId", discipline.EmployeeId),
                    new SqlParameter("@ViolationPath", discipline.ViolationPath ?? (object)DBNull.Value),
                    new SqlParameter("@DisciplinaryDecisionPath", discipline.DisciplinaryDecisionPath ?? (object)DBNull.Value),
                    new SqlParameter("@ResignationLetterPath", discipline.ResignationLetterPath ?? (object)DBNull.Value),
                    new SqlParameter("@TerminationDecisionPath", discipline.TerminationDecisionPath ?? (object)DBNull.Value),
                    new SqlParameter("@HandoverPath", discipline.HandoverPath ?? (object)DBNull.Value),
                    new SqlParameter("@LiquidationPath", discipline.LiquidationPath ?? (object)DBNull.Value)
                };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadDisciplines();
                MessageBox.Show("Thêm kỷ luật thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm kỷ luật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateDiscipline(Discipline discipline)
        {
            try
            {
                string query = @"UPDATE Discipline SET EmployeeId = @EmployeeId, ViolationPath = @ViolationPath, 
                                DisciplinaryDecisionPath = @DisciplinaryDecisionPath, ResignationLetterPath = @ResignationLetterPath, 
                                TerminationDecisionPath = @TerminationDecisionPath, HandoverPath = @HandoverPath, 
                                LiquidationPath = @LiquidationPath 
                                WHERE DisciplineId = @DisciplineId";
                SqlParameter[] parameters = {
                    new SqlParameter("@DisciplineId", discipline.DisciplineId),
                    new SqlParameter("@EmployeeId", discipline.EmployeeId),
                    new SqlParameter("@ViolationPath", discipline.ViolationPath ?? (object)DBNull.Value),
                    new SqlParameter("@DisciplinaryDecisionPath", discipline.DisciplinaryDecisionPath ?? (object)DBNull.Value),
                    new SqlParameter("@ResignationLetterPath", discipline.ResignationLetterPath ?? (object)DBNull.Value),
                    new SqlParameter("@TerminationDecisionPath", discipline.TerminationDecisionPath ?? (object)DBNull.Value),
                    new SqlParameter("@HandoverPath", discipline.HandoverPath ?? (object)DBNull.Value),
                    new SqlParameter("@LiquidationPath", discipline.LiquidationPath ?? (object)DBNull.Value)
                };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadDisciplines();
                MessageBox.Show("Cập nhật kỷ luật thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật kỷ luật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteDiscipline(string disciplineId)
        {
            try
            {
                string query = "DELETE FROM Discipline WHERE DisciplineId = @DisciplineId";
                SqlParameter[] parameters = { new SqlParameter("@DisciplineId", disciplineId) };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadDisciplines();
                MessageBox.Show("Xóa kỷ luật thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa kỷ luật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ExportDisciplinesToExcel()
        {
            try
            {
                DataTable dt = GetDisciplinesData();
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Kỷ luật");
                    worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel files|*.xlsx",
                        FileName = $"Disciplines_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
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