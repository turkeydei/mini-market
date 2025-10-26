using Application.Features.Interface;
using Application.Features.Interface.IRepositories;
using Domain.Entities;

namespace Application.Features.Services
{
    public class HoaDonService : IHoaDonService
    {
        private readonly IHoaDonRepository _hoaDonRepository;
        private readonly IUserRepository _userRepository;

        public HoaDonService(IHoaDonRepository hoaDonRepository, IUserRepository userRepository)
        {
            _hoaDonRepository = hoaDonRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<HoaDon>> GetAll()
        {
            return await _hoaDonRepository.GetAllAsync();
        }

        public async Task<HoaDon?> GetById(int id)
        {
            if (id <= 0)
                return null;

            return await _hoaDonRepository.GetByIdAsync(id);
        }

        public async Task Add(HoaDon hoaDon)
        {
            if (hoaDon == null)
                throw new ArgumentNullException(nameof(hoaDon));

            // Business logic validation
            if (hoaDon.MaUser <= 0)
                throw new ArgumentException("Mã người dùng không hợp lệ");

            if (!await _userRepository.ExistsAsync(hoaDon.MaUser))
                throw new ArgumentException("Người dùng không tồn tại");

            if (string.IsNullOrWhiteSpace(hoaDon.DiaChiGiao))
                throw new ArgumentException("Địa chỉ giao hàng không được để trống");

            if (hoaDon.PhiVanChuyen < 0)
                throw new ArgumentException("Phí vận chuyển không được âm");

            if (hoaDon.MaTrangThai <= 0)
                throw new ArgumentException("Mã trạng thái không hợp lệ");

            // Set default values
            if (hoaDon.NgayDat == default)
                hoaDon.NgayDat = DateTime.UtcNow;

            await _hoaDonRepository.AddAsync(hoaDon);
        }

        public async Task Update(HoaDon hoaDon)
        {
            if (hoaDon == null)
                throw new ArgumentNullException(nameof(hoaDon));

            if (!await _hoaDonRepository.ExistsAsync(hoaDon.MaHD))
                throw new ArgumentException("Hóa đơn không tồn tại");

            // Business logic validation
            if (hoaDon.MaUser <= 0)
                throw new ArgumentException("Mã người dùng không hợp lệ");

            if (!await _userRepository.ExistsAsync(hoaDon.MaUser))
                throw new ArgumentException("Người dùng không tồn tại");

            if (string.IsNullOrWhiteSpace(hoaDon.DiaChiGiao))
                throw new ArgumentException("Địa chỉ giao hàng không được để trống");

            if (hoaDon.PhiVanChuyen < 0)
                throw new ArgumentException("Phí vận chuyển không được âm");

            await _hoaDonRepository.UpdateAsync(hoaDon);
        }

        public async Task Delete(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID không hợp lệ");

            if (!await _hoaDonRepository.ExistsAsync(id))
                throw new ArgumentException("Hóa đơn không tồn tại");

            await _hoaDonRepository.DeleteAsync(id);
        }

        // Additional business methods
        public async Task<IEnumerable<HoaDon>> GetByUserId(int maUser)
        {
            if (maUser <= 0)
                return Enumerable.Empty<HoaDon>();

            return await _hoaDonRepository.GetByUserIdAsync(maUser);
        }

        public async Task<IEnumerable<HoaDon>> GetByStatus(int maTrangThai)
        {
            if (maTrangThai <= 0)
                return Enumerable.Empty<HoaDon>();

            return await _hoaDonRepository.GetByStatusAsync(maTrangThai);
        }

        public async Task<HoaDon?> GetWithDetails(int id)
        {
            if (id <= 0)
                return null;

            return await _hoaDonRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<bool> UpdateStatus(int maHD, int maTrangThai)
        {
            if (maHD <= 0 || maTrangThai <= 0)
                return false;

            var hoaDon = await _hoaDonRepository.GetByIdAsync(maHD);
            if (hoaDon == null)
                return false;

            hoaDon.MaTrangThai = maTrangThai;

            // Update delivery date if status is "Delivered"
            if (maTrangThai == 3) // Assuming 3 is "Delivered" status
            {
                hoaDon.NgayGiao = DateTime.UtcNow;
            }

            await _hoaDonRepository.UpdateAsync(hoaDon);
            return true;
        }
    }
}
