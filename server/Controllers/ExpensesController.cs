using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpensesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ExpensesController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get All Expenses for a Specific Group
        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<Expense>>> GetExpensesByGroup(int groupId)
        {
            var expenses = await _context.Expenses
                .Where(e => e.GroupID == groupId)
                .Include(e => e.PaidBy) // Include the user who paid
                .ToListAsync();

            return Ok(expenses);
        }

        // ✅ Get Expense by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Expense>> GetExpense(int id)
        {
            var expense = await _context.Expenses
                .Include(e => e.PaidBy)
                .Include(e => e.Group)
                .FirstOrDefaultAsync(e => e.ExpenseID == id);

            if (expense == null)
                return NotFound();

            return Ok(expense);
        }

        // ✅ Create a New Expense
        [HttpPost]
        public async Task<ActionResult<Expense>> PostExpense([FromBody] Expense expense)
        {
            // Validate if the group exists
            var group = await _context.Groups.FindAsync(expense.GroupID);
            if (group == null)
                return BadRequest(new { message = "Group does not exist" });

            // Add the expense
            expense.CreatedAt = DateTime.UtcNow;
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetExpense), new { id = expense.ExpenseID }, expense);
        }

        // ✅ Delete Expense by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);

            if (expense == null)
                return NotFound();

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
