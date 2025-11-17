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
    public partial class TrainingPage : UserControl
    {
        private readonly TrainingController _trainingController;
        private DataView _dvTrainings;

        public TrainingPage()
        {
            InitializeComponent();
            _trainingController = new TrainingController();
            LoadTrainings();
        }

        private void LoadTrainings()
        {
            try
            {
                DataTable dt = _trainingController.GetTrainingsData();
                _dvTrainings = dt.DefaultView;
                DgvTrainings.ItemsSource = _dvTrainings;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSearchTrainings_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_dvTrainings != null)
            {
                string filter = BuildRowFilter(_dvTrainings.Table, TxtSearchTrainings.Text);
                _dvTrainings.RowFilter = filter;
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

        private void BtnSearchTrainings_Click(object sender, RoutedEventArgs e) => TxtSearchTrainings_TextChanged(null, null);

        private void BtnAddTraining_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new TrainingDialog(_trainingController);
            if (dialog.ShowDialog() == true)
            {
                LoadTrainings();
            }
        }

        private void BtnUpdateTraining_Click(object sender, RoutedEventArgs e)
        {
            if (DgvTrainings.SelectedItem is DataRowView row)
            {
                string id = row["TrainingId"]?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    var dialog = new TrainingDialog(_trainingController, id);
                    if (dialog.ShowDialog() == true)
                    {
                        LoadTrainings();
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một đào tạo!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnDeleteTraining_Click(object sender, RoutedEventArgs e)
        {
            if (DgvTrainings.SelectedItem is DataRowView row)
            {
                string id = row["TrainingId"]?.ToString();
                if (!string.IsNullOrEmpty(id) && MessageBox.Show($"Xóa đào tạo {id}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _trainingController.DeleteTraining(id);
                    LoadTrainings();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một đào tạo!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnExportTrainings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _trainingController.ExportTrainingsToExcel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgvTrainings_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DgvTrainings.SelectedItem is DataRowView row)
            {
                string id = row["TrainingId"]?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    var dialog = new TrainingDialog(_trainingController, id, isReadOnly: true);
                    dialog.ShowDialog();
                }
            }
        }
    }
}