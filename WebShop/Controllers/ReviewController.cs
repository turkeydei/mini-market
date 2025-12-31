using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Domain.Entities;
using System.Security.Claims;

namespace WebShop.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Review/Create/{productId}
        [Authorize]
        [HttpGet("Review/Create/{productId}")]
        public async Task<IActionResult> Create(int productId)
        {
            var product = await _unitOfWork.HangHoas.GetByIdAsync(productId);
            if (product == null)
                return NotFound();

            // Kiểm tra user đã mua sản phẩm này chưa
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var hasPurchased = await HasUserPurchasedProduct(userId, productId);

            if (!hasPurchased)
            {
                TempData["Error"] = "Bạn chỉ có thể đánh giá sản phẩm đã mua!";
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            // Kiểm tra đã review chưa
            var existingReview = (await _unitOfWork.ProductReviews.FindAsync(r => 
                r.MaHH == productId && r.MaUser == userId)).FirstOrDefault();

            if (existingReview != null)
            {
                TempData["Info"] = "Bạn đã đánh giá sản phẩm này rồi!";
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            ViewBag.ProductId = productId;
            ViewBag.ProductName = product.TenHH;
            return View();
        }

        // POST: Review/Create
        [Authorize]
        [HttpPost("Review/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductReview review)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                
                // Kiểm tra user đã mua sản phẩm này chưa
                var hasPurchased = await HasUserPurchasedProduct(userId, review.MaHH);
                if (!hasPurchased)
                {
                    ModelState.AddModelError("", "Bạn chỉ có thể đánh giá sản phẩm đã mua!");
                    ViewBag.ProductId = review.MaHH;
                    return View(review);
                }

                // Kiểm tra đã review chưa
                var existingReview = (await _unitOfWork.ProductReviews.FindAsync(r => 
                    r.MaHH == review.MaHH && r.MaUser == userId)).FirstOrDefault();

                if (existingReview != null)
                {
                    TempData["Error"] = "Bạn đã đánh giá sản phẩm này rồi!";
                    return RedirectToAction("Details", "Product", new { id = review.MaHH });
                }

                review.MaUser = userId;
                review.NgayTao = DateTime.Now;
                review.IsApproved = false; // Cần admin phê duyệt

                await _unitOfWork.ProductReviews.AddAsync(review);
                await _unitOfWork.SaveAsync();

                TempData["Success"] = "Cảm ơn bạn đã đánh giá! Review của bạn đang chờ phê duyệt.";
                return RedirectToAction("Details", "Product", new { id = review.MaHH });
            }

            ViewBag.ProductId = review.MaHH;
            return View(review);
        }

        // Helper method: Kiểm tra user đã mua sản phẩm chưa
        private async Task<bool> HasUserPurchasedProduct(int userId, int productId)
        {
            var orders = await _unitOfWork.HoaDons.FindAsync(o => 
                o.MaUser == userId && o.Status == "Completed");

            foreach (var order in orders)
            {
                var orderDetails = await _unitOfWork.ChiTietHDs.FindAsync(ct => 
                    ct.MaHD == order.MaHD && ct.MaHH == productId);
                
                if (orderDetails.Any())
                    return true;
            }

            return false;
        }
    }
}

