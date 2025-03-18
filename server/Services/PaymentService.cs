using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;

namespace server.Services
{
    public class PaymentService
    {
        private readonly AppDbContext _context;

        public PaymentService(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Add a New Payment
        public async Task<Payment> AddPayment(Payment payment)
        {
            payment.PaidAt = DateTime.UtcNow; // Set payment timestamp
            _context.Payments.Add(payment);

            // Adjust balances for the payer and payee
            await AdjustBalancesAfterPayment(payment);

            await _context.SaveChangesAsync();
            return payment;
        }

        // ✅ Adjust Balances After Payment (Updates BalanceOwed for Payer and Payee)
        private async Task AdjustBalancesAfterPayment(Payment payment)
        {
            var payer = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupID == payment.GroupID && gm.UserID == payment.PayerID);
            var payee = await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.GroupID == payment.GroupID && gm.UserID == payment.PayeeID);

            if (payer != null && payee != null)
            {
                payer.BalanceOwed -= payment.Amount; // Reduce the payer's debt
                payee.BalanceOwed += payment.Amount; // Increase the payee's credit
            }
        }

        // ✅ Get All Payments for a Group
        public async Task<List<Payment>> GetPaymentsByGroup(int groupId)
        {
            return await _context.Payments
                .Where(p => p.GroupID == groupId)
                .Include(p => p.Payer) // Include Payer details
                .Include(p => p.Payee) // Include Payee details
                .ToListAsync();
        }

        // ✅ Get Payment Details by ID
        public async Task<Payment?> GetPaymentById(int paymentId)
        {
            return await _context.Payments
                .Include(p => p.Payer)
                .Include(p => p.Payee)
                .Include(p => p.Group)
                .FirstOrDefaultAsync(p => p.PaymentID == paymentId);
        }

        // ✅ Calculate Total Payments Made by a User in a Group
        public async Task<decimal> CalculateTotalPaymentsMadeByUser(int userId, int groupId)
        {
            return await _context.Payments
                .Where(p => p.PayerID == userId && p.GroupID == groupId)
                .SumAsync(p => p.Amount);
        }

        // ✅ Calculate Total Payments Received by a User in a Group
        public async Task<decimal> CalculateTotalPaymentsReceivedByUser(int userId, int groupId)
        {
            return await _context.Payments
                .Where(p => p.PayeeID == userId && p.GroupID == groupId)
                .SumAsync(p => p.Amount);
        }
    }
}
