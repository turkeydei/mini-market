using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly MiniMarketDbContext _context;
    
    // Repositories
    private IRepository<User>? _users;
    private IRepository<Loai>? _loais;
    private IRepository<HangHoa>? _hangHoas;
    private IRepository<HoaDon>? _hoaDons;
    private IRepository<ChiTietHD>? _chiTietHDs;
    private IRepository<PaymentTransaction>? _paymentTransactions;
    private IRepository<ProductReview>? _productReviews;

    public UnitOfWork(MiniMarketDbContext context)
    {
        _context = context;
    }

    public IRepository<User> Users => 
        _users ??= new Repository<User>(_context);
    
    public IRepository<Loai> Loais => 
        _loais ??= new Repository<Loai>(_context);
    
    public IRepository<HangHoa> HangHoas => 
        _hangHoas ??= new Repository<HangHoa>(_context);
    
    public IRepository<HoaDon> HoaDons => 
        _hoaDons ??= new Repository<HoaDon>(_context);
    
    public IRepository<ChiTietHD> ChiTietHDs => 
        _chiTietHDs ??= new Repository<ChiTietHD>(_context);
    
    public IRepository<PaymentTransaction> PaymentTransactions => 
        _paymentTransactions ??= new Repository<PaymentTransaction>(_context);
    
    public IRepository<ProductReview> ProductReviews => 
        _productReviews ??= new Repository<ProductReview>(_context);

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public int Save()
    {
        return _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
    
    // Complex queries
    public async Task<HoaDon?> GetOrderByIdWithDetailsAsync(int orderId, int userId)
    {
        return await _context.HoaDons
            .Include(h => h.ChiTietHDs)
                .ThenInclude(ct => ct.HangHoa)
                    .ThenInclude(hh => hh!.Loai)
            .Include(h => h.PaymentTransaction)
            .Include(h => h.User)
            .FirstOrDefaultAsync(h => h.MaHD == orderId && h.MaUser == userId);
    }
    
    public async Task<IEnumerable<HoaDon>> GetOrdersByUserWithDetailsAsync(int userId)
    {
        return await _context.HoaDons
            .Include(h => h.ChiTietHDs)
                .ThenInclude(ct => ct.HangHoa)
            .Include(h => h.PaymentTransaction)
            .Where(h => h.MaUser == userId)
            .OrderByDescending(h => h.NgayDat)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<HangHoa>> GetAllProductsWithCategoryAsync()
    {
        return await _context.HangHoas
            .Include(h => h.Loai)
            .OrderByDescending(h => h.SoLanXem)
            .Take(20)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<HangHoa>> GetProductsByCategoryWithDetailsAsync(int categoryId)
    {
        return await _context.HangHoas
            .Include(h => h.Loai)
            .Where(h => h.MaLoai == categoryId)
            .OrderByDescending(h => h.SoLanXem)
            .ToListAsync();
    }
    
    public async Task<HangHoa?> GetProductByIdWithCategoryAsync(int id)
    {
        return await _context.HangHoas
            .Include(h => h.Loai)
            .FirstOrDefaultAsync(h => h.MaHH == id);
    }
    
    public async Task<HoaDon?> GetOrderForCancelAsync(int orderId, int userId)
    {
        return await _context.HoaDons
            .Include(h => h.PaymentTransaction)
            .Include(h => h.ChiTietHDs)
            .FirstOrDefaultAsync(h => h.MaHD == orderId && h.MaUser == userId);
    }
}

