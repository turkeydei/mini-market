using Domain.Entities;

namespace Application.Features.Interface
{
    public interface ILoaiService
    {
        Task<IEnumerable<Loai>> GetAll();
        Task<Loai?> GetById(int id);
        Task Add(Loai loai);
        Task Update(Loai loai);
        Task Delete(int id);
    }
}