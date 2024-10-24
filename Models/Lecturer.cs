namespace ClaimManagementSystem.Models
{
    public class Lecturer
    {
        public int EmployeeNumber { get; set; }
        public string LecturerName { get; set; }
        public string LecturerSurname { get; set; }
        public string LecturerEmail { get; set; }
        public string ModuleCode { get; set; }
        public string Course { get; set; }
        public int MonthlyHoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public string LecturerPassword { get; internal set; }
    }

}
