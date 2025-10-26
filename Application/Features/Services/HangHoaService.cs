using Application.Features.Interface;
using Application.Features.Interface.IRepositories;
using Domain.Entities;

namespace Application.Features.Services
{
    public class HangHoaService : IHangHoaService
    {
        private readonly IHangHoaRepository _hangHoaRepository;

        public HangHoaService(IHangHoaRepository hangHoaRepository)
        {
            _hangHoaRepository = hangHoaRepository;
        }

        public async Task<IEnumerable<HangHoa>> GetAll()
        {
            return await _hangHoaRepository.GetAllAsync();
        }

        public async Task<HangHoa?> GetById(int id)
        {
            if (id <= 0)
                return null;

            return await _hangHoaRepository.GetByIdAsync(id);
        }

        public async Task Add(HangHoa hangHoa)
        {
            if (hangHoa == null)
                throw new ArgumentNullException(nameof(hangHoa));

            // Business logic validation
            if (string.IsNullOrWhiteSpace(hangHoa.TenHH))
                throw new ArgumentException("Tên hàng hóa không được để trống");

            if (hangHoa.DonGia <= 0)
                throw new ArgumentException("Đơn giá phải lớn hơn 0");

            if (hangHoa.MaLoai <= 0)
                throw new ArgumentException("Mã loại không hợp lệ");

            await _hangHoaRepository.AddAsync(hangHoa);
        }

        public async Task Update(HangHoa hangHoa)
        {
            if (hangHoa == null)
                throw new ArgumentNullException(nameof(hangHoa));

            if (!await _hangHoaRepository.ExistsAsync(hangHoa.MaHH))
                throw new ArgumentException("Hàng hóa không tồn tại");

            // Business logic validation
            if (string.IsNullOrWhiteSpace(hangHoa.TenHH))
                throw new ArgumentException("Tên hàng hóa không được để trống");

            if (hangHoa.DonGia <= 0)
                throw new ArgumentException("Đơn giá phải lớn hơn 0");

            await _hangHoaRepository.UpdateAsync(hangHoa);
        }

        public async Task Delete(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID không hợp lệ");

            if (!await _hangHoaRepository.ExistsAsync(id))
                throw new ArgumentException("Hàng hóa không tồn tại");

            await _hangHoaRepository.DeleteAsync(id);
        }

        // Additional business methods
        public async Task<IEnumerable<HangHoa>> GetByCategory(int maLoai)
        {
            if (maLoai <= 0)
                return Enumerable.Empty<HangHoa>();

            return await _hangHoaRepository.GetByCategoryAsync(maLoai);
        }

        public async Task<IEnumerable<HangHoa>> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await _hangHoaRepository.GetAllAsync();

            return await _hangHoaRepository.SearchAsync(keyword);
        }

        public async Task<HangHoa?> GetWithDetails(int id)
        {
            if (id <= 0)
                return null;

            return await _hangHoaRepository.GetByIdWithDetailsAsync(id);
        }
    }
}
