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
    public class AttendanceController
    {
        private readonly DatabaseManager _dbManager;

        public AttendanceController()
        {
            _dbManager = new DatabaseManager();
        }

        private string GenerateAttendanceId()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Attendance";
                int count = Convert.ToInt32(_dbManager.ExecuteScalar(query, null));
                return $"ATT{(count + 1):D4}";
            }
            catch
            {
                return $"ATT{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        public void LoadAttendances()
        {
            try
            {
                string query = @"SELECT a.*, (e.Name + ' - ' + ISNULL(e.CCCD,'') + ' - ' + ISNULL(CONVERT(varchar(10), e.DOB, 120),'') ) AS EmployeeInfo
                                 FROM Attendance a
                                 LEFT JOIN Employee e ON e.EmployeeId = a.EmployeeId";
                DataTable dt = _dbManager.GetDataTable(query, null);
                // Load data (view bind)
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách chấm công: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public DataTable GetAttendancesData()
        {
            string query = @"SELECT a.*, (e.Name + ' - ' + ISNULL(e.CCCD,'') + ' - ' + ISNULL(CONVERT(varchar(10), e.DOB, 120),'') ) AS EmployeeInfo
                             FROM Attendance a
                             LEFT JOIN Employee e ON e.EmployeeId = a.EmployeeId";
            return _dbManager.GetDataTable(query, null);
        }

        public Attendance GetAttendanceById(string attendanceId)
        {
            try
            {
                string query = "SELECT * FROM Attendance WHERE AttendanceId = @AttendanceId";
                SqlParameter[] parameters = { new SqlParameter("@AttendanceId", attendanceId) };
                DataTable dt = _dbManager.GetDataTable(query, parameters);
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new Attendance
                    {
                        AttendanceId = row["AttendanceId"].ToString(),
                        EmployeeId = row["EmployeeId"].ToString(),
                        AttendanceDate = row["AttendanceDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["AttendanceDate"]) : null,
                        CheckInTime = row["CheckInTime"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["CheckInTime"]) : null,
                        CheckOutTime = row["CheckOutTime"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["CheckOutTime"]) : null,
                        Status = row["Status"].ToString(),
                        AdminHours = row["AdminHours"] != DBNull.Value ? row["AdminHours"].ToString() : null,
                        OvertimeHours = row["OvertimeHours"] != DBNull.Value ? row["OvertimeHours"].ToString() : null,

                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lấy chấm công: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public void AddAttendance(Attendance attendance)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(attendance.AttendanceId))
                {
                    attendance.AttendanceId = GenerateAttendanceId();
                }
                string query = @"INSERT INTO Attendance (AttendanceId, EmployeeId, AttendanceDate, CheckInTime, CheckOutTime, Status, AdminHours, OvertimeHours)
                                VALUES (@AttendanceId, @EmployeeId, @AttendanceDate, @CheckInTime, @CheckOutTime, @Status, @AdminHours, @OvertimeHours)";
                SqlParameter[] parameters = {
                    new SqlParameter("@AttendanceId", attendance.AttendanceId),
                    new SqlParameter("@EmployeeId", attendance.EmployeeId),
                    new SqlParameter("@AttendanceDate", attendance.AttendanceDate ?? (object)DBNull.Value),
                    new SqlParameter("@CheckInTime", attendance.CheckInTime ?? (object)DBNull.Value),
                    new SqlParameter("@CheckOutTime", attendance.CheckOutTime ?? (object)DBNull.Value),
                    new SqlParameter("@Status", attendance.Status ?? (object)DBNull.Value),
                    new SqlParameter("@AdminHours", attendance.AdminHours ?? (object)DBNull.Value),
                    new SqlParameter("@OvertimeHours", attendance.OvertimeHours ?? (object)DBNull.Value)
                };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadAttendances();
                MessageBox.Show("Thêm chấm công thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm chấm công: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void UpdateAttendance(Attendance attendance)
        {
            try
            {
                string query = @"UPDATE Attendance SET EmployeeId = @EmployeeId, AttendanceDate = @AttendanceDate, 
                                CheckInTime = @CheckInTime, CheckOutTime = @CheckOutTime, Status = @Status, 
                                AdminHours = @AdminHours, OvertimeHours = @OvertimeHours 
                                WHERE AttendanceId = @AttendanceId";
                SqlParameter[] parameters = {
                    new SqlParameter("@AttendanceId", attendance.AttendanceId),
                    new SqlParameter("@EmployeeId", attendance.EmployeeId),
                    new SqlParameter("@AttendanceDate", attendance.AttendanceDate ?? (object)DBNull.Value),
                    new SqlParameter("@CheckInTime", attendance.CheckInTime ?? (object)DBNull.Value),
                    new SqlParameter("@CheckOutTime", attendance.CheckOutTime ?? (object)DBNull.Value),
                    new SqlParameter("@Status", attendance.Status ?? (object)DBNull.Value),
                    new SqlParameter("@AdminHours", attendance.AdminHours ?? (object)DBNull.Value),
                    new SqlParameter("@OvertimeHours", attendance.OvertimeHours ?? (object)DBNull.Value)
                };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadAttendances();
                MessageBox.Show("Cập nhật chấm công thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật chấm công: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteAttendance(string attendanceId)
        {
            try
            {
                string query = "DELETE FROM Attendance WHERE AttendanceId = @AttendanceId";
                SqlParameter[] parameters = { new SqlParameter("@AttendanceId", attendanceId) };
                _dbManager.ExecuteNonQuery(query, parameters);
                LoadAttendances();
                MessageBox.Show("Xóa chấm công thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa chấm công: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ExportAttendancesToExcel()
        {
            try
            {
                DataTable dt = GetAttendancesData();
                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Chấm công");
                    worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                    SaveFileDialog saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Excel files|*.xlsx",
                        FileName = $"Attendances_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
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