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
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PaymentService _paymentService;

        public PaymentsController(AppDbContext context)
        {
            _context = context;
            _paymentService = new PaymentService(_context); // Initialize service for payment logic
        }

        // ✅ Get All Payments for a Group
        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPaymentsByGroup(int groupId)
        {
            // Validate if the group exists
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
                return NotFound(new { message = "Group does not exist" });

            // Fetch payments
            var payments = await _paymentService.GetPaymentsByGroup(groupId);
            return Ok(payments);
        }

        // ✅ Get Payment by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            var payment = await _paymentService.GetPaymentById(id);
            if (payment == null)
                return NotFound(new { message = "Payment not found" });

            return Ok(payment);
        }

        // ✅ Create a New Payment
[HttpPost]
public async Task<ActionResult<Payment>> PostPayment([FromBody] Payment payment)
{
    // Validate if the group exists
    var group = await _context.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.GroupID == payment.GroupID);
    if (group == null)
        return BadRequest(new { message = "Group does not exist" });

    // Ensure payer and payee are part of the group
    var isPayerMember = group.Members.Any(m => m.UserID == payment.PayerID);
    var isPayeeMember = group.Members.Any(m => m.UserID == payment.PayeeID);

    if (!isPayerMember || !isPayeeMember)
        return BadRequest(new { message = "Payer or Payee is not a member of the group" });

    try
    {
        // Set the PaidAt field to the current UTC time
        payment.PaidAt = DateTime.UtcNow;

        // Add the payment to the database
        _context.Payments.Add(payment);

        // Adjust balances in the GroupMembers table
        var payer = group.Members.FirstOrDefault(m => m.UserID == payment.PayerID);
        var payee = group.Members.FirstOrDefault(m => m.UserID == payment.PayeeID);

        if (payer != null)
            payer.BalanceOwed -= payment.Amount; // Reduce payer's owed balance
        if (payee != null)
            payee.BalanceOwed += payment.Amount; // Increase payee's owed balance

        // Save changes to the database
        await _context.SaveChangesAsync();

        // Return the created payment
        return CreatedAtAction(nameof(GetPayment), new { id = payment.PaymentID }, payment);
    }
    catch (Exception ex)
    {
        // Handle errors
        return StatusCode(500, new { message = "Failed to process payment", details = ex.Message });
    }
}


        // ✅ Delete Paymentx
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
                return NotFound(new { message = "Payment not found" });

            // Remove the payment
            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
