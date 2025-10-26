using Application.Features.DTOs;
using Application.Features.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace WebShop.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IAuthService _authenticationService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IAuthService authenticationService, ILogger<AuthenticationController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (_authenticationService.IsAuthenticated())
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _authenticationService.LoginAsync(model);
                if (result)
                {
                    _logger.LogInformation("User {Email} logged in successfully", model.Email);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Email}", model.Email);
                ModelState.AddModelError("", "Có lỗi xảy ra khi đăng nhập");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (_authenticationService.IsAuthenticated())
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _authenticationService.RegisterAsync(model);
                if (result)
                {
                    _logger.LogInformation("User {Email} registered successfully", model.Email);
                    TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", "Có lỗi xảy ra khi đăng ký. Email có thể đã tồn tại.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user {Email}", model.Email);
                ModelState.AddModelError("", "Có lỗi xảy ra khi đăng ký");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authenticationService.LogoutAsync();
            _logger.LogInformation("User logged out");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
