namespace server.Models
{
    public class Expense
    {
        public int ExpenseID { get; set; }
        public int GroupID { get; set; }
        public int UserID { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
