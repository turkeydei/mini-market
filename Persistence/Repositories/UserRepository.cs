﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Application.Features.Interface.IRepositories;

namespace Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MiniMarketDbContext _context;

        public UserRepository(MiniMarketDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(x => x.HoaDons)
                .FirstOrDefaultAsync(x => x.MaUser == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetByEmailAndPasswordAsync(string email, string passwordHash)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email && x.MatKhau == passwordHash);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Users.AnyAsync(x => x.MaUser == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }
    }
}