using Application.Features.DTOs;
using Domain.Entities;

namespace Application.Features.Interface
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginDto loginDto);
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task LogoutAsync();
        Task<User?> GetCurrentUserAsync();
        bool IsAuthenticated();
        bool IsInRole(string role);
    }
}
