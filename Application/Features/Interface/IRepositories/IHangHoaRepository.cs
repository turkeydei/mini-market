using Domain.Entities;

namespace Application.Features.Interface.IRepositories
{
    public interface IHangHoaRepository
    {
        Task<IEnumerable<HangHoa>> GetAllAsync();
        Task<HangHoa?> GetByIdAsync(int id);
        Task<HangHoa?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<HangHoa>> GetByCategoryAsync(int maLoai);
        Task<IEnumerable<HangHoa>> SearchAsync(string keyword);
        Task AddAsync(HangHoa hangHoa);
        Task UpdateAsync(HangHoa hangHoa);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
