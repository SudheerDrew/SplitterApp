namespace server.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }

        // Navigation properties
        public List<GroupMember>? GroupMemberships { get; set; }
        public List<Expense>? ExpensesPaid { get; set; }
        public List<Payment>? PaymentsMade { get; set; }
        public List<Payment>? PaymentsReceived { get; set; }
    }
}
