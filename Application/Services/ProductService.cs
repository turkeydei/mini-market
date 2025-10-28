using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly MiniMarketDbContext _context;

    public ProductService(IUnitOfWork unitOfWork, MiniMarketDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<IEnumerable<HangHoa>> GetAllProductsAsync()
    {
        return await _context.HangHoas
            .Include(h => h.Loai)
            .OrderByDescending(h => h.SoLanXem)
            .Take(20)
            .ToListAsync();
    }

    public async Task<IEnumerable<HangHoa>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _context.HangHoas
            .Include(h => h.Loai)
            .Where(h => h.MaLoai == categoryId)
            .OrderByDescending(h => h.SoLanXem)
            .ToListAsync();
    }

    public async Task<HangHoa?> GetProductByIdAsync(int id)
    {
        return await _context.HangHoas
            .Include(h => h.Loai)
            .FirstOrDefaultAsync(h => h.MaHH == id);
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

