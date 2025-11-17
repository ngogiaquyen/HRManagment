using System.ComponentModel.DataAnnotations;

namespace HRMANAGMENT2.Models
{
    public class Salary
    {
        public string SalaryId { get; set; }

        [Required(ErrorMessage = "Mã nhân viên là bắt buộc")]
        public string EmployeeId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Lương tháng phải lớn hơn 0")]
        public decimal? MonthlySalary { get; set; }

        public string PaySlipPath { get; set; }
        public string SalaryIncreaseDecisionPath { get; set; }

        [StringLength(50, ErrorMessage = "Số tài khoản ngân hàng không được vượt quá 50 ký tự")]
        public string BankAccount { get; set; }

        [StringLength(200, ErrorMessage = "Thông tin bảo hiểm không được vượt quá 200 ký tự")]
        public string InsuranceInfo { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Phụ cấp phải lớn hơn hoặc bằng 0")]
        public decimal? Allowances { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Thưởng phải lớn hơn hoặc bằng 0")]
        public decimal? Bonuses { get; set; }

        [StringLength(200, ErrorMessage = "Chính sách nghỉ phép không được vượt quá 200 ký tự")]
        public string LeavePolicy { get; set; }
    }
}