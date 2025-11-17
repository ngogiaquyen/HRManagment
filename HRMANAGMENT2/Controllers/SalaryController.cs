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
    public class SalaryController
    {
        private readonly DatabaseManager _dbManager;

        public SalaryController()
        {
            _dbManager = new DatabaseManager();
        }

        private string GenerateSalaryId()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Salary";
                int count = Convert.ToInt32(_dbManager.ExecuteScalar(query, null));
                return $"SAL{(count + 1):D4}";
            }
            catch
            {
                return $"SAL{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        public void LoadSalaries()
        {
            try
            {
                string query = @"SELECT s.*, (e.Name + ' - ' + e.CCCD + ' - ' + CONVERT(varchar(10), e.DOB, 120)) AS EmployeeInfo
                                 FROM Salary s
                                 LEFT JOIN Employee e ON s.EmployeeId = e.EmployeeId";
                DataTable dt = _dbManager.GetDataTable(query, null);
                // Load data
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách lương: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public DataTable GetSalariesData()
        {
            string query = @"SELECT s.*, (e.Name + ' - ' + e.CCCD + ' - ' + CONVERT(varchar(10), e.DOB, 120)) AS EmployeeInfo
                             FROM Salary s
                             LEFT JOIN Employee e ON s.EmployeeId = e.EmployeeId";
            return _dbManager.GetDataTable(query, null);
        }

        public Salary GetSalaryById(string salaryId)
        {
            try
            {
                string query = "SELECT * FROM Salary WHERE SalaryId = @SalaryId";
                SqlParameter[] parameters = { new SqlParameter("@SalaryId", salaryId) };
                DataTable dt = _dbManager.GetDataTable(query, parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Salary
                    {
                        SalaryId = row["SalaryId"].ToString(),
                        EmployeeId = row["EmployeeId"].ToString(),
                        MonthlySalary = row["MonthlySalary"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["MonthlySalary"]) : null,
                        PaySlipPath = row["PaySlipPath"]?.ToString(),
                        SalaryIncreaseDecisionPath = row["SalaryIncreaseDecisionPath"]?.ToString(),
                        BankAccount = row["BankAccount"]?.ToString(),
                        InsuranceInfo = row["InsuranceInfo"]?.ToString(),
                        Allowances = row["Allowances"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["Allowances"]) : null,
                        Bonuses = row["Bonuses"] != DBNull.Value ? (decimal?)Convert.ToDecimal(row["Bonuses"]) : null,
                        LeavePolicy = row["LeavePolicy"]?.ToString()
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy lương: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public void AddSalary(Salary salary)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(salary.SalaryId))
                {
                    salary.SalaryId = GenerateSalaryId();
                }
                string query = @"INSERT INTO Salary (SalaryId, EmployeeId, MonthlySalary, PaySlipPath, SalaryIncreaseDecisionPath, 
                                BankAccount, InsuranceInfo, Allowances, Bonuses, LeavePolicy)
                                VALUES (@SalaryId, @EmployeeId, @MonthlySalary, @PaySlipPath, @SalaryIncreaseDecisionPath, 
                                @BankAccount, @InsuranceInfo, @Allowances, @Bonuses, @LeavePolicy)";
                SqlParameter[] parameters = {
                    new SqlParameter("@SalaryId", salary.SalaryId),
                    new SqlParameter("@EmployeeId", salary.EmployeeId),
                    new SqlParameter("@MonthlySalary", salary.MonthlySalary ?? (object)DBNull.Value),
                    new SqlParameter("@PaySlipPath", salary.PaySlipPath ?? (object)DBNull.Value),
                    new SqlParameter("@SalaryIncreaseDecisionPath", salary.SalaryIncreaseDecisionPath ?? (object)DBNull.Value),
                    new SqlParameter("@BankAccount", salary.BankAccount ?? (object)DBNull.Value),
                    new SqlParameter("@InsuranceInfo", salary.InsuranceInfo ?? (object)DBNull.Value),
                    new SqlParameter("@Allowances", salary.Allowances ?? (object)DBNull.Value),
                    new SqlParameter("@Bonuses", salary.Bonuses ?? (object)DBNull.Value),
                    new SqlParameter("@LeavePolicy", salary.LeavePolicy ?? (object)DBNull.Value)
                };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadSalaries();
                MessageBox.Show("Thêm lương thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm lương: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateSalary(Salary salary)
        {
            try
            {
                string query = @"UPDATE Salary SET EmployeeId = @EmployeeId, MonthlySalary = @MonthlySalary, 
                                PaySlipPath = @PaySlipPath, SalaryIncreaseDecisionPath = @SalaryIncreaseDecisionPath, 
                                BankAccount = @BankAccount, InsuranceInfo = @InsuranceInfo, Allowances = @Allowances, 
                                Bonuses = @Bonuses, LeavePolicy = @LeavePolicy 
                                WHERE SalaryId = @SalaryId";
                SqlParameter[] parameters = {
                    new SqlParameter("@SalaryId", salary.SalaryId),
                    new SqlParameter("@EmployeeId", salary.EmployeeId),
                    new SqlParameter("@MonthlySalary", salary.MonthlySalary ?? (object)DBNull.Value),
                    new SqlParameter("@PaySlipPath", salary.PaySlipPath ?? (object)DBNull.Value),
                    new SqlParameter("@SalaryIncreaseDecisionPath", salary.SalaryIncreaseDecisionPath ?? (object)DBNull.Value),
                    new SqlParameter("@BankAccount", salary.BankAccount ?? (object)DBNull.Value),
                    new SqlParameter("@InsuranceInfo", salary.InsuranceInfo ?? (object)DBNull.Value),
                    new SqlParameter("@Allowances", salary.Allowances ?? (object)DBNull.Value),
                    new SqlParameter("@Bonuses", salary.Bonuses ?? (object)DBNull.Value),
                    new SqlParameter("@LeavePolicy", salary.LeavePolicy ?? (object)DBNull.Value)
                };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadSalaries();
                MessageBox.Show("Cập nhật lương thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật lương: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteSalary(string salaryId)
        {
            try
            {
                string query = "DELETE FROM Salary WHERE SalaryId = @SalaryId";
                SqlParameter[] parameters = { new SqlParameter("@SalaryId", salaryId) };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadSalaries();
                MessageBox.Show("Xóa lương thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa lương: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ExportSalariesToExcel()
        {
            try
            {
                DataTable dt = GetSalariesData();
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Lương");
                    worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel files|*.xlsx",
                        FileName = $"Salaries_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
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