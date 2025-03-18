using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;

namespace server.Services
{
    public class ExpenseService
    {
        private readonly AppDbContext _context;

        public ExpenseService(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Add a New Expense
        public async Task<Expense> AddExpense(Expense expense)
        {
            expense.CreatedAt = DateTime.UtcNow; // Set creation time
            _context.Expenses.Add(expense);

            // Split the expense among group members
            await SplitExpense(expense);

            await _context.SaveChangesAsync();
            return expense;
        }

        // ✅ Split Expense Logic (Updates BalanceOwed for Group Members)
        private async Task SplitExpense(Expense expense)
        {
            var groupMembers = await _context.GroupMembers
                .Where(gm => gm.GroupID == expense.GroupID)
                .ToListAsync();

            var splitAmount = Math.Round(expense.Amount / groupMembers.Count, 2);

            foreach (var member in groupMembers)
            {
                if (member.UserID == expense.UserID)
                {
                    // The user who paid owes nothing
                    member.BalanceOwed -= expense.Amount;
                }
                else
                {
                    // All other members owe their share of the split amount
                    member.BalanceOwed += splitAmount;
                }
            }
        }

        // ✅ Get All Expenses for a Group
        public async Task<List<Expense>> GetExpensesByGroup(int groupId)
        {
            return await _context.Expenses
                .Where(e => e.GroupID == groupId)
                .Include(e => e.PaidBy) // Include the user who paid
                .ToListAsync();
        }

        // ✅ Get Expense by ID
        public async Task<Expense?> GetExpenseById(int expenseId)
        {
            return await _context.Expenses
                .Include(e => e.PaidBy)
                .Include(e => e.Group)
                .FirstOrDefaultAsync(e => e.ExpenseID == expenseId);
        }

        // ✅ Calculate Total Expenses for a Group
        public async Task<decimal> CalculateTotalExpenses(int groupId)
        {
            return await _context.Expenses
                .Where(e => e.GroupID == groupId)
                .SumAsync(e => e.Amount);
        }
    }
}
