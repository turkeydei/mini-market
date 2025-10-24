using Domain.Entities;

namespace Application.Features.Interface
{
    public interface IHoaDonService
    {
        Task<IEnumerable<HoaDon>> GetAll();
        Task<HoaDon?> GetById(int id);
        Task Add(HoaDon hd);
        Task Update(HoaDon hd);
        Task Delete(int id);
    }
}