namespace server.Models
{
    public class GroupMember
    {
        public int GroupMemberID { get; set; }
        public int GroupID { get; set; }
        public int UserID { get; set; }

        // Navigation properties
        public Group? Group { get; set; }
        public User? User { get; set; }
    }
}
