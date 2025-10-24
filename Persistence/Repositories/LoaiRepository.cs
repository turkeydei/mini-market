
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class LoaiRepository : ILoaiService
    {
        private readonly AppDbContext _context;

        public LoaiRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Loai>> GetAll()
        {
            return await _context.Loais.ToListAsync();
        }

        public async Task<Loai?> GetById(int id)
        {
            return await _context.Loais.FindAsync(id);
        }

        public async Task Add(Loai loai)
        {
            await _context.Loais.AddAsync(loai);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Loai loai)
        {
            _context.Loais.Update(loai);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var loai = await _context.Loais.FindAsync(id);
            if (loai != null)
            {
                _context.Loais.Remove(loai);
                await _context.SaveChangesAsync();
            }
        }
    }
}