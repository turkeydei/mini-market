using Domain.Entities;

namespace Application.Features.Interface;

public interface ITransactionHistoryService
{
    Task<IEnumerable<LichSuGiaoDich>> GetAllAsync();
    Task<LichSuGiaoDich?> GetByIdAsync(int id);
    Task<IEnumerable<LichSuGiaoDich>> GetByUserIdAsync(int maUser);
    Task<IEnumerable<LichSuGiaoDich>> GetByHoaDonIdAsync(int maHD);
    Task AddAsync(LichSuGiaoDich transaction);
    Task UpdateAsync(LichSuGiaoDich transaction);
}

