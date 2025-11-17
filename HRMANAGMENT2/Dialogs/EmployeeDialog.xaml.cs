using System;
using System.Windows;
using HRMANAGMENT2.Controllers;
using HRMANAGMENT2.Models;

namespace HRMANAGMENT2.Dialogs
{
    public partial class EmployeeDialog : Window
    {
        private readonly EmployeeController _controller;
        private readonly string _id;
        private readonly bool _isReadOnly;

        public EmployeeDialog(EmployeeController controller, string id = null, bool isReadOnly = false)
        {
            InitializeComponent();
            _controller = controller;
            _id = id;
            _isReadOnly = isReadOnly;
            BtnSave.IsEnabled = !_isReadOnly;
            if (!string.IsNullOrEmpty(id))
            {
                LoadEmployee(id);
                Title = "Xem/Sửa Nhân viên";
            }
            else
            {
                Title = "Thêm Nhân viên";
            }
        }

        private void LoadEmployee(string id)
        {
            var emp = _controller.GetEmployeeById(id);
            if (emp != null)
            {
                TxtEmployeeId.Text = emp.EmployeeId;
                TxtName.Text = emp.Name;
                DpDOB.SelectedDate = emp.DOB;
                CbGender.SelectedItem = emp.Gender;
                TxtNationality.Text = emp.Nationality;
                TxtCCCD.Text = emp.CCCD;
                DpCCCDIssueDate.SelectedDate = emp.CCCDIssueDate;
                TxtCCCDIssuePlace.Text = emp.CCCDIssuePlace;
                TxtPermanentAddress.Text = emp.PermanentAddress;
                TxtCurrentAddress.Text = emp.CurrentAddress;
                TxtPhone.Text = emp.Phone;
                TxtEmail.Text = emp.Email;
                TxtMaritalStatus.Text = emp.MaritalStatus;
                TxtDependents.Text = emp.Dependents?.ToString();
                TxtSocialInsuranceNumber.Text = emp.SocialInsuranceNumber;
                TxtTaxCode.Text = emp.TaxCode;
                TxtJobDescription.Text = emp.JobDescription;
                TxtPosition.Text = emp.Position;
                TxtDepartment.Text = emp.Department;
                TxtRank.Text = emp.Rank;
                TxtManager.Text = emp.Manager;
                TxtWorkSchedule.Text = emp.WorkSchedule;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var emp = new Employee
                {
                    EmployeeId = TxtEmployeeId.Text,
                    Name = TxtName.Text,
                    DOB = DpDOB.SelectedDate,
                    Gender = CbGender.SelectedItem?.ToString(),
                    Nationality = TxtNationality.Text,
                    CCCD = TxtCCCD.Text,
                    CCCDIssueDate = DpCCCDIssueDate.SelectedDate,
                    CCCDIssuePlace = TxtCCCDIssuePlace.Text,
                    PermanentAddress = TxtPermanentAddress.Text,
                    CurrentAddress = TxtCurrentAddress.Text,
                    Phone = TxtPhone.Text,
                    Email = TxtEmail.Text,
                    MaritalStatus = TxtMaritalStatus.Text,
                    Dependents = int.TryParse(TxtDependents.Text, out int dep) ? dep : null,
                    SocialInsuranceNumber = TxtSocialInsuranceNumber.Text,
                    TaxCode = TxtTaxCode.Text,
                    JobDescription = TxtJobDescription.Text,
                    Position = TxtPosition.Text,
                    Department = TxtDepartment.Text,
                    Rank = TxtRank.Text,
                    Manager = TxtManager.Text,
                    WorkSchedule = TxtWorkSchedule.Text
                };
                if (string.IsNullOrEmpty(_id))
                {
                    _controller.AddEmployee(emp);
                }
                else
                {
                    _controller.UpdateEmployee(emp);
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