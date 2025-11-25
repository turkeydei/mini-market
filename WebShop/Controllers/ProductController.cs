using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using WebShop.Models;

namespace WebShop.Controllers;

public class ProductController : Controller
{
    private readonly MiniMarketDbContext _context;

    public ProductController(MiniMarketDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.HangHoas
            .Include(p => p.Loai)
            .FirstOrDefaultAsync(p => p.MaHH == id);

        if (product == null)
        {
            return NotFound();
        }

        var related = await _context.HangHoas
            .Where(p => p.MaLoai == product.MaLoai && p.MaHH != product.MaHH)
            .OrderByDescending(p => p.SoLanXem)
            .Take(4)
            .ToListAsync();

        var viewModel = new ProductDetailViewModel
        {
            Product = product,
            RelatedProducts = related
        };

        return View(viewModel);
    }
}

