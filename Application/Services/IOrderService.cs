using Domain.Entities;

namespace Application.Services;

public interface IOrderService
{
    Task<HoaDon> CreateOrderAsync(HoaDon order, List<ChiTietHD> orderDetails);
    Task<HoaDon?> GetOrderByIdAsync(int orderId, int userId);
    Task<IEnumerable<HoaDon>> GetOrdersByUserAsync(int userId);
    Task<bool> CancelOrderAsync(int orderId, int userId);
    Task UpdateOrderStatusAsync(int orderId, string status);
    Task<PaymentTransaction> CreatePaymentTransactionAsync(PaymentTransaction transaction);
    Task UpdatePaymentStatusAsync(int transactionId, string status);
}

