using System.ComponentModel.DataAnnotations;

namespace HRMANAGMENT2.Models
{
    public class Training
    {
        public string TrainingId { get; set; }

        [Required(ErrorMessage = "Mã nhân viên là bắt buộc")]
        public string EmployeeId { get; set; }

        public string TrainingPlanPath { get; set; }
        public string CertificatePath { get; set; }
        public string EvaluationPath { get; set; }
        public string CareerPath { get; set; }
    }
}