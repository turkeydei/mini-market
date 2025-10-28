using Domain.Entities;

namespace Application.Services;

public interface IProductService
{
    Task<IEnumerable<HangHoa>> GetAllProductsAsync();
    Task<IEnumerable<HangHoa>> GetProductsByCategoryAsync(int categoryId);
    Task<HangHoa?> GetProductByIdAsync(int id);
    Task<HangHoa> CreateProductAsync(HangHoa product);
    Task UpdateProductAsync(HangHoa product);
    Task DeleteProductAsync(int id);
    Task<bool> IsProductInStockAsync(int id, int quantity);
}

