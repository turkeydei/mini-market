using Application.Features.Interface;
using Microsoft.AspNetCore.Mvc;

namespace WebShop.Controllers
{
    public class SimpleHangHoaController : Controller
    {
        private readonly IHangHoaService _hangHoaService;
        private readonly ILogger<SimpleHangHoaController> _logger;

        public SimpleHangHoaController(IHangHoaService hangHoaService, ILogger<SimpleHangHoaController> logger)
        {
            _hangHoaService = hangHoaService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var hangHoas = await _hangHoaService.GetAll();
                return View(hangHoas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading hang hoa list");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách hàng hóa";
                return View(new List<Domain.Entities.HangHoa>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var hangHoa = await _hangHoaService.GetById(id);
                if (hangHoa == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy hàng hóa";
                    return RedirectToAction("Index");
                }

                return View(hangHoa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading hang hoa details for ID {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin hàng hóa";
                return RedirectToAction("Index");
            }
        }
    }
}
