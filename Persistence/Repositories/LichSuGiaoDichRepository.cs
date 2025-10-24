using Application.Features.Interface;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class PaymentRepository : IPaymentService
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PaymentTransaction>> GetAll()
        {
            return await _context.PaymentTransactions
                .Include(p => p.HoaDon)
                .ThenInclude(h => h.User)
                .ToListAsync();
        }

        public async Task<PaymentTransaction?> GetById(int id)
        {
            return await _context.PaymentTransactions
                .Include(p => p.HoaDon)
                .ThenInclude(h => h.User)
                .FirstOrDefaultAsync(p => p.MaGiaoDich == id);
        }

        public async Task Add(PaymentTransaction transaction)
        {
            await _context.PaymentTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task Update(PaymentTransaction transaction)
        {
            _context.PaymentTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var transaction = await _context.PaymentTransactions.FindAsync(id);
            if (transaction != null)
            {
                _context.PaymentTransactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
        }
    }
}