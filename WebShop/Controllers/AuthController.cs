using System.Security.Claims;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using WebShop.Models;

namespace WebShop.Controllers;

public class AuthController : Controller
{
    private readonly MiniMarketDbContext _context;
    private readonly ILogger<AuthController> _logger;

    public AuthController(MiniMarketDbContext context, ILogger<AuthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: /Auth/Register
    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    // POST: /Auth/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Kiểm tra email đã tồn tại
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (existingUser != null)
        {
            ModelState.AddModelError("Email", "Email này đã được đăng ký");
            return View(model);
        }

        // Tạo user mới
        var user = new User
        {
            HoTen = model.HoTen,
            Email = model.Email,
            MatKhau = model.MatKhau, // Trong thực tế nên hash password
            DienThoai = model.DienThoai,
            DiaChi = model.DiaChi,
            VaiTro = "Customer",
            GioiTinh = model.GioiTinh,
            NgaySinh = model.NgaySinh
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
        return RedirectToAction(nameof(Login));
    }

    // GET: /Auth/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // POST: /Auth/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == model.Email && u.MatKhau == model.MatKhau);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng");
            return View(model);
        }

        // Tạo claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.MaUser.ToString()),
            new Claim(ClaimTypes.Name, user.HoTen ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Role, user.VaiTro)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(2)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        _logger.LogInformation("User {Email} logged in", user.Email);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    // POST: /Auth/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("User logged out");
        return RedirectToAction("Index", "Home");
    }

    // GET: /Auth/Profile
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // GET: /Auth/AccessDenied
    public IActionResult AccessDenied()
    {
        return View();
    }
    // GET: /Auth/ChangePassword
    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    // POST: /Auth/ChangePassword
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return NotFound();

        // Kiểm tra mật khẩu cũ có đúng không
        if (user.MatKhau != model.MatKhauCu)
        {
            ModelState.AddModelError("MatKhauCu", "Mật khẩu hiện tại không đúng");
            return View(model);
        }

        // Cập nhật mật khẩu mới
        user.MatKhau = model.MatKhauMoi;
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
        return RedirectToAction("ChangePassword");
    }
    [Authorize]
    [HttpGet]
    public IActionResult ChangeInfo()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = _context.Users.Find(int.Parse(userId!));
        if (user == null) return NotFound();

        var model = new ProfileUpdateViewModel
        {
            HoTen = user.HoTen,
            DiaChi = user.DiaChi,
            Email = user.Email,
            DienThoai = user.DienThoai,
            GioiTinh = user.GioiTinh,
            NgaySinh = user.NgaySinh
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ChangeInfo(ProfileUpdateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = _context.Users.Find(int.Parse(userId!));
        if (user == null) return NotFound();

        user.HoTen = model.HoTen;
        user.DiaChi = model.DiaChi;
        user.GioiTinh = model.GioiTinh;
        user.NgaySinh = model.NgaySinh;
        user.DienThoai = model.DienThoai;
        user.Email = model.Email;
    
        _context.Users.Update(user);
        _context.SaveChanges();

        TempData["Success"] = "Cập nhật thông tin thành công!";

        return RedirectToAction(nameof(ChangeInfo));
    }


}

