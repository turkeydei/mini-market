using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Domain.Entities;
using System.Linq;

namespace WebShop.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;

        public AdminController(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
        }

        // ==================== DASHBOARD ====================
        
        [HttpGet("")]
        [HttpGet("Dashboard")]
        public async Task<IActionResult> Index()
        {
            var stats = new
            {
                TotalUsers = (await _unitOfWork.Users.GetAllAsync()).Count(),
                TotalProducts = (await _unitOfWork.HangHoas.GetAllAsync()).Count(),
                TotalOrders = (await _unitOfWork.HoaDons.GetAllAsync()).Count(),
                TotalRevenue = (await _unitOfWork.HoaDons.FindAsync(h => h.Status == "Completed"))
                    .Sum(h => h.TongTien),
                PendingOrders = (await _unitOfWork.HoaDons.FindAsync(h => h.Status == "Pending")).Count(),
                RecentOrders = (await _unitOfWork.HoaDons.GetAllAsync())
                    .OrderByDescending(h => h.NgayDat)
                    .Take(10)
                    .ToList(),
                LowStockProducts = (await _unitOfWork.HangHoas.FindAsync(h => h.SoLuongTon < 10))
                    .OrderBy(h => h.SoLuongTon)
                    .Take(10)
                    .ToList()
            };

            return View(stats);
        }

        // ==================== QUẢN LÝ USERS ====================
        
        [HttpGet("Users")]
        public async Task<IActionResult> Users()
        {
            var users = (await _unitOfWork.Users.GetAllAsync())
                .OrderByDescending(u => u.MaUser)
                .ToList();
            return View(users);
        }

        [HttpPost("Users/ToggleRole/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserRole(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            user.VaiTro = user.VaiTro == "Admin" ? "Customer" : "Admin";
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveAsync();

            TempData["Success"] = $"Đã cập nhật role của {user.HoTen}";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost("Users/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            // Không cho xóa admin cuối cùng
            var adminCount = (await _unitOfWork.Users.GetAllAsync())
                .Count(u => u.VaiTro == "Admin");
            
            if (user.VaiTro == "Admin" && adminCount <= 1)
            {
                TempData["Error"] = "Không thể xóa admin cuối cùng!";
                return RedirectToAction(nameof(Users));
            }

            _unitOfWork.Users.Remove(user);
            await _unitOfWork.SaveAsync();

            TempData["Success"] = $"Đã xóa user {user.HoTen}";
            return RedirectToAction(nameof(Users));
        }

        // ==================== QUẢN LÝ CATEGORIES ====================
        
        [HttpGet("Categories")]
        public async Task<IActionResult> Categories()
        {
            var categories = (await _unitOfWork.Loais.GetAllAsync())
                .OrderBy(l => l.TenLoai)
                .ToList();
            return View(categories);
        }

        [HttpGet("Categories/Create")]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost("Categories/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Loai category)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Loais.AddAsync(category);
                await _unitOfWork.SaveAsync();
                TempData["Success"] = "Đã tạo danh mục mới!";
                return RedirectToAction(nameof(Categories));
            }
            return View(category);
        }

        [HttpGet("Categories/Edit/{id}")]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _unitOfWork.Loais.GetByIdAsync(id);
            if (category == null)
                return NotFound();
            return View(category);
        }

        [HttpPost("Categories/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, Loai category)
        {
            if (id != category.MaLoai)
                return NotFound();

            if (ModelState.IsValid)
            {
                _unitOfWork.Loais.Update(category);
                await _unitOfWork.SaveAsync();
                TempData["Success"] = "Đã cập nhật danh mục!";
                return RedirectToAction(nameof(Categories));
            }
            return View(category);
        }

        [HttpPost("Categories/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _unitOfWork.Loais.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            // Kiểm tra xem có sản phẩm nào thuộc danh mục này không
            var hasProducts = await _unitOfWork.HangHoas.ExistsAsync(h => h.MaLoai == id);
            if (hasProducts)
            {
                TempData["Error"] = "Không thể xóa danh mục có sản phẩm!";
                return RedirectToAction(nameof(Categories));
            }

            _unitOfWork.Loais.Remove(category);
            await _unitOfWork.SaveAsync();
            TempData["Success"] = "Đã xóa danh mục!";
            return RedirectToAction(nameof(Categories));
        }

        // ==================== QUẢN LÝ PRODUCTS ====================
        
        [HttpGet("Products")]
        public async Task<IActionResult> Products(string? search, int? categoryId)
        {
            // Lấy tất cả sản phẩm
            var allProductsList = (await _unitOfWork.HangHoas.GetAllAsync()).ToList();
            var products = allProductsList.AsEnumerable();

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(h => h.TenHH!.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (categoryId.HasValue)
            {
                products = products.Where(h => h.MaLoai == categoryId.Value);
            }

            // Load categories for products
            var categories = await _unitOfWork.Loais.GetAllAsync();
            foreach (var product in products)
            {
                product.Loai = categories.FirstOrDefault(l => l.MaLoai == product.MaLoai);
            }

            ViewBag.Categories = categories;
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;

            return View(products.ToList());
        }

        [HttpGet("Products/Create")]
        public async Task<IActionResult> CreateProduct()
        {
            ViewBag.Categories = await _unitOfWork.Loais.GetAllAsync();
            return View();
        }

        [HttpPost("Products/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(HangHoa product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                // Upload ảnh nếu có
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    var uploadPath = Path.Combine(_environment.WebRootPath, "images", "products");
                    
                    // Tạo thư mục nếu chưa có
                    Directory.CreateDirectory(uploadPath);
                    
                    var filePath = Path.Combine(uploadPath, fileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    
                    product.Hinh = $"/images/products/{fileName}";
                }

                await _unitOfWork.HangHoas.AddAsync(product);
                await _unitOfWork.SaveAsync();
                TempData["Success"] = "Đã thêm sản phẩm mới!";
                return RedirectToAction(nameof(Products));
            }

            ViewBag.Categories = await _unitOfWork.Loais.GetAllAsync();
            return View(product);
        }

        [HttpGet("Products/Edit/{id}")]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _unitOfWork.GetProductByIdWithCategoryAsync(id);
            if (product == null)
                return NotFound();

            ViewBag.Categories = await _unitOfWork.Loais.GetAllAsync();
            return View(product);
        }

        [HttpPost("Products/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, HangHoa product, IFormFile? imageFile)
        {
            if (id != product.MaHH)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existingProduct = await _unitOfWork.HangHoas.GetByIdAsync(id);
                if (existingProduct == null)
                    return NotFound();
                
                // Cập nhật properties của existingProduct thay vì update instance mới
                existingProduct.TenHH = product.TenHH;
                existingProduct.MaLoai = product.MaLoai;
                existingProduct.DonGia = product.DonGia;
                existingProduct.SoLuongTon = product.SoLuongTon;
                existingProduct.GiamGia = product.GiamGia;
                existingProduct.MoTa = product.MoTa;
                
                // Upload ảnh mới nếu có
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    var uploadPath = Path.Combine(_environment.WebRootPath, "images", "products");
                    Directory.CreateDirectory(uploadPath);
                    var filePath = Path.Combine(uploadPath, fileName);
                    
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    
                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(existingProduct.Hinh))
                    {
                        var oldImagePath = Path.Combine(_environment.WebRootPath, existingProduct.Hinh.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    
                    existingProduct.Hinh = $"/images/products/{fileName}";
                }
                // Nếu không có ảnh mới, giữ nguyên ảnh cũ (không cần làm gì)

                _unitOfWork.HangHoas.Update(existingProduct);
                await _unitOfWork.SaveAsync();
                TempData["Success"] = "Đã cập nhật sản phẩm!";
                return RedirectToAction(nameof(Products));
            }

            ViewBag.Categories = await _unitOfWork.Loais.GetAllAsync();
            return View(product);
        }

        [HttpPost("Products/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _unitOfWork.HangHoas.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            // Xóa ảnh nếu có
            if (!string.IsNullOrEmpty(product.Hinh))
            {
                var imagePath = Path.Combine(_environment.WebRootPath, product.Hinh.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _unitOfWork.HangHoas.Remove(product);
            await _unitOfWork.SaveAsync();
            TempData["Success"] = "Đã xóa sản phẩm!";
            return RedirectToAction(nameof(Products));
        }

        // ==================== QUẢN LÝ ORDERS ====================
        
        [HttpGet("Orders")]
        public async Task<IActionResult> Orders(string? status)
        {
            var allOrders = (await _unitOfWork.HoaDons.GetAllAsync()).ToList();
            
            var orders = allOrders.AsEnumerable();

            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(h => h.Status == status);
            }

            orders = orders.OrderByDescending(h => h.NgayDat);

            // Load users for orders
            var users = await _unitOfWork.Users.GetAllAsync();
            foreach (var order in orders)
            {
                order.User = users.FirstOrDefault(u => u.MaUser == order.MaUser);
            }

            ViewBag.Status = status;
            return View(orders.ToList());
        }

        [HttpGet("Orders/Details/{id}")]
        public async Task<IActionResult> OrderDetails(int id)
        {
            // Lấy order với tất cả details
            var order = (await _unitOfWork.HoaDons.GetAllAsync())
                .FirstOrDefault(h => h.MaHD == id);

            if (order == null)
                return NotFound();

            // Load related data manually
            var user = await _unitOfWork.Users.GetByIdAsync(order.MaUser);
            order.User = user;

            var chiTietHDs = (await _unitOfWork.ChiTietHDs.FindAsync(ct => ct.MaHD == id)).ToList();
            
            // Load products for each detail
            foreach (var ct in chiTietHDs)
            {
                ct.HangHoa = await _unitOfWork.HangHoas.GetByIdAsync(ct.MaHH);
            }
            
            order.ChiTietHDs = chiTietHDs;

            // Load payment transaction
            var payment = (await _unitOfWork.PaymentTransactions.FindAsync(p => p.MaHD == id)).FirstOrDefault();
            if (payment != null)
            {
                order.PaymentTransaction = payment;
            }

            return View(order);
        }

        [HttpPost("Orders/UpdateStatus/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            var order = await _unitOfWork.HoaDons.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            order.Status = status;
            
            if (status == "Completed")
            {
                order.NgayGiao = DateTime.Now;
            }

            _unitOfWork.HoaDons.Update(order);
            await _unitOfWork.SaveAsync();
            
            TempData["Success"] = $"Đã cập nhật trạng thái đơn hàng #{id}";
            return RedirectToAction(nameof(OrderDetails), new { id });
        }

        [HttpPost("Orders/Cancel/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _unitOfWork.HoaDons.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            if (order.Status == "Completed" || order.Status == "Cancelled")
            {
                TempData["Error"] = "Không thể hủy đơn hàng này!";
                return RedirectToAction(nameof(OrderDetails), new { id });
            }

            // Hoàn trả số lượng tồn kho
            var chiTietHDs = await _unitOfWork.ChiTietHDs.FindAsync(ct => ct.MaHD == id);
            foreach (var detail in chiTietHDs)
            {
                var product = await _unitOfWork.HangHoas.GetByIdAsync(detail.MaHH);
                if (product != null)
                {
                    product.SoLuongTon += detail.SoLuong;
                    _unitOfWork.HangHoas.Update(product);
                }
            }

            order.Status = "Cancelled";
            _unitOfWork.HoaDons.Update(order);
            await _unitOfWork.SaveAsync();

            TempData["Success"] = $"Đã hủy đơn hàng #{id}";
            return RedirectToAction(nameof(OrderDetails), new { id });
        }

        // ==================== STATISTICS ====================
        
        [HttpGet("Statistics")]
        public async Task<IActionResult> Statistics()
        {
            var today = DateTime.Today;
            var thisMonth = new DateTime(today.Year, today.Month, 1);
            var thisYear = new DateTime(today.Year, 1, 1);

            var allOrders = (await _unitOfWork.HoaDons.GetAllAsync()).ToList();
            var completedOrders = allOrders.Where(h => h.Status == "Completed").ToList();

            var stats = new
            {
                // Doanh thu
                RevenueToday = completedOrders
                    .Where(h => h.NgayDat.Date == today)
                    .Sum(h => h.TongTien),
                RevenueThisMonth = completedOrders
                    .Where(h => h.NgayDat >= thisMonth)
                    .Sum(h => h.TongTien),
                RevenueThisYear = completedOrders
                    .Where(h => h.NgayDat >= thisYear)
                    .Sum(h => h.TongTien),

                // Đơn hàng
                OrdersToday = allOrders.Count(h => h.NgayDat.Date == today),
                OrdersThisMonth = allOrders.Count(h => h.NgayDat >= thisMonth),
                OrdersThisYear = allOrders.Count(h => h.NgayDat >= thisYear),

                // Sản phẩm bán chạy
                TopProducts = (await GetTopProductsAsync()).Take(10).ToList(),

                // Doanh thu theo tháng
                MonthlyRevenue = completedOrders
                    .Where(h => h.NgayDat >= thisYear)
                    .GroupBy(h => h.NgayDat.Month)
                    .Select(g => new
                    {
                        Month = g.Key,
                        Revenue = g.Sum(h => h.TongTien)
                    })
                    .OrderBy(x => x.Month)
                    .ToList()
            };

            return View(stats);
        }

        private async Task<IEnumerable<dynamic>> GetTopProductsAsync()
        {
            var chiTietHDs = (await _unitOfWork.ChiTietHDs.GetAllAsync()).ToList();
            
            var grouped = chiTietHDs
                .GroupBy(ct => ct.MaHH)
                .Select(g => new
                {
                    MaHH = g.Key,
                    TotalSold = g.Sum(ct => ct.SoLuong),
                    TotalRevenue = g.Sum(ct => ct.SoLuong * ct.DonGia)
                })
                .OrderByDescending(x => x.TotalSold)
                .ToList();

            var result = new List<dynamic>();
            foreach (var item in grouped)
            {
                var product = await _unitOfWork.HangHoas.GetByIdAsync(item.MaHH);
                result.Add(new
                {
                    Product = product,
                    item.TotalSold,
                    item.TotalRevenue
                });
            }

            return result;
        }

        // ==================== QUẢN LÝ REVIEWS ====================
        
        [HttpGet("Reviews")]
        public async Task<IActionResult> Reviews(string? status)
        {
            var allReviews = (await _unitOfWork.ProductReviews.GetAllAsync()).ToList();
            
            var reviews = allReviews.AsEnumerable();

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "Approved")
                    reviews = reviews.Where(r => r.IsApproved == true);
                else if (status == "Pending")
                    reviews = reviews.Where(r => r.IsApproved == false);
            }

            reviews = reviews.OrderByDescending(r => r.NgayTao);

            // Load related data
            var products = await _unitOfWork.HangHoas.GetAllAsync();
            var users = await _unitOfWork.Users.GetAllAsync();
            
            foreach (var review in reviews)
            {
                review.HangHoa = products.FirstOrDefault(p => p.MaHH == review.MaHH);
                review.User = users.FirstOrDefault(u => u.MaUser == review.MaUser);
            }

            ViewBag.Status = status;
            return View(reviews.ToList());
        }

        [HttpPost("Reviews/Approve/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveReview(int id)
        {
            var review = await _unitOfWork.ProductReviews.GetByIdAsync(id);
            if (review == null)
                return NotFound();

            review.IsApproved = true;
            _unitOfWork.ProductReviews.Update(review);
            await _unitOfWork.SaveAsync();

            TempData["Success"] = "Đã phê duyệt đánh giá!";
            return RedirectToAction(nameof(Reviews));
        }

        [HttpPost("Reviews/Reject/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectReview(int id)
        {
            var review = await _unitOfWork.ProductReviews.GetByIdAsync(id);
            if (review == null)
                return NotFound();

            _unitOfWork.ProductReviews.Remove(review);
            await _unitOfWork.SaveAsync();

            TempData["Success"] = "Đã từ chối đánh giá!";
            return RedirectToAction(nameof(Reviews));
        }

        [HttpPost("Reviews/Response/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReviewResponse(int id, string response)
        {
            var review = await _unitOfWork.ProductReviews.GetByIdAsync(id);
            if (review == null)
                return NotFound();

            review.AdminResponse = response;
            _unitOfWork.ProductReviews.Update(review);
            await _unitOfWork.SaveAsync();

            TempData["Success"] = "Đã thêm phản hồi!";
            return RedirectToAction(nameof(Reviews));
        }
    }
}

