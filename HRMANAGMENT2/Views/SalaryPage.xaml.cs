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
    public partial class SalaryPage : UserControl
    {
        private readonly SalaryController _salaryController;
        private DataView _dvSalaries;

        public SalaryPage()
        {
            InitializeComponent();
            _salaryController = new SalaryController();
            LoadSalaries();
        }

        private void LoadSalaries()
        {
            try
            {
                DataTable dt = _salaryController.GetSalariesData();
                _dvSalaries = dt.DefaultView;
                DgvSalaries.ItemsSource = _dvSalaries;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSearchSalaries_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_dvSalaries != null)
            {
                string filter = BuildRowFilter(_dvSalaries.Table, TxtSearchSalaries.Text);
                _dvSalaries.RowFilter = filter;
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

        private void BtnSearchSalaries_Click(object sender, RoutedEventArgs e) => TxtSearchSalaries_TextChanged(null, null);

        private void BtnAddSalary_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SalaryDialog(_salaryController);
            if (dialog.ShowDialog() == true)
            {
                LoadSalaries();
            }
        }

        private void BtnUpdateSalary_Click(object sender, RoutedEventArgs e)
        {
            if (DgvSalaries.SelectedItem is DataRowView row)
            {
                string id = row["SalaryId"]?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    var dialog = new SalaryDialog(_salaryController, id);
                    if (dialog.ShowDialog() == true)
                    {
                        LoadSalaries();
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một lương!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnDeleteSalary_Click(object sender, RoutedEventArgs e)
        {
            if (DgvSalaries.SelectedItem is DataRowView row)
            {
                string id = row["SalaryId"]?.ToString();
                if (!string.IsNullOrEmpty(id) && MessageBox.Show($"Xóa lương {id}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _salaryController.DeleteSalary(id);
                    LoadSalaries();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một lương!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnExportSalaries_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _salaryController.ExportSalariesToExcel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgvSalaries_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DgvSalaries.SelectedItem is DataRowView row)
            {
                string id = row["SalaryId"]?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    var dialog = new SalaryDialog(_salaryController, id, isReadOnly: true);
                    dialog.ShowDialog();
                }
            }
        }
    }
}