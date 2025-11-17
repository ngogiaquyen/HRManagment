using System;
using System.ComponentModel.DataAnnotations;

namespace HRMANAGMENT2.Models
{
    public class Contract
    {
        public string ContractId { get; set; }

        [Required(ErrorMessage = "Mã nhân viên là bắt buộc")]
        public string EmployeeId { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu hợp đồng là bắt buộc")]
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Loại hợp đồng là bắt buộc")]
        [StringLength(50, ErrorMessage = "Loại hợp đồng không được vượt quá 50 ký tự")]
        public string ContractType { get; set; }

        public string ContractAnnexPath { get; set; }
        public string ConfidentialityAgreementPath { get; set; }
        public string NonCompeteAgreementPath { get; set; }
        public string AppointmentDecisionPath { get; set; }
        public string SalaryIncreaseDecisionPath { get; set; }
        public string RewardDecisionPath { get; set; }
    }
}