using Domain.Entities;

namespace Application.Features.Interface
{
    public interface IHangHoaService
    {
        Task<IEnumerable<HangHoa>> GetAll();
        Task<HangHoa?> GetById(int id);
        Task Add(HangHoa hh);
        Task Update(HangHoa hh);
        Task Delete(int id);
    }
}