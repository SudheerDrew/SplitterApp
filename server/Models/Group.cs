namespace server.Models
{
    public class Group
    {
        public int GroupID { get; set; }
        public string? GroupName { get; set; }

        // Navigation properties
        public List<GroupMember>? Members { get; set; }
        public List<Expense>? Expenses { get; set; }
        public List<Payment>? Payments { get; set; }
    }
}
