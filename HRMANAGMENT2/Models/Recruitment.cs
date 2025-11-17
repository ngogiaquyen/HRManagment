using System.ComponentModel.DataAnnotations;

namespace HRMANAGMENT2.Models
{
    public class Recruitment
    {
        public string RecruitmentId { get; set; }

        [Required(ErrorMessage = "Mã nhân viên là bắt buộc")]
        public string EmployeeId { get; set; }

        public string JobApplicationPath { get; set; }
        public string ResumePath { get; set; }
        public string DegreesPath { get; set; }
        public string HealthCheckPath { get; set; }
        public string CVPath { get; set; }
        public string ReferenceLetterPath { get; set; }
        public string InterviewMinutesPath { get; set; }
        public string OfferLetterPath { get; set; }
    }
}