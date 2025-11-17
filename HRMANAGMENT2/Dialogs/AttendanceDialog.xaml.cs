using System;
using System.Windows;
using System.Windows.Controls;
using HRMANAGMENT2.Controllers;
using HRMANAGMENT2.Models;

namespace HRMANAGMENT2.Dialogs
{
    public partial class AttendanceDialog : Window
    {
        private readonly AttendanceController _controller;
        private readonly string _id;
        private readonly bool _isReadOnly;

        public AttendanceDialog(AttendanceController controller, string id = null, bool isReadOnly = false)
        {
            InitializeComponent();
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _id = id ?? string.Empty;
            _isReadOnly = isReadOnly;
            BtnSave.IsEnabled = !_isReadOnly;
            var employees = _controller.GetEmployeesForComboBox();
            CbEmployeeId.ItemsSource = employees?.DefaultView;
            CbEmployeeId.DisplayMemberPath = "Name";
            CbEmployeeId.SelectedValuePath = "EmployeeId";
            if (!string.IsNullOrEmpty(id))
            {
                LoadAttendance(id);
                Title = "Xem/Sửa Chấm công";
            }
            else
            {
                Title = "Thêm Chấm công";
            }
        }

        private void LoadAttendance(string id)
        {
            var attendance = _controller.GetAttendanceById(id);
            if (attendance != null)
            {
                TxtAttendanceId.Text = attendance.AttendanceId ?? string.Empty;
                CbEmployeeId.SelectedValue = attendance.EmployeeId;
                DpAttendanceDate.SelectedDate = attendance.AttendanceDate;
                SetTimeFromDateTime(CbCheckInHour, CbCheckInMinute, attendance.CheckInTime);
                SetTimeFromDateTime(CbCheckOutHour, CbCheckOutMinute, attendance.CheckOutTime);
                CbStatus.SelectedItem = attendance.Status ?? string.Empty;
                TxtAdminHours.Text = attendance.AdminHours?.ToString() ?? string.Empty;
                TxtOvertimeHours.Text = attendance.OvertimeHours?.ToString() ?? string.Empty;
            }
        }

        private void SetTimeFromDateTime(ComboBox hourCombo, ComboBox minuteCombo, DateTime? time)
        {
            if (time.HasValue)
            {
                hourCombo.SelectedItem = time.Value.Hour.ToString("D2");
                minuteCombo.SelectedItem = time.Value.Minute.ToString("D2");
            }
            else
            {
                hourCombo.SelectedIndex = -1;
                minuteCombo.SelectedIndex = -1;
            }
        }

        private DateTime? GetTimeFromCombos(ComboBox hourCombo, ComboBox minuteCombo)
        {
            if (hourCombo.SelectedItem is string hourStr && minuteCombo.SelectedItem is string minuteStr &&
                int.TryParse(hourStr, out int hour) && int.TryParse(minuteStr, out int minute) &&
                hour >= 0 && hour <= 23 && minute >= 0 && minute <= 59)
            {
                return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, hour, minute, 0);
            }
            return null;
        }

        private void CalculateHours()
        {
            DateTime? checkIn = GetTimeFromCombos(CbCheckInHour, CbCheckInMinute);
            DateTime? checkOut = GetTimeFromCombos(CbCheckOutHour, CbCheckOutMinute);

            if (checkIn.HasValue && checkOut.HasValue)
            {
                TimeSpan duration;
                if (checkOut.Value > checkIn.Value)
                {
                    duration = checkOut.Value - checkIn.Value;
                }
                else
                {
                    // Giả sử làm qua đêm, thêm 1 ngày vào checkOut
                    duration = checkOut.Value.AddDays(1) - checkIn.Value;
                }

                double totalHours = duration.TotalHours;
                double adminHours = Math.Min(totalHours, 8.0);
                double overtimeHours = Math.Max(0, totalHours - 8.0);

                TxtAdminHours.Text = FormatHours(adminHours);
                TxtOvertimeHours.Text = FormatHours(overtimeHours);
            }
            else
            {
                TxtAdminHours.Text = string.Empty;
                TxtOvertimeHours.Text = string.Empty;
            }
        }

        private string FormatHours(double hours)
        {
            int wholeHours = (int)hours;
            int minutes = (int)((hours - wholeHours) * 60);
            return $"{wholeHours}h {minutes}m";
        }

        private void CbCheckInHour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateHours();
        }

        private void CbCheckInMinute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateHours();
        }

        private void CbCheckOutHour_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateHours();
        }

        private void CbCheckOutMinute_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateHours();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validation cho các trường bắt buộc
            if (string.IsNullOrEmpty(CbEmployeeId.SelectedValue?.ToString()))
            {
                MessageBox.Show("Vui lòng chọn nhân viên.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DpAttendanceDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày chấm công.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime? checkIn = GetTimeFromCombos(CbCheckInHour, CbCheckInMinute);
            DateTime? checkOut = GetTimeFromCombos(CbCheckOutHour, CbCheckOutMinute);

            if (!checkIn.HasValue)
            {
                MessageBox.Show("Vui lòng chọn giờ CheckIn.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!checkOut.HasValue)
            {
                MessageBox.Show("Vui lòng chọn giờ CheckOut.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(CbStatus.SelectedItem?.ToString()))
            {
                MessageBox.Show("Vui lòng chọn trạng thái.", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var attendance = new Attendance
                {
                    AttendanceId = TxtAttendanceId.Text ?? string.Empty,
                    EmployeeId = CbEmployeeId.SelectedValue?.ToString() ?? string.Empty,
                    AttendanceDate = DpAttendanceDate.SelectedDate,
                    CheckInTime = checkIn,
                    CheckOutTime = checkOut,
                    Status = CbStatus.SelectedItem?.ToString() ?? string.Empty,
                    AdminHours = !string.IsNullOrWhiteSpace(TxtAdminHours.Text)
                    ? TxtAdminHours.Text
                    : null,

                                    OvertimeHours = !string.IsNullOrWhiteSpace(TxtOvertimeHours.Text)
                    ? TxtOvertimeHours.Text
                    : null

                };
                if (string.IsNullOrEmpty(_id))
                {
                    _controller.AddAttendance(attendance);
                }
                else
                {
                    _controller.UpdateAttendance(attendance);
                }
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}