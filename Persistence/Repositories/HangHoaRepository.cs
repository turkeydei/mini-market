using Application.Features.Interface;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class HangHoaRepository : IHangHoaService
    {
        private readonly AppDbContext _context;

        public HangHoaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HangHoa>> GetAll()
        {
            return await _context.HangHoas.Include(x => x.Loai).ToListAsync();
        }

        public async Task<HangHoa?> GetById(int id)
        {
            return await _context.HangHoas.Include(x => x.Loai)
                .FirstOrDefaultAsync(x => x.MaHH == id);
        }

        public async Task Add(HangHoa hh)
        {
            await _context.HangHoas.AddAsync(hh);
            await _context.SaveChangesAsync();
        }

        public async Task Update(HangHoa hh)
        {
            _context.HangHoas.Update(hh);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var hh = await _context.HangHoas.FindAsync(id);
            if (hh != null)
            {
                _context.HangHoas.Remove(hh);
                await _context.SaveChangesAsync();
            }
        }
    }
}