using Application.Features.Interface;
using Application.Features.Interface.IRepositories;
using Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace Application.Features.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetById(int id)
        {
            if (id <= 0)
                return null;

            return await _userRepository.GetByIdAsync(id);
        }

        public async Task Add(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Business logic validation
            if (string.IsNullOrWhiteSpace(user.HoTen))
                throw new ArgumentException("Họ tên không được để trống");

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email không được để trống");

            if (!IsValidEmail(user.Email))
                throw new ArgumentException("Email không hợp lệ");

            if (await _userRepository.EmailExistsAsync(user.Email))
                throw new ArgumentException("Email đã tồn tại");

            if (string.IsNullOrWhiteSpace(user.MatKhau))
                throw new ArgumentException("Mật khẩu không được để trống");

            if (user.MatKhau.Length < 6)
                throw new ArgumentException("Mật khẩu phải có ít nhất 6 ký tự");

            // Hash password
            user.MatKhau = HashPassword(user.MatKhau);

            await _userRepository.AddAsync(user);
        }

        public async Task Update(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (!await _userRepository.ExistsAsync(user.MaUser))
                throw new ArgumentException("Người dùng không tồn tại");

            // Business logic validation
            if (string.IsNullOrWhiteSpace(user.HoTen))
                throw new ArgumentException("Họ tên không được để trống");

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email không được để trống");

            if (!IsValidEmail(user.Email))
                throw new ArgumentException("Email không hợp lệ");

            // Check if email exists for other users
            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null && existingUser.MaUser != user.MaUser)
                throw new ArgumentException("Email đã tồn tại");

            await _userRepository.UpdateAsync(user);
        }

        public async Task Delete(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID không hợp lệ");

            if (!await _userRepository.ExistsAsync(id))
                throw new ArgumentException("Người dùng không tồn tại");

            await _userRepository.DeleteAsync(id);
        }

        // Additional business methods
        public async Task<User?> GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<User?> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            var hashedPassword = HashPassword(password);
            return await _userRepository.GetByEmailAndPasswordAsync(email, hashedPassword);
        }

        public async Task<bool> ChangePassword(int userId, string oldPassword, string newPassword)
        {
            if (userId <= 0)
                return false;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            var hashedOldPassword = HashPassword(oldPassword);
            if (user.MatKhau != hashedOldPassword)
                return false;

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                return false;

            user.MatKhau = HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
