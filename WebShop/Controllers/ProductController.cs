using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using WebShop.Models;
using Application.Interfaces;
using System.Security.Claims;

namespace WebShop.Controllers;

public class ProductController : Controller
{
    private readonly MiniMarketDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public ProductController(MiniMarketDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
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

        // Load reviews đã được phê duyệt
        var reviews = (await _unitOfWork.ProductReviews.FindAsync(r => 
            r.MaHH == id && r.IsApproved == true))
            .OrderByDescending(r => r.NgayTao)
            .ToList();

        // Load User info cho reviews
        foreach (var review in reviews)
        {
            review.User = await _unitOfWork.Users.GetByIdAsync(review.MaUser);
        }

        // Tính average rating
        var averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;
        var totalReviews = reviews.Count;

        // Kiểm tra user có thể review không
        bool canReview = false;
        bool hasReviewed = false;
        
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // Kiểm tra đã review chưa
            var userReview = reviews.FirstOrDefault(r => r.MaUser == userId);
            hasReviewed = userReview != null;

            if (!hasReviewed)
            {
                // Kiểm tra đã mua chưa
                var orders = await _unitOfWork.HoaDons.FindAsync(o => 
                    o.MaUser == userId && o.Status == "Completed");

                foreach (var order in orders)
                {
                    var orderDetails = await _unitOfWork.ChiTietHDs.FindAsync(ct => 
                        ct.MaHD == order.MaHD && ct.MaHH == id);
                    
                    if (orderDetails.Any())
                    {
                        canReview = true;
                        break;
                    }
                }
            }
        }

        var viewModel = new ProductDetailViewModel
        {
            Product = product,
            RelatedProducts = related,
            Reviews = reviews,
            AverageRating = Math.Round(averageRating, 1),
            TotalReviews = totalReviews,
            CanReview = canReview,
            HasReviewed = hasReviewed
        };

        return View(viewModel);
    }
}

