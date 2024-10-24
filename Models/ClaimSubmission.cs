namespace ClaimManagementSystem.Models
{
    public class ClaimSubmission
    {
        public int ClaimID { get; set; } // Primary key
        public DateTime ClaimDate { get; set; }
        public int EmployeeNumber { get; set; }
        public string ModuleCode { get; set; }
        public string Course { get; set; }
        public decimal Amount { get; set; }
        public int MonthlyHoursWorked { get; set; }
        public decimal HourlyRate { get; set; }
        public string SuppDocFileName { get; set; }
        public byte[] SuppDocFileContent { get; set; }
        public DateTime SuppDocUploadDate { get; set; }
        public string ClaimStatus { get; set; } = "Pending"; // Default status
    }

}
