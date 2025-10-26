using Application.Features.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebShop.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly IHangHoaService _hangHoaService;
        private readonly ILogger<HangHoaController> _logger;

        public HangHoaController(IHangHoaService hangHoaService, ILogger<HangHoaController> logger)
        {
            _hangHoaService = hangHoaService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, int? categoryId)
        {
            try
            {
                IEnumerable<Domain.Entities.HangHoa> hangHoas;

                // Temporarily simplified - will add search and category filter later
                hangHoas = await _hangHoaService.GetAll();

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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Domain.Entities.HangHoa hangHoa)
        {
            if (!ModelState.IsValid)
                return View(hangHoa);

            try
            {
                await _hangHoaService.Add(hangHoa);
                TempData["SuccessMessage"] = "Thêm hàng hóa thành công";
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating hang hoa");
                ModelState.AddModelError("", "Có lỗi xảy ra khi thêm hàng hóa");
            }

            return View(hangHoa);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
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
                _logger.LogError(ex, "Error loading hang hoa for edit, ID {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin hàng hóa";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Domain.Entities.HangHoa hangHoa)
        {
            if (!ModelState.IsValid)
                return View(hangHoa);

            try
            {
                await _hangHoaService.Update(hangHoa);
                TempData["SuccessMessage"] = "Cập nhật hàng hóa thành công";
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating hang hoa, ID {Id}", hangHoa.MaHH);
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật hàng hóa");
            }

            return View(hangHoa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _hangHoaService.Delete(id);
                TempData["SuccessMessage"] = "Xóa hàng hóa thành công";
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting hang hoa, ID {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa hàng hóa";
            }

            return RedirectToAction("Index");
        }
    }
}
