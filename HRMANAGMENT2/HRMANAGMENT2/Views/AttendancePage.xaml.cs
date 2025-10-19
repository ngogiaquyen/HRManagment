using HRMANAGMENT2.Controllers;
using HRMANAGMENT2.Dialogs;
using HRMANAGMENT2.Models;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HRMANAGMENT2.Views
{
    public partial class AttendancePage : UserControl
    {
        private readonly AttendanceController _attendanceController;
        private DataView _dvAttendance;

        public AttendancePage()
        {
            InitializeComponent();
            _attendanceController = new AttendanceController();
            LoadAttendanceData();
        }

        private void LoadAttendanceData()
        {
            try
            {
                DataTable dt = _attendanceController.GetAttendancesData();
                _dvAttendance = dt.DefaultView;
                dgAttendance.ItemsSource = _dvAttendance;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSearchAttendance_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_dvAttendance != null)
            {
                string filter = BuildRowFilter(_dvAttendance.Table, TxtSearchAttendance.Text);
                _dvAttendance.RowFilter = filter;
            }
        }

        private static string BuildRowFilter(DataTable dt, string search)
        {
            if (string.IsNullOrWhiteSpace(search)) return string.Empty;
            string escaped = search.Replace("'", "''");
            var conditions = dt.Columns.Cast<DataColumn>()
                .Where(c => c.DataType == typeof(string) || c.DataType == typeof(DateTime) || c.DataType == typeof(int) || c.DataType == typeof(decimal))
                .Select(c => $"CONVERT([{c.ColumnName}], 'System.String') LIKE '%{escaped}%'");
            return string.Join(" OR ", conditions);
        }

        private void BtnSearchAttendance_Click(object sender, RoutedEventArgs e) => TxtSearchAttendance_TextChanged(null, null);

        private void BtnAddAttendance_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AttendanceDialog(_attendanceController);
            if (dialog.ShowDialog() == true)
            {
                LoadAttendanceData();
            }
        }

        private void BtnUpdateAttendance_Click(object sender, RoutedEventArgs e)
        {
            if (dgAttendance.SelectedItem is DataRowView row)
            {
                string id = row["AttendanceId"]?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    var dialog = new AttendanceDialog(_attendanceController, id);
                    if (dialog.ShowDialog() == true)
                    {
                        LoadAttendanceData();
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một bản ghi chấm công!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnDeleteAttendance_Click(object sender, RoutedEventArgs e)
        {
            if (dgAttendance.SelectedItem is DataRowView row)
            {
                string id = row["AttendanceId"]?.ToString();
                if (!string.IsNullOrEmpty(id) && MessageBox.Show($"Xóa bản ghi chấm công {id}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _attendanceController.DeleteAttendance(id);
                    LoadAttendanceData();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một bản ghi chấm công!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnExportAttendance_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _attendanceController.ExportAttendancesToExcel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgAttendance_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgAttendance.SelectedItem is DataRowView row)
            {
                string id = row["AttendanceId"]?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    var dialog = new AttendanceDialog(_attendanceController, id, isReadOnly: true);
                    dialog.ShowDialog();
                }
            }
        }
    }
}