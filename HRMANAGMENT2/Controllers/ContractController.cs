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
    public class ContractController
    {
        private readonly DatabaseManager _dbManager;

        public ContractController()
        {
            _dbManager = new DatabaseManager();
        }

        private string GenerateContractId()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Contract";
                int count = Convert.ToInt32(_dbManager.ExecuteScalar(query, null));
                return $"CON{(count + 1):D4}";
            }
            catch
            {
                return $"CON{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        public void LoadContracts()
        {
            try
            {
                string query = @"SELECT c.*, (e.Name + ' - ' + ISNULL(e.CCCD,'') + ' - ' + ISNULL(CONVERT(varchar(10), e.DOB, 120),'') ) AS EmployeeInfo
                                 FROM Contract c
                                 LEFT JOIN Employee e ON e.EmployeeId = c.EmployeeId";
                DataTable dt = _dbManager.GetDataTable(query, null);
                // Load data
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public DataTable GetContractsData()
        {
            string query = @"SELECT c.*, (e.Name + ' - ' + ISNULL(e.CCCD,'') + ' - ' + ISNULL(CONVERT(varchar(10), e.DOB, 120),'') ) AS EmployeeInfo
                             FROM Contract c
                             LEFT JOIN Employee e ON e.EmployeeId = c.EmployeeId";
            return _dbManager.GetDataTable(query, null);
        }

        public Contract GetContractById(string contractId)
        {
            try
            {
                string query = "SELECT * FROM Contract WHERE ContractId = @ContractId";
                SqlParameter[] parameters = { new SqlParameter("@ContractId", contractId) };
                DataTable dt = _dbManager.GetDataTable(query, parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Contract
                    {
                        ContractId = row["ContractId"].ToString(),
                        EmployeeId = row["EmployeeId"].ToString(),
                        StartDate = row["StartDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["StartDate"]) : null,
                        EndDate = row["EndDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["EndDate"]) : null,
                        ContractType = row["ContractType"].ToString(),
                        ContractAnnexPath = row["ContractAnnexPath"]?.ToString(),
                        ConfidentialityAgreementPath = row["ConfidentialityAgreementPath"]?.ToString(),
                        NonCompeteAgreementPath = row["NonCompeteAgreementPath"]?.ToString(),
                        AppointmentDecisionPath = row["AppointmentDecisionPath"]?.ToString(),
                        SalaryIncreaseDecisionPath = row["SalaryIncreaseDecisionPath"]?.ToString(),
                        RewardDecisionPath = row["RewardDecisionPath"]?.ToString()
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public void AddContract(Contract contract)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(contract.ContractId))
                {
                    contract.ContractId = GenerateContractId();
                }
                string query = @"INSERT INTO Contract (ContractId, EmployeeId, StartDate, EndDate, ContractType, ContractAnnexPath, 
                                ConfidentialityAgreementPath, NonCompeteAgreementPath, AppointmentDecisionPath, 
                                SalaryIncreaseDecisionPath, RewardDecisionPath)
                                VALUES (@ContractId, @EmployeeId, @StartDate, @EndDate, @ContractType, @ContractAnnexPath, 
                                @ConfidentialityAgreementPath, @NonCompeteAgreementPath, @AppointmentDecisionPath, 
                                @SalaryIncreaseDecisionPath, @RewardDecisionPath)";
                SqlParameter[] parameters = {
                    new SqlParameter("@ContractId", contract.ContractId),
                    new SqlParameter("@EmployeeId", contract.EmployeeId),
                    new SqlParameter("@StartDate", contract.StartDate ?? (object)DBNull.Value),
                    new SqlParameter("@EndDate", contract.EndDate ?? (object)DBNull.Value),
                    new SqlParameter("@ContractType", contract.ContractType ?? (object)DBNull.Value),
                    new SqlParameter("@ContractAnnexPath", contract.ContractAnnexPath ?? (object)DBNull.Value),
                    new SqlParameter("@ConfidentialityAgreementPath", contract.ConfidentialityAgreementPath ?? (object)DBNull.Value),
                    new SqlParameter("@NonCompeteAgreementPath", contract.NonCompeteAgreementPath ?? (object)DBNull.Value),
                    new SqlParameter("@AppointmentDecisionPath", contract.AppointmentDecisionPath ?? (object)DBNull.Value),
                    new SqlParameter("@SalaryIncreaseDecisionPath", contract.SalaryIncreaseDecisionPath ?? (object)DBNull.Value),
                    new SqlParameter("@RewardDecisionPath", contract.RewardDecisionPath ?? (object)DBNull.Value)
                };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadContracts();
                MessageBox.Show("Thêm hợp đồng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateContract(Contract contract)
        {
            try
            {
                string query = @"UPDATE Contract SET EmployeeId = @EmployeeId, StartDate = @StartDate, EndDate = @EndDate, 
                                ContractType = @ContractType, ContractAnnexPath = @ContractAnnexPath, 
                                ConfidentialityAgreementPath = @ConfidentialityAgreementPath, NonCompeteAgreementPath = @NonCompeteAgreementPath, 
                                AppointmentDecisionPath = @AppointmentDecisionPath, SalaryIncreaseDecisionPath = @SalaryIncreaseDecisionPath, 
                                RewardDecisionPath = @RewardDecisionPath 
                                WHERE ContractId = @ContractId";
                SqlParameter[] parameters = {
                    new SqlParameter("@ContractId", contract.ContractId),
                    new SqlParameter("@EmployeeId", contract.EmployeeId),
                    new SqlParameter("@StartDate", contract.StartDate ?? (object)DBNull.Value),
                    new SqlParameter("@EndDate", contract.EndDate ?? (object)DBNull.Value),
                    new SqlParameter("@ContractType", contract.ContractType ?? (object)DBNull.Value),
                    new SqlParameter("@ContractAnnexPath", contract.ContractAnnexPath ?? (object)DBNull.Value),
                    new SqlParameter("@ConfidentialityAgreementPath", contract.ConfidentialityAgreementPath ?? (object)DBNull.Value),
                    new SqlParameter("@NonCompeteAgreementPath", contract.NonCompeteAgreementPath ?? (object)DBNull.Value),
                    new SqlParameter("@AppointmentDecisionPath", contract.AppointmentDecisionPath ?? (object)DBNull.Value),
                    new SqlParameter("@SalaryIncreaseDecisionPath", contract.SalaryIncreaseDecisionPath ?? (object)DBNull.Value),
                    new SqlParameter("@RewardDecisionPath", contract.RewardDecisionPath ?? (object)DBNull.Value)
                };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadContracts();
                MessageBox.Show("Cập nhật hợp đồng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteContract(string contractId)
        {
            try
            {
                string query = "DELETE FROM Contract WHERE ContractId = @ContractId";
                SqlParameter[] parameters = { new SqlParameter("@ContractId", contractId) };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadContracts();
                MessageBox.Show("Xóa hợp đồng thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa hợp đồng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ExportContractsToExcel()
        {
            try
            {
                DataTable dt = GetContractsData();
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Hợp đồng");
                    worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel files|*.xlsx",
                        FileName = $"Contracts_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
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