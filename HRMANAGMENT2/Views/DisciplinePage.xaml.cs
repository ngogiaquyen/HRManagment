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
    public partial class DisciplinePage : UserControl
    {
        private readonly DisciplineController _disciplineController;
        private DataView _dvDisciplines;

        public DisciplinePage()
        {
            InitializeComponent();
            _disciplineController = new DisciplineController();
            LoadDisciplines();
        }

        private void LoadDisciplines()
        {
            try
            {
                DataTable dt = _disciplineController.GetDisciplinesData();
                _dvDisciplines = dt.DefaultView;
                DgvDisciplines.ItemsSource = _dvDisciplines;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TxtSearchDisciplines_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_dvDisciplines != null)
            {
                string filter = BuildRowFilter(_dvDisciplines.Table, TxtSearchDisciplines.Text);
                _dvDisciplines.RowFilter = filter;
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

        private void BtnSearchDisciplines_Click(object sender, RoutedEventArgs e) => TxtSearchDisciplines_TextChanged(null, null);

        private void BtnAddDiscipline_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new DisciplineDialog(_disciplineController);
            if (dialog.ShowDialog() == true)
            {
                LoadDisciplines();
            }
        }

        private void BtnUpdateDiscipline_Click(object sender, RoutedEventArgs e)
        {
            if (DgvDisciplines.SelectedItem is DataRowView row)
            {
                string id = row["DisciplineId"]?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    var dialog = new DisciplineDialog(_disciplineController, id);
                    if (dialog.ShowDialog() == true)
                    {
                        LoadDisciplines();
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một kỷ luật!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BtnDeleteDiscipline_Click(object sender, RoutedEventArgs e)
        {
            if (DgvDisciplines.SelectedItem is DataRowView row)
            {
                string id = row["DisciplineId"]?.ToString();
                if (!string.IsNullOrEmpty(id) && MessageBox.Show($"Xóa kỷ luật {id}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _disciplineController.DeleteDiscipline(id);
                    LoadDisciplines();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một kỷ luật!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnExportDisciplines_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _disciplineController.ExportDisciplinesToExcel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgvDisciplines_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DgvDisciplines.SelectedItem is DataRowView row)
            {
                string id = row["DisciplineId"]?.ToString();
                if (!string.IsNullOrEmpty(id))
                {
                    var dialog = new DisciplineDialog(_disciplineController, id, isReadOnly: true);
                    dialog.ShowDialog();
                }
            }
        }
    }
}