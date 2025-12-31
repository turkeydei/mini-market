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
    IRepository<ProductReview> ProductReviews { get; }
    
    // Save changes
    Task<int> SaveAsync();
    int Save();
    
    // Complex queries
    Task<HoaDon?> GetOrderByIdWithDetailsAsync(int orderId, int userId);
    Task<IEnumerable<HoaDon>> GetOrdersByUserWithDetailsAsync(int userId);
    Task<IEnumerable<HangHoa>> GetAllProductsWithCategoryAsync();
    Task<IEnumerable<HangHoa>> GetProductsByCategoryWithDetailsAsync(int categoryId);
    Task<HangHoa?> GetProductByIdWithCategoryAsync(int id);
    Task<HoaDon?> GetOrderForCancelAsync(int orderId, int userId);
}

