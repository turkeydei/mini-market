using Domain.Entities;

namespace Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // Repositories
    IRepository<User> Users { get; }
    IRepository<Loai> Loais { get; }
    IRepository<HangHoa> HangHoas { get; }
    IRepository<HoaDon> HoaDons { get; }
    IRepository<ChiTietHD> ChiTietHDs { get; }
    IRepository<PaymentTransaction> PaymentTransactions { get; }
    
    // Save changes
    Task<int> SaveAsync();
    int Save();
}

