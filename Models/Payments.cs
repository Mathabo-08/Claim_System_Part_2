namespace ClaimManagementSystem.Models
{
    public class Payments
    {
        public int PaymentID { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public int EmployeeNumber { get; set; }
        public int ClaimID { get; set; }
    }

}
