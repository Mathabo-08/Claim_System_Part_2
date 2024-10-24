namespace ClaimManagementSystem.Models
{
    public class ContractorResponse
    {
        public int ResponseID { get; set; }
        public DateTime ResponseDate { get; set; }
        public int ContractorID { get; set; }
        public int ClaimID { get; set; }
        public string ContractorFeedback { get; set; }
        public string ClaimStatus { get; set; } = "Pending"; // Default status
    }

}
