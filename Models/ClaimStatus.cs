namespace ClaimManagementSystem.Models
{
    public class ClaimStatus
    {
        public int StatusID { get; set; }  // Primary key
        public DateTime StatusDate { get; set; }
        public string ContractorFeedback { get; set; }
        public string Claim_Status { get; set; }
        public int EmployeeNumber { get; set; }
        public int ClaimID { get; set; }
    }

}
