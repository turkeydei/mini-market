using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MiniMarketDbContext _context;

    public OrderService(IUnitOfWork unitOfWork, MiniMarketDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<HoaDon> CreateOrderAsync(HoaDon order, List<ChiTietHD> orderDetails)
    {
        // Thêm đơn hàng
        await _unitOfWork.HoaDons.AddAsync(order);
        await _unitOfWork.SaveAsync();

        // Thêm chi tiết đơn hàng
        foreach (var detail in orderDetails)
        {
            detail.MaHD = order.MaHD;
            await _unitOfWork.ChiTietHDs.AddAsync(detail);
            
            // Cập nhật số lượng tồn
            var product = await _unitOfWork.HangHoas.GetByIdAsync(detail.MaHH);
            if (product != null)
            {
                product.SoLuongTon -= detail.SoLuong;
                _unitOfWork.HangHoas.Update(product);
            }
        }

        await _unitOfWork.SaveAsync();
        return order;
    }

    public async Task<HoaDon?> GetOrderByIdAsync(int orderId, int userId)
    {
        return await _context.HoaDons
            .Include(h => h.ChiTietHDs)
                .ThenInclude(ct => ct.HangHoa)
                    .ThenInclude(hh => hh!.Loai)
            .Include(h => h.PaymentTransaction)
            .Include(h => h.User)
            .FirstOrDefaultAsync(h => h.MaHD == orderId && h.MaUser == userId);
    }

    public async Task<IEnumerable<HoaDon>> GetOrdersByUserAsync(int userId)
    {
        return await _context.HoaDons
            .Include(h => h.ChiTietHDs)
                .ThenInclude(ct => ct.HangHoa)
            .Include(h => h.PaymentTransaction)
            .Where(h => h.MaUser == userId)
            .OrderByDescending(h => h.NgayDat)
            .ToListAsync();
    }

    public async Task<bool> CancelOrderAsync(int orderId, int userId)
    {
        var order = await _context.HoaDons
            .Include(h => h.PaymentTransaction)
            .Include(h => h.ChiTietHDs)
            .FirstOrDefaultAsync(h => h.MaHD == orderId && h.MaUser == userId);

        if (order == null || order.Status != "Pending")
            return false;

        // Cập nhật trạng thái đơn hàng
        order.Status = "Cancelled";
        
        // Cập nhật trạng thái thanh toán
        if (order.PaymentTransaction != null)
        {
            order.PaymentTransaction.Status = "Cancelled";
            order.PaymentTransaction.NgayCapNhat = DateTime.Now;
        }

        // Hoàn lại số lượng tồn kho
        if (order.ChiTietHDs != null)
        {
            foreach (var detail in order.ChiTietHDs)
            {
                var product = await _unitOfWork.HangHoas.GetByIdAsync(detail.MaHH);
                if (product != null)
                {
                    product.SoLuongTon += detail.SoLuong;
                    _unitOfWork.HangHoas.Update(product);
                }
            }
        }

        await _unitOfWork.SaveAsync();
        return true;
    }

    public async Task UpdateOrderStatusAsync(int orderId, string status)
    {
        var order = await _unitOfWork.HoaDons.GetByIdAsync(orderId);
        if (order != null)
        {
            order.Status = status;
            if (status == "Completed")
            {
                order.NgayGiao = DateTime.Now;
            }
            _unitOfWork.HoaDons.Update(order);
            await _unitOfWork.SaveAsync();
        }
    }

    public async Task<PaymentTransaction> CreatePaymentTransactionAsync(PaymentTransaction transaction)
    {
        await _unitOfWork.PaymentTransactions.AddAsync(transaction);
        await _unitOfWork.SaveAsync();
        return transaction;
    }

    public async Task UpdatePaymentStatusAsync(int transactionId, string status)
    {
        var transaction = await _unitOfWork.PaymentTransactions.GetByIdAsync(transactionId);
        if (transaction != null)
        {
            transaction.Status = status;
            transaction.NgayCapNhat = DateTime.Now;
            _unitOfWork.PaymentTransactions.Update(transaction);
            await _unitOfWork.SaveAsync();
        }
    }
}

