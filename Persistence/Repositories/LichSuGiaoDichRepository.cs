using Application.Features.Interface;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence; // <-- để nhìn thấy MiniMarketDbContext

namespace Persistence.Repositories
{
    public class LichSuGiaoDichRepository : ILichSuGiaoDich
    {
        private readonly MiniMarketDbContext _context;

        // TÊN CONSTRUCTOR PHẢI TRÙNG TÊN LỚP
        public LichSuGiaoDichRepository(MiniMarketDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LichSuGiaoDich>> GetAll()
        {
            return await _context.LichSuGiaoDiches
                .Include(p => p.HoaDon)
                .ThenInclude(h => h.User)
                .ToListAsync();
        }

        public async Task<LichSuGiaoDich?> GetById(int id)
        {
            return await _context.LichSuGiaoDiches
                .Include(p => p.HoaDon)
                .ThenInclude(h => h.User)
                .FirstOrDefaultAsync(p => p.MaGiaoDich == id);
        }

        public async Task Add(LichSuGiaoDich transaction)
        {
            await _context.LichSuGiaoDiches.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task Update(LichSuGiaoDich transaction)
        {
            _context.LichSuGiaoDiches.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var transaction = await _context.LichSuGiaoDiches.FindAsync(id);
            if (transaction != null)
            {
                _context.LichSuGiaoDiches.Remove(transaction);
                await _context.SaveChangesAsync();
            }
        }
    }
}