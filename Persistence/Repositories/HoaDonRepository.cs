using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Application.Features.Interface.IRepositories;

namespace Persistence.Repositories
{
    public class HoaDonRepository : IHoaDonRepository
    {
        private readonly MiniMarketDbContext _context;

        public HoaDonRepository(MiniMarketDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HoaDon>> GetAllAsync()
        {
            return await _context.HoaDons
                .Include(x => x.User)
                .Include(x => x.TrangThai)
                .Include(x => x.ChiTietHDs)
                .ToListAsync();
        }

        public async Task<HoaDon?> GetByIdAsync(int id)
        {
            return await _context.HoaDons
                .Include(x => x.User)
                .Include(x => x.TrangThai)
                .FirstOrDefaultAsync(x => x.MaHD == id);
        }

        public async Task<HoaDon?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.HoaDons
                .Include(x => x.User)
                .Include(x => x.TrangThai)
                .Include(x => x.ChiTietHDs)
                    .ThenInclude(ct => ct.HangHoa)
                .FirstOrDefaultAsync(x => x.MaHD == id);
        }

        public async Task<IEnumerable<HoaDon>> GetByUserIdAsync(int maUser)
        {
            return await _context.HoaDons
                .Include(x => x.User)
                .Include(x => x.TrangThai)
                .Include(x => x.ChiTietHDs)
                .Where(x => x.MaUser == maUser)
                .OrderByDescending(x => x.NgayDat)
                .ToListAsync();
        }

        public async Task<IEnumerable<HoaDon>> GetByStatusAsync(int maTrangThai)
        {
            return await _context.HoaDons
                .Include(x => x.User)
                .Include(x => x.TrangThai)
                .Include(x => x.ChiTietHDs)
                .Where(x => x.MaTrangThai == maTrangThai)
                .OrderByDescending(x => x.NgayDat)
                .ToListAsync();
        }

        public async Task AddAsync(HoaDon hoaDon)
        {
            await _context.HoaDons.AddAsync(hoaDon);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(HoaDon hoaDon)
        {
            _context.HoaDons.Update(hoaDon);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon != null)
            {
                _context.HoaDons.Remove(hoaDon);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.HoaDons.AnyAsync(x => x.MaHD == id);
        }
    }
}