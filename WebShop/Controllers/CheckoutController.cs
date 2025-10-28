using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Security.Claims;

namespace WebShop.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly MiniMarketDbContext _context;

    public CheckoutController(MiniMarketDbContext context)
    {
        _context = context;
    }

    // GET: /Checkout
    public IActionResult Index()
    {
        // Hiển thị trang checkout với giỏ hàng
        return View();
    }

    // POST: /Checkout/CreateOrder
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CheckoutRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Lấy user ID từ claims
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            // 1. Tạo đơn hàng (HoaDon)
            var hoaDon = new HoaDon
            {
                MaUser = userId,
                NgayDat = DateTime.Now,
                DiaChiGiao = request.DiaChiGiao,
                SoDienThoai = request.SoDienThoai,
                PhiVanChuyen = request.PhiVanChuyen,
                Status = "Pending",
                GhiChu = request.GhiChu
            };

            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync();

            // 2. Tạo chi tiết đơn hàng (ChiTietHD)
            decimal tongTien = 0;
            foreach (var item in request.Items)
            {
                var hangHoa = await _context.HangHoas.FindAsync(item.MaHH);
                if (hangHoa == null)
                {
                    return BadRequest($"Không tìm thấy hàng hóa với mã {item.MaHH}");
                }

                // Kiểm tra số lượng tồn
                if (hangHoa.SoLuongTon < item.SoLuong)
                {
                    return BadRequest($"Không đủ hàng trong kho cho sản phẩm {hangHoa.TenHH}");
                }

                var chiTiet = new ChiTietHD
                {
                    MaHD = hoaDon.MaHD,
                    MaHH = item.MaHH,
                    DonGia = hangHoa.DonGia,
                    SoLuong = item.SoLuong,
                    GiamGia = hangHoa.GiamGia * item.SoLuong
                };

                _context.ChiTietHDs.Add(chiTiet);
                tongTien += chiTiet.ThanhTien;

                // Cập nhật số lượng tồn
                hangHoa.SoLuongTon -= item.SoLuong;
            }

            // Cập nhật tổng tiền cho đơn hàng
            hoaDon.TongTien = tongTien + hoaDon.PhiVanChuyen;
            await _context.SaveChangesAsync();

            // 3. Tạo bản ghi PaymentTransaction (status = Pending)
            var paymentTransaction = new PaymentTransaction
            {
                MaHD = hoaDon.MaHD,
                SoTien = hoaDon.TongTien,
                Status = "Pending",
                PhuongThucTT = request.PhuongThucTT,
                NgayTao = DateTime.Now,
                GhiChu = "Chờ thanh toán"
            };

            _context.PaymentTransactions.Add(paymentTransaction);
            await _context.SaveChangesAsync();

            // Trả về kết quả với redirect URL
            return Ok(new
            {
                success = true,
                message = "Đặt hàng thành công",
                redirectUrl = Url.Action("Success", "Order", new { id = hoaDon.MaHD }),
                data = new
                {
                    maHD = hoaDon.MaHD,
                    tongTien = hoaDon.TongTien,
                    status = hoaDon.Status,
                    paymentTransaction = new
                    {
                        maGD = paymentTransaction.MaGD,
                        status = paymentTransaction.Status,
                        phuongThucTT = paymentTransaction.PhuongThucTT
                    }
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi tạo đơn hàng",
                error = ex.Message
            });
        }
    }

    // POST: /Checkout/UpdatePaymentStatus
    [HttpPost]
    public async Task<IActionResult> UpdatePaymentStatus([FromBody] PaymentStatusRequest request)
    {
        var transaction = await _context.PaymentTransactions
            .Include(pt => pt.HoaDon)
            .FirstOrDefaultAsync(pt => pt.MaGD == request.MaGD);

        if (transaction == null)
        {
            return NotFound("Không tìm thấy giao dịch");
        }

        // Cập nhật trạng thái thanh toán
        transaction.Status = request.Status;
        transaction.NgayCapNhat = DateTime.Now;
        transaction.GhiChu = request.GhiChu;

        // Cập nhật trạng thái đơn hàng
        if (transaction.HoaDon != null)
        {
            transaction.HoaDon.Status = request.Status switch
            {
                "Completed" => "Processing",
                "Failed" => "Cancelled",
                "Cancelled" => "Cancelled",
                _ => transaction.HoaDon.Status
            };
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            success = true,
            message = "Cập nhật trạng thái thành công",
            data = new
            {
                maGD = transaction.MaGD,
                status = transaction.Status,
                maHD = transaction.MaHD,
                hoaDonStatus = transaction.HoaDon?.Status
            }
        });
    }

    // GET: /Checkout/GetOrderStatus/{maHD}
    [HttpGet]
    public async Task<IActionResult> GetOrderStatus(int maHD)
    {
        var hoaDon = await _context.HoaDons
            .Include(hd => hd.ChiTietHDs)
                .ThenInclude(ct => ct.HangHoa)
            .Include(hd => hd.PaymentTransaction)
            .Include(hd => hd.User)
            .FirstOrDefaultAsync(hd => hd.MaHD == maHD);

        if (hoaDon == null)
        {
            return NotFound("Không tìm thấy đơn hàng");
        }

        return Ok(new
        {
            success = true,
            data = new
            {
                maHD = hoaDon.MaHD,
                ngayDat = hoaDon.NgayDat,
                diaChiGiao = hoaDon.DiaChiGiao,
                soDienThoai = hoaDon.SoDienThoai,
                tongTien = hoaDon.TongTien,
                phiVanChuyen = hoaDon.PhiVanChuyen,
                status = hoaDon.Status,
                ghiChu = hoaDon.GhiChu,
                khachHang = new
                {
                    hoTen = hoaDon.User?.HoTen,
                    email = hoaDon.User?.Email
                },
                chiTiet = hoaDon.ChiTietHDs?.Select(ct => new
                {
                    tenHH = ct.HangHoa?.TenHH,
                    donGia = ct.DonGia,
                    soLuong = ct.SoLuong,
                    giamGia = ct.GiamGia,
                    thanhTien = ct.ThanhTien
                }),
                paymentTransaction = hoaDon.PaymentTransaction == null ? null : new
                {
                    maGD = hoaDon.PaymentTransaction.MaGD,
                    soTien = hoaDon.PaymentTransaction.SoTien,
                    status = hoaDon.PaymentTransaction.Status,
                    phuongThucTT = hoaDon.PaymentTransaction.PhuongThucTT,
                    ngayTao = hoaDon.PaymentTransaction.NgayTao,
                    ngayCapNhat = hoaDon.PaymentTransaction.NgayCapNhat
                }
            }
        });
    }
}

// DTO cho request
public class CheckoutRequest
{
    public int MaUser { get; set; }
    public string DiaChiGiao { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
    public decimal PhiVanChuyen { get; set; }
    public string? GhiChu { get; set; }
    public string PhuongThucTT { get; set; } = "COD"; // COD, Bank Transfer, E-Wallet
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    public int MaHH { get; set; }
    public int SoLuong { get; set; }
}

public class PaymentStatusRequest
{
    public int MaGD { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? GhiChu { get; set; }
}

