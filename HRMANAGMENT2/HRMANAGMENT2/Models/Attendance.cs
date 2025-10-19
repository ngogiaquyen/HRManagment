using System;
using System.ComponentModel.DataAnnotations;

namespace HRMANAGMENT2.Models
{
    public class Attendance
    {
        public string AttendanceId { get; set; }

        [Required(ErrorMessage = "Mã nhân viên là bắt buộc")]
        public string EmployeeId { get; set; }

        [Required(ErrorMessage = "Ngày chấm công là bắt buộc")]
        public DateTime? AttendanceDate { get; set; }

        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        [StringLength(20, ErrorMessage = "Trạng thái không được vượt quá 20 ký tự")]
        public string Status { get; set; }

        [Range(0, 24, ErrorMessage = "Số giờ hành chính phải từ 0 đến 24")]
        public string? AdminHours { get; set; }

        [Range(0, 24, ErrorMessage = "Số giờ làm thêm phải từ 0 đến 24")]
        public string? OvertimeHours { get; set; }
    }
}