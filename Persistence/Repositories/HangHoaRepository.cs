using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Application.Features.Interface.IRepositories;

namespace Persistence.Repositories
{
    public class HangHoaRepository : IHangHoaRepository
    {
        private readonly MiniMarketDbContext _context;

        public HangHoaRepository(MiniMarketDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HangHoa>> GetAllAsync()
        {
            return await _context.HangHoas.Include(x => x.Loai).ToListAsync();
        }

        public async Task<HangHoa?> GetByIdAsync(int id)
        {
            return await _context.HangHoas.Include(x => x.Loai)
                .FirstOrDefaultAsync(x => x.MaHH == id);
        }

        public async Task<HangHoa?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.HangHoas
                .Include(x => x.Loai)
                .Include(x => x.ChiTietHDs)
                .FirstOrDefaultAsync(x => x.MaHH == id);
        }

        public async Task<IEnumerable<HangHoa>> GetByCategoryAsync(int maLoai)
        {
            return await _context.HangHoas
                .Include(x => x.Loai)
                .Where(x => x.MaLoai == maLoai)
                .ToListAsync();
        }

        public async Task<IEnumerable<HangHoa>> SearchAsync(string keyword)
        {
            return await _context.HangHoas
                .Include(x => x.Loai)
                .Where(x => x.TenHH.Contains(keyword) || 
                           (x.MoTa != null && x.MoTa.Contains(keyword)))
                .ToListAsync();
        }

        public async Task AddAsync(HangHoa hangHoa)
        {
            await _context.HangHoas.AddAsync(hangHoa);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(HangHoa hangHoa)
        {
            _context.HangHoas.Update(hangHoa);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var hangHoa = await _context.HangHoas.FindAsync(id);
            if (hangHoa != null)
            {
                _context.HangHoas.Remove(hangHoa);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.HangHoas.AnyAsync(x => x.MaHH == id);
        }
    }
}