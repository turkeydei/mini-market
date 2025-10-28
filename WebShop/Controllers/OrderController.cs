using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace WebShop.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly MiniMarketDbContext _context;

    public OrderController(MiniMarketDbContext context)
    {
        _context = context;
    }

    // GET: /Order/History
    public async Task<IActionResult> History()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var orders = await _context.HoaDons
            .Include(h => h.ChiTietHDs)
                .ThenInclude(ct => ct.HangHoa)
            .Include(h => h.PaymentTransaction)
            .Where(h => h.MaUser == userId)
            .OrderByDescending(h => h.NgayDat)
            .ToListAsync();

        return View(orders);
    }

    // GET: /Order/Success/{id}
    public async Task<IActionResult> Success(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var order = await _context.HoaDons
            .Include(h => h.ChiTietHDs)
                .ThenInclude(ct => ct.HangHoa)
            .Include(h => h.PaymentTransaction)
            .Include(h => h.User)
            .FirstOrDefaultAsync(h => h.MaHD == id && h.MaUser == userId);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // GET: /Order/Details/{id}
    public async Task<IActionResult> Details(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var order = await _context.HoaDons
            .Include(h => h.ChiTietHDs)
                .ThenInclude(ct => ct.HangHoa)
                    .ThenInclude(hh => hh!.Loai)
            .Include(h => h.PaymentTransaction)
            .Include(h => h.User)
            .FirstOrDefaultAsync(h => h.MaHD == id && h.MaUser == userId);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    // POST: /Order/CancelOrder/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var order = await _context.HoaDons
            .Include(h => h.PaymentTransaction)
            .FirstOrDefaultAsync(h => h.MaHD == id && h.MaUser == userId);

        if (order == null)
        {
            return NotFound();
        }

        // Chỉ cho phép hủy đơn hàng ở trạng thái Pending
        if (order.Status == "Pending")
        {
            order.Status = "Cancelled";
            
            if (order.PaymentTransaction != null)
            {
                order.PaymentTransaction.Status = "Cancelled";
                order.PaymentTransaction.NgayCapNhat = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công";
        }
        else
        {
            TempData["ErrorMessage"] = "Không thể hủy đơn hàng này";
        }

        return RedirectToAction(nameof(History));
    }
}

