namespace server.Models
{
    public class GroupMember
    {
        public int GroupMemberID { get; set; }
        public int GroupID { get; set; }
        public int UserID { get; set; }

        // Add this property to track the balance owed by each member
        public decimal BalanceOwed { get; set; } = 0;

        // Navigation properties
        public Group? Group { get; set; }
        public User? User { get; set; }
    }
}
