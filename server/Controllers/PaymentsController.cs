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
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentsController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get All Payments for the Logged-In User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            var userId = int.Parse(User.FindFirst("UserID")!.Value); // Get user ID from JWT claims

            var payments = await _context.Payments
                .Where(p => p.PayerID == userId || p.PayeeID == userId) // Fetch payments involving the user
                .Include(p => p.Payer) // Include Payer details
                .Include(p => p.Payee) // Include Payee details
                .Include(p => p.Group) // Include Group details
                .ToListAsync();

            return Ok(payments);
        }

        // ✅ Get Payment by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = await _context.Payments
                .Include(p => p.Payer)
                .Include(p => p.Payee)
                .Include(p => p.Group)
                .FirstOrDefaultAsync(p => p.PaymentID == id);

            if (payment == null)
                return NotFound();

            return Ok(payment);
        }

        // ✅ Create a New Payment
        [HttpPost]
        public async Task<ActionResult<Payment>> PostPayment([FromBody] Payment payment)
        {
            // Validate that the Group exists
            var group = await _context.Groups.FindAsync(payment.GroupID);
            if (group == null)
                return BadRequest(new { message = "Group does not exist" });

            // Validate that the Payee exists
            var payee = await _context.Users.FindAsync(payment.PayeeID);
            if (payee == null)
                return BadRequest(new { message = "Payee does not exist" });

            // Validate that the Payer exists
            var payer = await _context.Users.FindAsync(payment.PayerID);
            if (payer == null)
                return BadRequest(new { message = "Payer does not exist" });

            // Add the payment
            payment.PaidAt = DateTime.UtcNow;
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPayment), new { id = payment.PaymentID }, payment);
        }

        // ✅ Delete Payment
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound();

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
