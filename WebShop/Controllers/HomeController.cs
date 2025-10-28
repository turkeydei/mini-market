using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using WebShop.Models;

namespace WebShop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MiniMarketDbContext _context;

    public HomeController(ILogger<HomeController> logger, MiniMarketDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index(int? categoryId)
    {
        // Lấy tất cả categories cho dropdown
        ViewBag.Categories = await _context.Loais.ToListAsync();
        
        // Lấy sản phẩm - filter theo category nếu có
        var products = _context.HangHoas
            .Include(h => h.Loai)
            .AsQueryable();
        
        if (categoryId.HasValue)
        {
            products = products.Where(h => h.MaLoai == categoryId.Value);
        }
        
        var productList = await products
            .OrderByDescending(h => h.SoLanXem)
            .Take(20)
            .ToListAsync();
        
        return View(productList);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
