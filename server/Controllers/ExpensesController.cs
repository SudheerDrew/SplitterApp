using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;
using server.Services;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpensesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ExpenseService _expenseService;

        public ExpensesController(AppDbContext context)
        {
            _context = context;
            _expenseService = new ExpenseService(_context); // Initialize service for splitting logic
        }

        // ✅ Get All Expenses for a Specific Group
        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpensesByGroup(int groupId)
        {
            // Check if the group exists
            var group = await _context.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.GroupID == groupId);
            if (group == null)
                return NotFound(new { message = "Group does not exist" });

            // Fetch expenses for the group
            var expenses = await _expenseService.GetExpensesByGroup(groupId);

            return Ok(expenses);
        }

        // ✅ Get Expense by ID
    [HttpGet("{id}")]
public async Task<ActionResult<Expense>> GetExpense(int id)
{
    // Fetch the expense along with related data
    var expense = await _context.Expenses
        .Include(e => e.Group) // Include Group navigation property
            .ThenInclude(g => g.Expenses) // Include related Group Expenses
        .Include(e => e.PaidBy) // Include PaidBy (User) navigation property
            .ThenInclude(u => u.ExpensesPaid) // Include related PaidBy User Expenses
        .FirstOrDefaultAsync(e => e.ExpenseID == id);

    // Check if the expense exists
    if (expense == null)
    {
        return NotFound(new { message = "Expense not found" });
    }

    return Ok(expense);
}


        // ✅ Create a New Expense (With Splitting Logic)
[HttpPost]
public async Task<ActionResult<Expense>> PostExpense([FromBody] Expense expense)
{
    Console.WriteLine($"Incoming Expense Payload: Description={expense.Description}, Amount={expense.Amount}, GroupID={expense.GroupID}, UserID={expense.UserID}");
    // Validate if the group exists
    var group = await _context.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.GroupID == expense.GroupID);
    if (group == null)
        return BadRequest(new { message = "Group does not exist" });

    // Validate if the user exists
    var user = await _context.Users.FindAsync(expense.UserID);
    if (user == null)
    {
        return BadRequest(new { message = $"UserID {expense.UserID} does not exist" });
    }

    try
    {
        // Add expense and split costs among members
        var createdExpense = await _expenseService.AddExpense(expense);

        return CreatedAtAction(nameof(GetExpense), new { id = createdExpense.ExpenseID }, createdExpense);
    }
    catch (Exception ex)
    {
        // Return detailed error if splitting or saving fails
        return StatusCode(500, new { message = "Failed to add expense", details = ex.Message });
    }
}

        // ✅ Update an Existing Expense
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] Expense updatedExpense)
        {
            // Validate if the expense ID matches
            if (id != updatedExpense.ExpenseID)
            {
                return BadRequest(new { message = "Expense ID mismatch" });
            }

            // Check if the expense exists in the database
            var existingExpense = await _context.Expenses
                .Include(e => e.Group) // Ensure Group is loaded
                .FirstOrDefaultAsync(e => e.ExpenseID == id);

            if (existingExpense == null)
            {
                return NotFound(new { message = "Expense not found" });
            }

            // Update the expense properties
            existingExpense.Description = updatedExpense.Description;
            existingExpense.Amount = updatedExpense.Amount;
            existingExpense.UserID = updatedExpense.UserID; // PaidBy

            // Optional: Add logic to recalculate balances in GroupMembers
            if (existingExpense.Group != null)
            {
                var splitAmount = updatedExpense.Amount / existingExpense.Group.Members.Count; // Equal split
                foreach (var member in existingExpense.Group.Members)
                {
                    var groupMember = await _context.GroupMembers
                        .FirstOrDefaultAsync(gm => gm.GroupID == existingExpense.GroupID && gm.UserID == member.UserID);

                    if (groupMember != null)
                    {
                        groupMember.BalanceOwed = splitAmount; // Adjust logic if needed
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync(); // Save changes to the database
                return NoContent(); // 204 - Successful update, no content to return
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update expense", details = ex.Message });
            }
        }

        // ✅ Delete Expense by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);

            if (expense == null)
                return NotFound(new { message = "Expense not found" });

            // Remove the expense
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
