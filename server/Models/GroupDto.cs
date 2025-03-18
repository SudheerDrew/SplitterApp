namespace server.Models
{
    public class GroupDto
    {
        public int GroupID { get; set; }
        public string? GroupName { get; set; }
        public List<MemberDto>? Members { get; set; }
        public List<ExpenseDto>? Expenses { get; set; } // Add expenses to the group
        public List<BalanceSummaryDto>? BalanceSummary { get; set; } // Add balance summary
    }

    public class MemberDto
    {
        public int UserID { get; set; }
        public string? Name { get; set; }
    }

   public class ExpenseDto
{
    public int ExpenseID { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string PaidBy { get; set; } = string.Empty; // Include payer's name
}

    public class BalanceSummaryDto
    {
        public int UserID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Owes { get; set; } // Total amount this user owes
        public decimal OwedByGroup { get; set; } // Total amount owed by the group to this user
    }
}
