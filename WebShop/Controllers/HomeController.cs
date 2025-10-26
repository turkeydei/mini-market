using System.Diagnostics;
using Application.Features.Interface;
using Microsoft.AspNetCore.Mvc;
using WebShop.Models;

namespace WebShop.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHangHoaService _hangHoaService;
        private readonly IAuthService _authenticationService;

    public HomeController(
        ILogger<HomeController> logger, 
        IHangHoaService hangHoaService,
        IAuthService authenticationService)
    {
        _logger = logger;
        _hangHoaService = hangHoaService;
        _authenticationService = authenticationService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            // Get featured products (first 8 products)
            var featuredProducts = (await _hangHoaService.GetAll()).Take(8);
            return View(featuredProducts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading featured products");
            return View(new List<Domain.Entities.HangHoa>());
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult About()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Contact()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
