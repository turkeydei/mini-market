using Application.Features.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebShop.Controllers
{
    [Authorize]
    public class HoaDonController : Controller
    {
        private readonly IHoaDonService _hoaDonService;
        private readonly IAuthService _authenticationService;
        private readonly ILogger<HoaDonController> _logger;

        public HoaDonController(
            IHoaDonService hoaDonService, 
            IAuthService authenticationService,
            ILogger<HoaDonController> logger)
        {
            _hoaDonService = hoaDonService;
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var currentUser = await _authenticationService.GetCurrentUserAsync();
                if (currentUser == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng";
                    return RedirectToAction("Login", "Authentication");
                }

                var hoaDons = await _hoaDonService.GetAll(); // Temporarily get all - will filter by user later
                return View(hoaDons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading hoa don list for user");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách hóa đơn";
                return View(new List<Domain.Entities.HoaDon>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var hoaDon = await _hoaDonService.GetById(id);
                if (hoaDon == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy hóa đơn";
                    return RedirectToAction("Index");
                }

                // Check if user owns this order or is admin
                var currentUser = await _authenticationService.GetCurrentUserAsync();
                if (currentUser == null || (hoaDon.MaUser != currentUser.MaUser && !_authenticationService.IsInRole("Admin")))
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền xem hóa đơn này";
                    return RedirectToAction("Index");
                }

                return View(hoaDon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading hoa don details for ID {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông tin hóa đơn";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminIndex(int? statusId)
        {
            try
            {
                IEnumerable<Domain.Entities.HoaDon> hoaDons;

                hoaDons = await _hoaDonService.GetAll(); // Temporarily get all

                return View(hoaDons);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin hoa don list");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách hóa đơn";
                return View(new List<Domain.Entities.HoaDon>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, int statusId)
        {
            try
            {
                // var result = await _hoaDonService.UpdateStatus(id, statusId);
                var result = true; // Temporarily return true
                if (result)
                {
                    TempData["SuccessMessage"] = "Cập nhật trạng thái hóa đơn thành công";
                }
                else
                {
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật trạng thái hóa đơn";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating hoa don status, ID {Id}, Status {StatusId}", id, statusId);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi cập nhật trạng thái hóa đơn";
            }

            return RedirectToAction("AdminIndex");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _hoaDonService.Delete(id);
                TempData["SuccessMessage"] = "Xóa hóa đơn thành công";
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting hoa don, ID {Id}", id);
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa hóa đơn";
            }

            return RedirectToAction("AdminIndex");
        }
    }
}
