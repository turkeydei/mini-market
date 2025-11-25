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

    public async Task<IActionResult> Index(int? categoryId, string? keyword, string? sort)
    {
        // Lấy tất cả categories cho dropdown
        ViewBag.Categories = await _context.Loais.ToListAsync();
        ViewBag.SelectedCategory = categoryId;
        ViewBag.SearchKeyword = keyword?.Trim();
        ViewBag.SelectedSort = sort;
        
        // Lấy sản phẩm - filter theo category nếu có
        var products = _context.HangHoas
            .Include(h => h.Loai)
            .AsQueryable();
        
        if (categoryId.HasValue)
        {
            products = products.Where(h => h.MaLoai == categoryId.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var term = keyword.Trim();
            var likeTerm = $"%{term}%";
            
            products = products.Where(h =>
                EF.Functions.Like(h.TenHH, likeTerm) ||
                (h.MoTa != null && EF.Functions.Like(h.MoTa, likeTerm)) ||
                (h.Loai != null && EF.Functions.Like(h.Loai.TenLoai, likeTerm)));
        }

        products = sort switch
        {
            "price_asc" => products.OrderBy(h => h.DonGia - h.GiamGia),
            "price_desc" => products.OrderByDescending(h => h.DonGia - h.GiamGia),
            "new" => products.OrderByDescending(h => h.MaHH),
            "popular" => products.OrderByDescending(h => h.SoLanXem),
            _ => products.OrderByDescending(h => h.SoLanXem)
        };
        
        var productList = await products.Take(20).ToListAsync();
        
        ViewBag.SearchResultCount = productList.Count;
        
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
