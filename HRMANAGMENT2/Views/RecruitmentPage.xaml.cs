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
    public partial class RecruitmentPage : UserControl
    {
        private readonly RecruitmentController _recruitmentController;
        private DataView _dvRecruitments;

        public RecruitmentPage()
        {
            InitializeComponent();
            _recruitmentController = new RecruitmentController();
            LoadRecruitments();
        }

        private void LoadRecruitments()
        {
            try
            {
                DataTable dt = _recruitmentController.GetRecruitmentsData();
                _dvRecruitments = dt.DefaultView;
                DgvRecruitments.ItemsSource = _dvRecruitments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSearchRecruitments_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_dvRecruitments != null)
            {
                string filter = BuildRowFilter(_dvRecruitments.Table, TxtSearchRecruitments.Text);
                _dvRecruitments.RowFilter = filter;
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

        private void BtnSearchRecruitments_Click(object sender, RoutedEventArgs e) => TxtSearchRecruitments_TextChanged(null, null);

        private void BtnAddRecruitment_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new RecruitmentDialog(_recruitmentController);
            if (dialog.ShowDialog() == true)
            {
                LoadRecruitments();
            }
        }

        private void BtnUpdateRecruitment_Click(object sender, RoutedEventArgs e)
        {
            if (DgvRecruitments.SelectedItem is DataRowView row)
            {
                string id = row["RecruitmentId"]?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    var dialog = new RecruitmentDialog(_recruitmentController, id);
                    if (dialog.ShowDialog() == true)
                    {
                        LoadRecruitments();
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một tuyển dụng!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnDeleteRecruitment_Click(object sender, RoutedEventArgs e)
        {
            if (DgvRecruitments.SelectedItem is DataRowView row)
            {
                string id = row["RecruitmentId"]?.ToString();
                if (!string.IsNullOrEmpty(id) && MessageBox.Show($"Xóa tuyển dụng {id}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _recruitmentController.DeleteRecruitment(id);
                    LoadRecruitments();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một tuyển dụng!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnExportRecruitments_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _recruitmentController.ExportRecruitmentsToExcel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgvRecruitments_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DgvRecruitments.SelectedItem is DataRowView row)
            {
                string id = row["RecruitmentId"]?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    var dialog = new RecruitmentDialog(_recruitmentController, id, isReadOnly: true);
                    dialog.ShowDialog();
                }
            }
        }
    }
}