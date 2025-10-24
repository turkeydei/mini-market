using Application.Features.Interface;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories
{
    public class TrangThaiRepository : ITrangThaiService
    {
        private readonly AppDbContext _context;

        public TrangThaiRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TrangThai>> GetAll()
        {
            return await _context.TrangThais.ToListAsync();
        }

        public async Task<TrangThai?> GetById(int id)
        {
            return await _context.TrangThais.FindAsync(id);
        }

        public async Task Add(TrangThai tt)
        {
            await _context.TrangThais.AddAsync(tt);
            await _context.SaveChangesAsync();
        }

        public async Task Update(TrangThai tt)
        {
            _context.TrangThais.Update(tt);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var tt = await _context.TrangThais.FindAsync(id);
            if (tt != null)
            {
                _context.TrangThais.Remove(tt);
                await _context.SaveChangesAsync();
            }
        }
    }
}