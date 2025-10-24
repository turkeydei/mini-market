using Application.Features.Interface;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class HoaDonRepository : IHoaDonService
    {
        private readonly AppDbContext _context;

        public HoaDonRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HoaDon>> GetAll()
        {
            return await _context.HoaDons
                .Include(h => h.User)
                .Include(h => h.TrangThai)
                .Include(h => h.ChiTietHDs)
                .ThenInclude(ct => ct.HangHoa)
                .ToListAsync();
        }

        public async Task<HoaDon?> GetById(int id)
        {
            return await _context.HoaDons
                .Include(h => h.User)
                .Include(h => h.TrangThai)
                .Include(h => h.ChiTietHDs)
                .ThenInclude(ct => ct.HangHoa)
                .FirstOrDefaultAsync(h => h.MaHD == id);
        }

        public async Task Add(HoaDon hd)
        {
            await _context.HoaDons.AddAsync(hd);
            await _context.SaveChangesAsync();
        }

        public async Task Update(HoaDon hd)
        {
            _context.HoaDons.Update(hd);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var hd = await _context.HoaDons.FindAsync(id);
            if (hd != null)
            {
                _context.HoaDons.Remove(hd);
                await _context.SaveChangesAsync();
            }
        }
    }
}