using Application.Features.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebShop.Controllers;

[Authorize]
public class TransactionHistoryController : Controller
{
    private readonly ITransactionHistoryService _transactionService;
    private readonly IAuthService _authService;
    private readonly ILogger<TransactionHistoryController> _logger;

    public TransactionHistoryController(
        ITransactionHistoryService transactionService,
        IAuthService authService,
        ILogger<TransactionHistoryController> logger)
    {
        _transactionService = transactionService;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem lịch sử giao dịch";
                return RedirectToAction("Login", "Authentication");
            }

            var transactions = await _transactionService.GetByUserIdAsync(currentUser.MaUser);
            return View(transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading transaction history");
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải lịch sử giao dịch";
            return View(new List<Domain.Entities.LichSuGiaoDich>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var transaction = await _transactionService.GetByIdAsync(id);
            if (transaction == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy giao dịch";
                return RedirectToAction("Index");
            }

            // Check if current user owns this transaction
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null || transaction.HoaDon?.MaUser != currentUser.MaUser)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền xem giao dịch này";
                return RedirectToAction("Index");
            }

            return View(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading transaction details for ID {Id}", id);
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin giao dịch";
            return RedirectToAction("Index");
        }
    }
}

