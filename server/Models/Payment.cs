namespace server.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public int PayerID { get; set; }
        public int PayeeID { get; set; }
        public int GroupID { get; set; }

        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }

        // Navigation properties
        public User? Payer { get; set; }
        public User? Payee { get; set; }
        public Group? Group { get; set; }
    }
}
