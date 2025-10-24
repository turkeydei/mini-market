using Domain.Entities;

namespace Application.Features.Interface
{
    public interface ILichSuGiaoDich
    {
        Task<IEnumerable<LichSuGiaoDich>> GetAll();
        Task<LichSuGiaoDich?> GetById(int id);
        Task Add(LichSuGiaoDich transaction);
        Task Update(LichSuGiaoDich transaction);
        Task Delete(int id);
    }
}