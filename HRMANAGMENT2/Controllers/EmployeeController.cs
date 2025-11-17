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
    public class EmployeeController
    {
        private readonly DatabaseManager _dbManager;

        public EmployeeController()
        {
            _dbManager = new DatabaseManager();
        }

        private string GenerateEmployeeId()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Employee";
                int count = Convert.ToInt32(_dbManager.ExecuteScalar(query, null));
                return $"EMP{(count + 1):D4}";
            }
            catch
            {
                return $"EMP{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        public void LoadEmployees()
        {
            try
            {
                string query = "SELECT * FROM Employee";
                DataTable dt = _dbManager.GetDataTable(query, null);
                // Load data
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách nhân viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public DataTable GetEmployeesData()
        {
            string query = "SELECT * FROM Employee";
            return _dbManager.GetDataTable(query, null);
        }

        public Employee GetEmployeeById(string employeeId)
        {
            try
            {
                string query = "SELECT * FROM Employee WHERE EmployeeId = @EmployeeId";
                SqlParameter[] parameters = { new SqlParameter("@EmployeeId", employeeId) };
                DataTable dt = _dbManager.GetDataTable(query, parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Employee
                    {
                        EmployeeId = row["EmployeeId"].ToString(),
                        Name = row["Name"].ToString(),
                        DOB = row["DOB"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["DOB"]) : null,
                        Gender = row["Gender"].ToString(),
                        Nationality = row["Nationality"]?.ToString(),
                        CCCD = row["CCCD"]?.ToString(),
                        CCCDIssueDate = row["CCCDIssueDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["CCCDIssueDate"]) : null,
                        CCCDIssuePlace = row["CCCDIssuePlace"]?.ToString(),
                        PermanentAddress = row["PermanentAddress"]?.ToString(),
                        CurrentAddress = row["CurrentAddress"]?.ToString(),
                        Phone = row["Phone"]?.ToString(),
                        Email = row["Email"]?.ToString(),
                        MaritalStatus = row["MaritalStatus"]?.ToString(),
                        Dependents = row["Dependents"] != DBNull.Value ? (int?)Convert.ToInt32(row["Dependents"]) : null,
                        SocialInsuranceNumber = row["SocialInsuranceNumber"]?.ToString(),
                        TaxCode = row["TaxCode"]?.ToString(),
                        JobDescription = row["JobDescription"]?.ToString(),
                        Position = row["Position"]?.ToString(),
                        Department = row["Department"]?.ToString(),
                        Rank = row["Rank"]?.ToString(),
                        Manager = row["Manager"]?.ToString(),
                        WorkSchedule = row["WorkSchedule"]?.ToString()
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy nhân viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public void AddEmployee(Employee employee)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(employee.EmployeeId))
                {
                    employee.EmployeeId = GenerateEmployeeId();
                }
                string query = @"INSERT INTO Employee (EmployeeId, Name, DOB, Gender, Nationality, CCCD, CCCDIssueDate, 
                                CCCDIssuePlace, PermanentAddress, CurrentAddress, Phone, Email, MaritalStatus, Dependents, 
                                SocialInsuranceNumber, TaxCode, JobDescription, Position, Department, Rank, Manager, WorkSchedule)
                                VALUES (@EmployeeId, @Name, @DOB, @Gender, @Nationality, @CCCD, @CCCDIssueDate, 
                                @CCCDIssuePlace, @PermanentAddress, @CurrentAddress, @Phone, @Email, @MaritalStatus, 
                                @Dependents, @SocialInsuranceNumber, @TaxCode, @JobDescription, @Position, @Department, 
                                @Rank, @Manager, @WorkSchedule)";
                SqlParameter[] parameters = {
                    new SqlParameter("@EmployeeId", employee.EmployeeId),
                    new SqlParameter("@Name", employee.Name),
                    new SqlParameter("@DOB", employee.DOB ?? (object)DBNull.Value),
                    new SqlParameter("@Gender", employee.Gender ?? (object)DBNull.Value),
                    new SqlParameter("@Nationality", employee.Nationality ?? (object)DBNull.Value),
                    new SqlParameter("@CCCD", employee.CCCD ?? (object)DBNull.Value),
                    new SqlParameter("@CCCDIssueDate", employee.CCCDIssueDate ?? (object)DBNull.Value),
                    new SqlParameter("@CCCDIssuePlace", employee.CCCDIssuePlace ?? (object)DBNull.Value),
                    new SqlParameter("@PermanentAddress", employee.PermanentAddress ?? (object)DBNull.Value),
                    new SqlParameter("@CurrentAddress", employee.CurrentAddress ?? (object)DBNull.Value),
                    new SqlParameter("@Phone", employee.Phone ?? (object)DBNull.Value),
                    new SqlParameter("@Email", employee.Email ?? (object)DBNull.Value),
                    new SqlParameter("@MaritalStatus", employee.MaritalStatus ?? (object)DBNull.Value),
                    new SqlParameter("@Dependents", employee.Dependents ?? (object)DBNull.Value),
                    new SqlParameter("@SocialInsuranceNumber", employee.SocialInsuranceNumber ?? (object)DBNull.Value),
                    new SqlParameter("@TaxCode", employee.TaxCode ?? (object)DBNull.Value),
                    new SqlParameter("@JobDescription", employee.JobDescription ?? (object)DBNull.Value),
                    new SqlParameter("@Position", employee.Position ?? (object)DBNull.Value),
                    new SqlParameter("@Department", employee.Department ?? (object)DBNull.Value),
                    new SqlParameter("@Rank", employee.Rank ?? (object)DBNull.Value),
                    new SqlParameter("@Manager", employee.Manager ?? (object)DBNull.Value),
                    new SqlParameter("@WorkSchedule", employee.WorkSchedule ?? (object)DBNull.Value)
                };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadEmployees();
                MessageBox.Show("Thêm nhân viên thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm nhân viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateEmployee(Employee employee)
        {
            try
            {
                string query = @"UPDATE Employee SET Name = @Name, DOB = @DOB, Gender = @Gender, Nationality = @Nationality, 
                                CCCD = @CCCD, CCCDIssueDate = @CCCDIssueDate, CCCDIssuePlace = @CCCDIssuePlace, 
                                PermanentAddress = @PermanentAddress, CurrentAddress = @CurrentAddress, Phone = @Phone, 
                                Email = @Email, MaritalStatus = @MaritalStatus, Dependents = @Dependents, 
                                SocialInsuranceNumber = @SocialInsuranceNumber, TaxCode = @TaxCode, 
                                JobDescription = @JobDescription, Position = @Position, Department = @Department, 
                                Rank = @Rank, Manager = @Manager, WorkSchedule = @WorkSchedule 
                                WHERE EmployeeId = @EmployeeId";
                SqlParameter[] parameters = {
                    new SqlParameter("@EmployeeId", employee.EmployeeId),
                    new SqlParameter("@Name", employee.Name),
                    new SqlParameter("@DOB", employee.DOB ?? (object)DBNull.Value),
                    new SqlParameter("@Gender", employee.Gender ?? (object)DBNull.Value),
                    new SqlParameter("@Nationality", employee.Nationality ?? (object)DBNull.Value),
                    new SqlParameter("@CCCD", employee.CCCD ?? (object)DBNull.Value),
                    new SqlParameter("@CCCDIssueDate", employee.CCCDIssueDate ?? (object)DBNull.Value),
                    new SqlParameter("@CCCDIssuePlace", employee.CCCDIssuePlace ?? (object)DBNull.Value),
                    new SqlParameter("@PermanentAddress", employee.PermanentAddress ?? (object)DBNull.Value),
                    new SqlParameter("@CurrentAddress", employee.CurrentAddress ?? (object)DBNull.Value),
                    new SqlParameter("@Phone", employee.Phone ?? (object)DBNull.Value),
                    new SqlParameter("@Email", employee.Email ?? (object)DBNull.Value),
                    new SqlParameter("@MaritalStatus", employee.MaritalStatus ?? (object)DBNull.Value),
                    new SqlParameter("@Dependents", employee.Dependents ?? (object)DBNull.Value),
                    new SqlParameter("@SocialInsuranceNumber", employee.SocialInsuranceNumber ?? (object)DBNull.Value),
                    new SqlParameter("@TaxCode", employee.TaxCode ?? (object)DBNull.Value),
                    new SqlParameter("@JobDescription", employee.JobDescription ?? (object)DBNull.Value),
                    new SqlParameter("@Position", employee.Position ?? (object)DBNull.Value),
                    new SqlParameter("@Department", employee.Department ?? (object)DBNull.Value),
                    new SqlParameter("@Rank", employee.Rank ?? (object)DBNull.Value),
                    new SqlParameter("@Manager", employee.Manager ?? (object)DBNull.Value),
                    new SqlParameter("@WorkSchedule", employee.WorkSchedule ?? (object)DBNull.Value)
                };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadEmployees();
                MessageBox.Show("Cập nhật nhân viên thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật nhân viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteEmployee(string employeeId)
        {
            try
            {
                _dbManager.ExecuteNonQuery("DELETE FROM Contract WHERE EmployeeId = @EmployeeId", new SqlParameter[] { new SqlParameter("@EmployeeId", employeeId) });
                _dbManager.ExecuteNonQuery("DELETE FROM Attendance WHERE EmployeeId = @EmployeeId", new SqlParameter[] { new SqlParameter("@EmployeeId", employeeId) });
                _dbManager.ExecuteNonQuery("DELETE FROM Recruitment WHERE EmployeeId = @EmployeeId", new SqlParameter[] { new SqlParameter("@EmployeeId", employeeId) });
                _dbManager.ExecuteNonQuery("DELETE FROM Salary WHERE EmployeeId = @EmployeeId", new SqlParameter[] { new SqlParameter("@EmployeeId", employeeId) });
                _dbManager.ExecuteNonQuery("DELETE FROM Training WHERE EmployeeId = @EmployeeId", new SqlParameter[] { new SqlParameter("@EmployeeId", employeeId) });
                _dbManager.ExecuteNonQuery("DELETE FROM Discipline WHERE EmployeeId = @EmployeeId", new SqlParameter[] { new SqlParameter("@EmployeeId", employeeId) });
                _dbManager.ExecuteNonQuery("DELETE FROM Employee WHERE EmployeeId = @EmployeeId", new SqlParameter[] { new SqlParameter("@EmployeeId", employeeId) });
                LoadEmployees();
                MessageBox.Show("Xóa nhân viên thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa nhân viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ExportEmployeesToExcel()
        {
            try
            {
                DataTable dt = GetEmployeesData();
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Nhân viên");
                    worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel files|*.xlsx",
                        FileName = $"Employees_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
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