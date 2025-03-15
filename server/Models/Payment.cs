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
    }
}
