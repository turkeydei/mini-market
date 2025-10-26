using Domain.Entities;

namespace Application.Features.Interface.IRepositories
{
    public interface IHoaDonRepository
    {
        Task<IEnumerable<HoaDon>> GetAllAsync();
        Task<HoaDon?> GetByIdAsync(int id);
        Task<HoaDon?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<HoaDon>> GetByUserIdAsync(int maUser);
        Task<IEnumerable<HoaDon>> GetByStatusAsync(int maTrangThai);
        Task AddAsync(HoaDon hoaDon);
        Task UpdateAsync(HoaDon hoaDon);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
