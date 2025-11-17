using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HRMANAGMENT2.Controllers;
using HRMANAGMENT2.Models;
using HRMANAGMENT2.Dialogs;

namespace HRMANAGMENT2.Views
{
    public partial class EmployeePage : UserControl
    {
        private EmployeeController? _employeeController;
        private DataView? _dvEmployees;

        public EmployeePage()
        {
            InitializeComponent();
            _employeeController = new EmployeeController();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                if (_employeeController is null) return;
                DataTable? dt = _employeeController.GetEmployeesData();
                if (dt is not null)
                {
                    _dvEmployees = dt.DefaultView;
                    DgvEmployees.ItemsSource = _dvEmployees;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSearchEmployees_TextChanged(object? sender, TextChangedEventArgs? e)
        {
            if (_dvEmployees is not null)
            {
                string? searchText = TxtSearchEmployees?.Text;
                string filter = BuildRowFilter(_dvEmployees.Table, searchText);
                _dvEmployees.RowFilter = filter;
            }
        }

        private static string BuildRowFilter(DataTable? dt, string? search)
        {
            if (dt is null || string.IsNullOrWhiteSpace(search)) return string.Empty;
            string escaped = search.Replace("'", "''");
            var conditions = dt.Columns.Cast<DataColumn>()
                .Where(c => c.DataType == typeof(string) || c.DataType == typeof(DateTime) || c.DataType == typeof(int) || c.DataType == typeof(decimal))
                .Select(c => $"CONVERT([{c.ColumnName}], 'System.String') LIKE '%{escaped}%'");
            return string.Join(" OR ", conditions);
        }

        private void BtnSearchEmployees_Click(object? sender, RoutedEventArgs? e)
        {
            TxtSearchEmployees_TextChanged(null, null);
        }

        private void BtnAddEmployee_Click(object? sender, RoutedEventArgs? e)
        {
            if (_employeeController is null) return;
            var dialog = new EmployeeDialog(_employeeController);
            if (dialog.ShowDialog() == true)
            {
                LoadEmployees();
            }
        }

        private void BtnUpdateEmployee_Click(object? sender, RoutedEventArgs? e)
        {
            if (DgvEmployees.SelectedItem is DataRowView row && _employeeController is not null)
            {
                object? idObj = row["EmployeeId"];
                string? id = idObj?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    var dialog = new EmployeeDialog(_employeeController, id);
                    if (dialog.ShowDialog() == true)
                    {
                        LoadEmployees();
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một nhân viên!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnDeleteEmployee_Click(object? sender, RoutedEventArgs? e)
        {
            if (DgvEmployees.SelectedItem is DataRowView row && _employeeController is not null)
            {
                object? idObj = row["EmployeeId"];
                string? id = idObj?.ToString();
                if (!string.IsNullOrEmpty(id) && MessageBox.Show($"Xóa nhân viên {id}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _employeeController.DeleteEmployee(id);
                    LoadEmployees();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một nhân viên!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnExportEmployees_Click(object? sender, RoutedEventArgs? e)
        {
            try
            {
                if (_employeeController is null) return;
                _employeeController.ExportEmployeesToExcel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgvEmployees_MouseDoubleClick(object? sender, System.Windows.Input.MouseButtonEventArgs? e)
        {
            if (DgvEmployees.SelectedItem is DataRowView row && _employeeController is not null)
            {
                object? idObj = row["EmployeeId"];
                string? id = idObj?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    var dialog = new EmployeeDialog(_employeeController, id, isReadOnly: true);
                    dialog.ShowDialog();
                }
            }
        }
    }
}