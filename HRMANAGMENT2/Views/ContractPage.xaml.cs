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
    public partial class ContractPage : UserControl
    {
        private readonly ContractController _contractController;
        private DataView _dvContracts;

        public ContractPage()
        {
            InitializeComponent();
            _contractController = new ContractController();
            LoadContracts();
        }

        private void LoadContracts()
        {
            try
            {
                DataTable dt = _contractController.GetContractsData();
                _dvContracts = dt.DefaultView;
                DgvContracts.ItemsSource = _dvContracts;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSearchContracts_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_dvContracts != null)
            {
                string filter = BuildRowFilter(_dvContracts.Table, TxtSearchContracts.Text);
                _dvContracts.RowFilter = filter;
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

        private void BtnSearchContracts_Click(object sender, RoutedEventArgs e) => TxtSearchContracts_TextChanged(null, null);

        private void BtnAddContract_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ContractDialog(_contractController);
            if (dialog.ShowDialog() == true)
            {
                LoadContracts();
            }
        }

        private void BtnUpdateContract_Click(object sender, RoutedEventArgs e)
        {
            if (DgvContracts.SelectedItem is DataRowView row)
            {
                string id = row["ContractId"]?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    var dialog = new ContractDialog(_contractController, id);
                    if (dialog.ShowDialog() == true)
                    {
                        LoadContracts();
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một hợp đồng!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnDeleteContract_Click(object sender, RoutedEventArgs e)
        {
            if (DgvContracts.SelectedItem is DataRowView row)
            {
                string id = row["ContractId"]?.ToString();
                if (!string.IsNullOrEmpty(id) && MessageBox.Show($"Xóa hợp đồng {id}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _contractController.DeleteContract(id);
                    LoadContracts();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một hợp đồng!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnExportContracts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _contractController.ExportContractsToExcel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgvContracts_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DgvContracts.SelectedItem is DataRowView row)
            {
                string id = row["ContractId"]?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    var dialog = new ContractDialog(_contractController, id, isReadOnly: true);
                    dialog.ShowDialog();
                }
            }
        }
    }
}