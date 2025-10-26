using Application.Features.DTOs;
using Application.Features.Interface;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Features.Services
{
    public class AuthenticationService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> LoginAsync(LoginDto loginDto)
        {
            if (loginDto == null)
                return false;

            var user = await _userService.Login(loginDto.Email, loginDto.Password);
            if (user == null)
                return false;

            await SignInAsync(user, loginDto.RememberMe);
            return true;
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            if (registerDto == null)
                return false;

            try
            {
                var user = new User
                {
                    HoTen = registerDto.HoTen,
                    Email = registerDto.Email,
                    MatKhau = registerDto.Password, // Will be hashed in UserService
                    DienThoai = registerDto.DienThoai,
                    DiaChi = registerDto.DiaChi,
                    NgaySinh = registerDto.NgaySinh,
                    GioiTinh = registerDto.GioiTinh,
                    VaiTro = 0 // Default role: Customer
                };

                await _userService.Add(user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            await _httpContextAccessor.HttpContext?.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
                return null;

            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return null;

            return await _userService.GetById(userId);
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
        }

        public bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole(role) == true;
        }

        private async Task SignInAsync(User user, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.MaUser.ToString()),
                new Claim(ClaimTypes.Name, user.HoTen),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.VaiTro == 1 ? "Admin" : "Customer")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(24)
            };

            await _httpContextAccessor.HttpContext?.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}
