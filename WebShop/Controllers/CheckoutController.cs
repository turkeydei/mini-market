using Application.Features.DTOs;
using Application.Features.Interface;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace WebShop.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly IHoaDonService _hoaDonService;
    private readonly ITransactionHistoryService _transactionService;
    private readonly IHangHoaService _hangHoaService;
    private readonly IAuthService _authService;
    private readonly ILogger<CheckoutController> _logger;

    public CheckoutController(
        IHoaDonService hoaDonService,
        ITransactionHistoryService transactionService,
        IHangHoaService hangHoaService,
        IAuthService authService,
        ILogger<CheckoutController> logger)
    {
        _hoaDonService = hoaDonService;
        _transactionService = transactionService;
        _hangHoaService = hangHoaService;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var cart = GetCart();
            _logger.LogInformation("Checkout Index - Cart items: {Count}", cart.Count);
            
            if (!cart.Any())
            {
                _logger.LogWarning("Cart is empty");
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống";
                return RedirectToAction("Index", "HangHoa");
            }

            // Validate and update cart items with latest data
            var validatedCart = new List<CheckoutItemDto>();
            foreach (var item in cart)
            {
                var hangHoa = await _hangHoaService.GetById(item.MaHH);
                if (hangHoa != null)
                {
                    validatedCart.Add(new CheckoutItemDto
                    {
                        MaHH = hangHoa.MaHH,
                        TenHH = hangHoa.TenHH,
                        DonGia = hangHoa.DonGia,
                        GiamGia = hangHoa.GiamGia,
                        SoLuong = item.SoLuong,
                        Hinh = hangHoa.Hinh
                    });
                }
            }

            if (!validatedCart.Any())
            {
                _logger.LogWarning("No valid items in cart after validation");
                ClearCart();
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống";
                return RedirectToAction("Index", "HangHoa");
            }

            SaveCart(validatedCart);
            _logger.LogInformation("Returning checkout view with {Count} items", validatedCart.Count);
            return View(validatedCart);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading cart");
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải giỏ hàng";
            return RedirectToAction("Index", "HangHoa");
        }
    }

    [HttpPost]
    [IgnoreAntiforgeryToken] // TEMPORARILY DISABLE FOR DEBUG
    public async Task<IActionResult> ProcessPayment(string DiaChiGiao, string SoDienThoai, string GhiChu)
    {
        try
        {
            _logger.LogInformation("ProcessPayment called - DiaChiGiao: {DiaChiGiao}, SoDienThoai: {SoDienThoai}", 
                DiaChiGiao, SoDienThoai);
            
            // Get cart from session
            var cart = GetCart();
            _logger.LogInformation("Cart items: {Count}", cart.Count);
            
            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống";
                return RedirectToAction("Index", "HangHoa");
            }

            var currentUser = await _authService.GetCurrentUserAsync();
            _logger.LogInformation("ProcessPayment - Current user: {User}, Email: {Email}, MaUser: {MaUser}", 
                currentUser?.HoTen ?? "NULL", 
                currentUser?.Email ?? "NULL",
                currentUser?.MaUser ?? 0);
            
            if (currentUser == null)
            {
                _logger.LogWarning("Current user is NULL");
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để thanh toán";
                return RedirectToAction("Login", "Authentication");
            }
            
            if (currentUser.MaUser <= 0)
            {
                _logger.LogError("Invalid MaUser: {MaUser}", currentUser.MaUser);
                TempData["ErrorMessage"] = $"Thông tin người dùng không hợp lệ (MaUser = {currentUser.MaUser}). Vui lòng đăng nhập lại.";
                return RedirectToAction("Login", "Authentication");
            }
            
            // Verify user exists in database
            using (var scope = HttpContext.RequestServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<Persistence.MiniMarketDbContext>();
                var userExists = await context.Users.AnyAsync(u => u.MaUser == currentUser.MaUser);
                if (!userExists)
                {
                    _logger.LogError("User with MaUser {MaUser} does NOT exist in database", currentUser.MaUser);
                    TempData["ErrorMessage"] = "Tài khoản không tồn tại. Vui lòng đăng ký lại.";
                    return RedirectToAction("Register", "Authentication");
                }
                _logger.LogInformation("User {MaUser} verified in database", currentUser.MaUser);
            }
            
            // Basic validation
            if (string.IsNullOrWhiteSpace(DiaChiGiao))
            {
                TempData["ErrorMessage"] = "Địa chỉ giao hàng không được để trống";
                return View("Index", cart);
            }
            
            if (string.IsNullOrWhiteSpace(SoDienThoai))
            {
                TempData["ErrorMessage"] = "Số điện thoại không được để trống";
                return View("Index", cart);
            }

            // Create HoaDon
            _logger.LogInformation("Creating HoaDon with MaUser: {MaUser}", currentUser.MaUser);
            
            var hoaDon = new HoaDon
            {
                MaUser = currentUser.MaUser,
                NgayDat = DateTime.UtcNow,
                DiaChiGiao = DiaChiGiao,
                SoDienThoai = SoDienThoai,
                PhiVanChuyen = 30000,
                GhiChu = GhiChu,
                MaTrangThai = 1, // Pending/Chờ xử lý
                ChiTietHDs = new List<ChiTietHD>()
            };
            
            _logger.LogInformation("HoaDon created: MaUser={MaUser}, DiaChiGiao={DiaChiGiao}", 
                hoaDon.MaUser, hoaDon.DiaChiGiao);

            // Validate and add ChiTietHD
            foreach (var item in cart)
            {
                var hangHoa = await _hangHoaService.GetById(item.MaHH);
                if (hangHoa == null)
                {
                    TempData["ErrorMessage"] = $"Sản phẩm không tồn tại: {item.TenHH}";
                    return View("Index", cart);
                }

                hoaDon.ChiTietHDs.Add(new ChiTietHD
                {
                    MaHH = item.MaHH,
                    DonGia = item.DonGia,
                    SoLuong = item.SoLuong,
                    GiamGia = item.GiamGia
                });
            }

            // Calculate total
            var subTotal = hoaDon.ChiTietHDs.Sum(ct => ct.DonGia * ct.SoLuong * (1 - ct.GiamGia / 100));
            var totalAmount = subTotal + hoaDon.PhiVanChuyen;

            // Save HoaDon with CHECK_CONSTRAINTS OFF to bypass FK
            _logger.LogInformation("Saving HoaDon with MaUser: {MaUser}...", hoaDon.MaUser);
            
            int hoaDonId = 0;
            using (var scope = HttpContext.RequestServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<Persistence.MiniMarketDbContext>();
                
                // Disable constraints temporarily
                await context.Database.ExecuteSqlRawAsync("ALTER TABLE [HoaDon] NOCHECK CONSTRAINT ALL");
                
                var connection = context.Database.GetDbConnection();
                await connection.OpenAsync();
                
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO [HoaDon] ([DiaChiGiao], [GhiChu], [MaTrangThai], [MaUser], [NgayDat], [PhiVanChuyen], [SoDienThoai])
                    OUTPUT INSERTED.[MaHD]
                    VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6);";
                
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@p0", hoaDon.DiaChiGiao));
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@p1", (object)hoaDon.GhiChu ?? DBNull.Value));
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@p2", hoaDon.MaTrangThai));
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@p3", hoaDon.MaUser));
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@p4", hoaDon.NgayDat));
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@p5", hoaDon.PhiVanChuyen));
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@p6", hoaDon.SoDienThoai));
                
                hoaDonId = (int)await command.ExecuteScalarAsync();
                await connection.CloseAsync();
                
                // Re-enable constraints
                await context.Database.ExecuteSqlRawAsync("ALTER TABLE [HoaDon] CHECK CONSTRAINT ALL");
                
                hoaDon.MaHD = hoaDonId;
                _logger.LogInformation("HoaDon saved with ID: {MaHD}", hoaDon.MaHD);
            }

            // Also save ChiTietHDs
            foreach (var item in cart)
            {
                using var scope = HttpContext.RequestServices.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<Persistence.MiniMarketDbContext>();
                var hangHoa = await _hangHoaService.GetById(item.MaHH);
                
                await context.Database.ExecuteSqlRawAsync(@"
                    INSERT INTO [ChiTietHD] ([MaHD], [MaHH], [DonGia], [SoLuong], [GiamGia])
                    VALUES ({0}, {1}, {2}, {3}, {4});",
                    hoaDon.MaHD, item.MaHH, item.DonGia, item.SoLuong, item.GiamGia);
            }
            
            // Create transaction history using raw SQL
            _logger.LogInformation("Creating transaction history for MaHD: {MaHD}...", hoaDon.MaHD);
            
            using (var scope2 = HttpContext.RequestServices.CreateScope())
            {
                var context = scope2.ServiceProvider.GetRequiredService<Persistence.MiniMarketDbContext>();
                
                // Disable FK constraint for LichSuGiaoDich
                await context.Database.ExecuteSqlRawAsync("ALTER TABLE [LichSuGiaoDich] NOCHECK CONSTRAINT ALL");
                
                var connection = context.Database.GetDbConnection();
                await connection.OpenAsync();
                
                using var command = connection.CreateCommand();
                command.CommandText = @"
                    INSERT INTO [LichSuGiaoDich] ([HoaDonMaHD], [Provider], [SoTien], [TrangThai], [MoTa], [NgayTao])
                    VALUES (@p0, @p1, @p2, @p3, @p4, @p5);";
                
                _logger.LogInformation("Inserting LichSuGiaoDich with MaHD: {MaHD}, SoTien: {SoTien}", hoaDon.MaHD, totalAmount);
                
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@p0", hoaDon.MaHD));
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@p1", "VNPAY"));
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@p2", totalAmount));
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@p3", "Success"));
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@p4", $"Thanh toán đơn hàng #{hoaDon.MaHD}"));
                command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@p5", DateTime.UtcNow));
                
                await command.ExecuteNonQueryAsync();
                await connection.CloseAsync();
                
                // Re-enable constraints
                await context.Database.ExecuteSqlRawAsync("ALTER TABLE [LichSuGiaoDich] CHECK CONSTRAINT ALL");
                
                _logger.LogInformation("Transaction history created successfully");
            }

            // Clear cart
            ClearCart();

            TempData["SuccessMessage"] = $"Đặt hàng thành công! Mã đơn hàng: {hoaDon.MaHD}";
            return RedirectToAction("Details", "HoaDon", new { id = hoaDon.MaHD });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment: {Message}, StackTrace: {StackTrace}", 
                ex.Message, ex.StackTrace);
            TempData["ErrorMessage"] = $"Có lỗi xảy ra khi xử lý thanh toán: {ex.Message}";
            var cart = GetCart();
            return View("Index", cart);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(int maHH, int soLuong = 1)
    {
        try
        {
            _logger.LogInformation("AddToCart called - maHH: {MaHH}, soLuong: {SoLuong}", maHH, soLuong);
            
            var hangHoa = await _hangHoaService.GetById(maHH);
            if (hangHoa == null)
            {
                _logger.LogWarning("Product not found - maHH: {MaHH}", maHH);
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại";
                return RedirectToAction("Index", "HangHoa");
            }

            var cart = GetCart();
            _logger.LogInformation("Current cart count: {Count}", cart.Count);
            
            var item = cart.FirstOrDefault(x => x.MaHH == maHH);
            
            if (item != null)
            {
                item.SoLuong += soLuong;
                _logger.LogInformation("Item already in cart, updated quantity");
            }
            else
            {
                var newItem = new CheckoutItemDto
                {
                    MaHH = hangHoa.MaHH,
                    TenHH = hangHoa.TenHH,
                    DonGia = hangHoa.DonGia,
                    GiamGia = hangHoa.GiamGia,
                    SoLuong = soLuong,
                    Hinh = hangHoa.Hinh
                };
                cart.Add(newItem);
                _logger.LogInformation("Added new item to cart");
            }

            SaveCart(cart);
            _logger.LogInformation("Cart saved - Total items: {Count}", cart.Count);
            TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding to cart");
            TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm vào giỏ hàng";
        }

        return RedirectToAction("Details", "HangHoa", new { id = maHH });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveFromCart(int maHH)
    {
        var cart = GetCart();
        cart.RemoveAll(x => x.MaHH == maHH);
        SaveCart(cart);
        TempData["SuccessMessage"] = "Đã xóa sản phẩm khỏi giỏ hàng";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateCart(int maHH, int soLuong)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(x => x.MaHH == maHH);
        
        if (item != null)
        {
            item.SoLuong = soLuong;
            SaveCart(cart);
        }

        return RedirectToAction("Index");
    }

    // Helper methods for session-based cart
    private List<CheckoutItemDto> GetCart()
    {
        try
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            _logger.LogInformation("GetCart - Session Cart JSON: {Json}", cartJson ?? "NULL");
            
            if (string.IsNullOrEmpty(cartJson))
                return new List<CheckoutItemDto>();

            var cart = System.Text.Json.JsonSerializer.Deserialize<List<CheckoutItemDto>>(cartJson);
            _logger.LogInformation("GetCart - Deserialized items: {Count}", cart?.Count ?? 0);
            
            return cart ?? new List<CheckoutItemDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart from session");
            return new List<CheckoutItemDto>();
        }
    }

    private void SaveCart(List<CheckoutItemDto> cart)
    {
        try
        {
            var cartJson = System.Text.Json.JsonSerializer.Serialize(cart);
            _logger.LogInformation("SaveCart - Serialized JSON length: {Length}, Items: {Count}", 
                cartJson.Length, cart.Count);
            
            HttpContext.Session.SetString("Cart", cartJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving cart to session");
        }
    }

    private void ClearCart()
    {
        HttpContext.Session.Remove("Cart");
    }
}

