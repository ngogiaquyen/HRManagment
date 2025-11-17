using System;
using System.ComponentModel.DataAnnotations;

namespace HRMANAGMENT2.Models
{
    public class Employee
    {
        [Required(ErrorMessage = "Mã nhân viên là bắt buộc")]
        [StringLength(20, ErrorMessage = "Mã nhân viên không được vượt quá 20 ký tự")]
        public string EmployeeId { get; set; }

        [Required(ErrorMessage = "Tên nhân viên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên nhân viên không được vượt quá 100 ký tự")]
        public string Name { get; set; }

        public DateTime? DOB { get; set; }

        [Required(ErrorMessage = "Giới tính là bắt buộc")]
        public string Gender { get; set; }

        [StringLength(50, ErrorMessage = "Quốc tịch không được vượt quá 50 ký tự")]
        public string Nationality { get; set; }

        [StringLength(20, ErrorMessage = "Số CCCD không được vượt quá 20 ký tự")]
        public string CCCD { get; set; }

        public DateTime? CCCDIssueDate { get; set; }

        [StringLength(100, ErrorMessage = "Nơi cấp CCCD không được vượt quá 100 ký tự")]
        public string CCCDIssuePlace { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ thường trú không được vượt quá 200 ký tự")]
        public string PermanentAddress { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ hiện tại không được vượt quá 200 ký tự")]
        public string CurrentAddress { get; set; }

        [StringLength(15, ErrorMessage = "Số điện thoại không được vượt quá 15 ký tự")]
        [RegularExpression(@"^[0-9+\-\s()]*$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public string MaritalStatus { get; set; }

        [Range(0, 20, ErrorMessage = "Số người phụ thuộc phải từ 0 đến 20")]
        public int? Dependents { get; set; }

        [StringLength(20, ErrorMessage = "Số bảo hiểm xã hội không được vượt quá 20 ký tự")]
        public string SocialInsuranceNumber { get; set; }

        [StringLength(20, ErrorMessage = "Mã số thuế không được vượt quá 20 ký tự")]
        public string TaxCode { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả công việc không được vượt quá 500 ký tự")]
        public string JobDescription { get; set; }

        [StringLength(100, ErrorMessage = "Chức vụ không được vượt quá 100 ký tự")]
        public string Position { get; set; }

        [StringLength(100, ErrorMessage = "Phòng ban không được vượt quá 100 ký tự")]
        public string Department { get; set; }

        [StringLength(50, ErrorMessage = "Cấp bậc không được vượt quá 50 ký tự")]
        public string Rank { get; set; }

        [StringLength(100, ErrorMessage = "Người quản lý không được vượt quá 100 ký tự")]
        public string Manager { get; set; }

        [StringLength(200, ErrorMessage = "Lịch làm việc không được vượt quá 200 ký tự")]
        public string WorkSchedule { get; set; }
    }
}