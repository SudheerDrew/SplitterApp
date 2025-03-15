namespace server.Models
{
    public class Expense
    {
        public int ExpenseID { get; set; }
        public int GroupID { get; set; }
        public int UserID { get; set; } // Paid by

        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Group? Group { get; set; }
        public User? PaidBy { get; set; }
    }
}
