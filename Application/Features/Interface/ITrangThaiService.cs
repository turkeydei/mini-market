using Domain.Entities;

namespace Application.Features.Interface
{
    public interface ITrangThaiService
    {
        Task<IEnumerable<TrangThai>> GetAll();
        Task<TrangThai?> GetById(int id);
        Task Add(TrangThai tt);
        Task Update(TrangThai tt);
        Task Delete(int id);
    }
}