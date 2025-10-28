using Application.Interfaces;
using Domain.Entities;

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
}

