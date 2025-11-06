using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<HangHoa>> GetAllProductsAsync()
    {
        return await _unitOfWork.GetAllProductsWithCategoryAsync();
    }

    public async Task<IEnumerable<HangHoa>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _unitOfWork.GetProductsByCategoryWithDetailsAsync(categoryId);
    }

    public async Task<HangHoa?> GetProductByIdAsync(int id)
    {
        return await _unitOfWork.GetProductByIdWithCategoryAsync(id);
    }

    public async Task<HangHoa> CreateProductAsync(HangHoa product)
    {
        await _unitOfWork.HangHoas.AddAsync(product);
        await _unitOfWork.SaveAsync();
        return product;
    }

    public async Task UpdateProductAsync(HangHoa product)
    {
        _unitOfWork.HangHoas.Update(product);
        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _unitOfWork.HangHoas.GetByIdAsync(id);
        if (product != null)
        {
            _unitOfWork.HangHoas.Remove(product);
            await _unitOfWork.SaveAsync();
        }
    }

    public async Task<bool> IsProductInStockAsync(int id, int quantity)
    {
        var product = await _unitOfWork.HangHoas.GetByIdAsync(id);
        return product != null && product.SoLuongTon >= quantity;
    }
}

