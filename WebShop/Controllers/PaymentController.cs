using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Security.Claims;
using System.Text;

namespace WebShop.Controllers;

[Authorize]
public class PaymentController : Controller
{
    private readonly MiniMarketDbContext _context;
    private readonly IVnPayService _vnPayService;
    private readonly ILogger<PaymentController> _logger;
    private readonly IConfiguration _configuration;

    public PaymentController(
        MiniMarketDbContext context,
        IVnPayService vnPayService,
        ILogger<PaymentController> logger,
        IConfiguration configuration)
    {
        _context = context;
        _vnPayService = vnPayService;
        _logger = logger;
        _configuration = configuration;
    }

    // POST: /Payment/CreateVnPayPayment
    [HttpPost]
    public async Task<IActionResult> CreateVnPayPayment([FromBody] VnPayPaymentRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Lấy đơn hàng
            var hoaDon = await _context.HoaDons
                .Include(hd => hd.PaymentTransaction)
                .FirstOrDefaultAsync(hd => hd.MaHD == request.OrderId && hd.MaUser == userId);

            if (hoaDon == null)
            {
                return NotFound("Không tìm thấy đơn hàng");
            }

            // Kiểm tra đơn hàng đã thanh toán chưa
            if (hoaDon.Status == "Completed" || hoaDon.PaymentTransaction?.Status == "Completed")
            {
                return BadRequest("Đơn hàng đã được thanh toán");
            }

            // Tạo hoặc cập nhật payment transaction
            PaymentTransaction? paymentTransaction = hoaDon.PaymentTransaction;
            if (paymentTransaction == null)
            {
                paymentTransaction = new PaymentTransaction
                {
                    MaHD = hoaDon.MaHD,
                    SoTien = hoaDon.TongTien,
                    Status = "Pending",
                    PhuongThucTT = "VNPay",
                    NgayTao = DateTime.Now,
                    GhiChu = "Đang chờ thanh toán qua VNPay"
                };
                _context.PaymentTransactions.Add(paymentTransaction);
                await _context.SaveChangesAsync();
            }
            else
            {
                paymentTransaction.PhuongThucTT = "VNPay";
                paymentTransaction.Status = "Pending";
                paymentTransaction.GhiChu = "Đang chờ thanh toán qua VNPay";
                await _context.SaveChangesAsync();
            }

            // Lấy IP address của client
            // QUAN TRỌNG: VNPay yêu cầu public IP, không phải 127.0.0.1
            // Khi chạy trong Docker hoặc sau proxy, cần lấy từ header
            // Nếu không có public IP, sử dụng một IP hợp lệ (ví dụ: 192.168.1.1)
            var ipAddress = "192.168.1.1"; // Default IP thay vì 127.0.0.1
            
            // Ưu tiên lấy từ header X-Forwarded-For (khi đằng sau proxy)
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                var forwardedFor = Request.Headers["X-Forwarded-For"].ToString();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    ipAddress = forwardedFor.Split(',')[0].Trim();
                }
            }
            // Lấy từ header X-Real-IP (khi đằng sau nginx/proxy)
            else if (Request.Headers.ContainsKey("X-Real-IP"))
            {
                var realIp = Request.Headers["X-Real-IP"].ToString();
                if (!string.IsNullOrEmpty(realIp))
                {
                    ipAddress = realIp;
                }
            }
            // Lấy từ connection
            else
            {
                var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                if (!string.IsNullOrEmpty(remoteIp) && remoteIp != "::1" && remoteIp != "127.0.0.1")
                {
                    ipAddress = remoteIp;
                }
            }
            
            // QUAN TRỌNG: Nếu vẫn là localhost, cần thay bằng IP thực tế
            // Trong môi trường test, có thể sử dụng IP mạng local (ví dụ: 192.168.x.x)
            // Hoặc lấy public IP từ service bên ngoài
            if (ipAddress == "127.0.0.1" || ipAddress == "::1" || string.IsNullOrEmpty(ipAddress))
            {
                // Trong môi trường test/Docker, sử dụng IP mạng local
                // Hoặc có thể lấy từ cấu hình
                ipAddress = _configuration["VnPay:ClientIP"] ?? "192.168.1.1";
            }
            
            _logger.LogInformation($"Using IP Address for VNPay: {ipAddress}");
            
            // Tạo order info (chỉ chữ, số, không có ký tự đặc biệt)
            var orderInfo = $"Thanh toan don hang {hoaDon.MaHD}";
            
            // Tạo return URL (absolute URL)
            // QUAN TRỌNG: VNPay yêu cầu return URL phải là absolute URL và accessible từ internet
            // Khi chạy local/Docker, sử dụng localhost:5000
            var scheme = "http";
            var host = "localhost:5000";
            
            // Nếu có cấu hình trong appsettings, sử dụng cấu hình đó
            var configReturnUrl = _configuration["VnPay:ReturnUrl"];
            if (!string.IsNullOrEmpty(configReturnUrl))
            {
                var uri = new Uri(configReturnUrl);
                scheme = uri.Scheme;
                host = uri.Host + (uri.Port != 80 && uri.Port != 443 ? $":{uri.Port}" : "");
            }
            else
            {
                // Khi chạy trong Docker, Request.Host có thể là container hostname
                // Nhưng VNPay cần URL public, nên sử dụng localhost:5000
                if (Request.Host.Port.HasValue && (Request.Host.Port == 5000 || Request.Host.Port == 5001))
                {
                    host = $"localhost:{Request.Host.Port}";
                    scheme = Request.Scheme;
                }
                else if (Request.Host.Host == "localhost" || Request.Host.Host == "127.0.0.1")
                {
                    host = $"localhost:{Request.Host.Port ?? 5000}";
                    scheme = Request.Scheme;
                }
            }
            
            var returnUrl = $"{scheme}://{host}/Payment/VnPayReturn";
            
            _logger.LogInformation($"Creating VNPay payment - Order: {hoaDon.MaHD}, Amount: {hoaDon.TongTien}");
            _logger.LogInformation($"IP Address (before processing): {ipAddress}");
            _logger.LogInformation($"Return URL: {returnUrl}");
            _logger.LogInformation($"Order Info: {orderInfo}");
            
            var paymentUrl = _vnPayService.CreatePaymentUrl(
                hoaDon.MaHD,
                hoaDon.TongTien,
                orderInfo,
                returnUrl,
                ipAddress
            );
            
            _logger.LogInformation($"VNPay Payment URL created: {paymentUrl.Substring(0, Math.Min(200, paymentUrl.Length))}...");
            
            _logger.LogInformation($"VNPay Payment URL created (length: {paymentUrl.Length})");

            return Ok(new
            {
                success = true,
                paymentUrl = paymentUrl,
                orderId = hoaDon.MaHD,
                amount = hoaDon.TongTien
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating VNPay payment");
            return StatusCode(500, new
            {
                success = false,
                message = "Có lỗi xảy ra khi tạo thanh toán VNPay",
                error = ex.Message
            });
        }
    }

    // GET: /Payment/VnPayReturn
    [HttpGet]
    public async Task<IActionResult> VnPayReturn()
    {
        try
        {
            // Lấy query string từ VNPay
            var queryString = Request.QueryString.ToString();
            var vnpayData = _vnPayService.ParseReturnUrl(queryString);

            if (!vnpayData.Any())
            {
                return RedirectToAction("Failed", "Order", new { message = "Không nhận được dữ liệu từ VNPay" });
            }

            // Verify signature
            var vnpHashSecret = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["VnPay:HashSecret"]!;
            if (!_vnPayService.VerifyPaymentSignature(vnpayData, vnpHashSecret))
            {
                _logger.LogWarning("VNPay signature verification failed");
                return RedirectToAction("Failed", "Order", new { message = "Xác thực chữ ký thất bại" });
            }

            // Lấy thông tin từ VNPay response
            var vnpResponseCode = vnpayData.GetValueOrDefault("vnp_ResponseCode", "");
            var vnpTransactionStatus = vnpayData.GetValueOrDefault("vnp_TransactionStatus", "");
            var vnpTxnRef = vnpayData.GetValueOrDefault("vnp_TxnRef", "");
            var vnpTransactionNo = vnpayData.GetValueOrDefault("vnp_TransactionNo", "");
            var vnpAmount = vnpayData.GetValueOrDefault("vnp_Amount", "0");

            // Parse order ID từ vnpTxnRef (format: YYYYMMDDHHmmss + orderId)
            if (vnpTxnRef.Length < 14)
            {
                return RedirectToAction("Failed", "Order", new { message = "Mã đơn hàng không hợp lệ" });
            }

            var orderIdStr = vnpTxnRef.Substring(14);
            if (!int.TryParse(orderIdStr, out var orderId))
            {
                return RedirectToAction("Failed", "Order", new { message = "Mã đơn hàng không hợp lệ" });
            }

            // Lấy đơn hàng và payment transaction
            var hoaDon = await _context.HoaDons
                .Include(hd => hd.PaymentTransaction)
                .FirstOrDefaultAsync(hd => hd.MaHD == orderId);

            if (hoaDon == null)
            {
                return RedirectToAction("Failed", "Order", new { message = "Không tìm thấy đơn hàng" });
            }

            var paymentTransaction = hoaDon.PaymentTransaction;
            if (paymentTransaction == null)
            {
                return RedirectToAction("Failed", "Order", new { message = "Không tìm thấy giao dịch thanh toán" });
            }

            // Kiểm tra số tiền
            var amount = decimal.Parse(vnpAmount) / 100; // VNPay trả về số tiền * 100
            if (Math.Abs(amount - hoaDon.TongTien) > 1000) // Cho phép sai số 1000 VND
            {
                _logger.LogWarning($"Amount mismatch: Expected {hoaDon.TongTien}, Got {amount}");
                return RedirectToAction("Failed", "Order", new { message = "Số tiền thanh toán không khớp" });
            }

            // Xử lý kết quả thanh toán
            if (vnpResponseCode == "00" && vnpTransactionStatus == "00")
            {
                // Thanh toán thành công
                paymentTransaction.Status = "Completed";
                paymentTransaction.NgayCapNhat = DateTime.Now;
                paymentTransaction.GhiChu = $"Thanh toán thành công qua VNPay. Mã GD: {vnpTransactionNo}";
                
                hoaDon.Status = "Processing";
                
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Payment successful for order {orderId}, VNPay transaction: {vnpTransactionNo}");

                return RedirectToAction("Success", "Order", new { id = orderId });
            }
            else
            {
                // Thanh toán thất bại
                paymentTransaction.Status = "Failed";
                paymentTransaction.NgayCapNhat = DateTime.Now;
                paymentTransaction.GhiChu = $"Thanh toán thất bại. Mã lỗi: {vnpResponseCode}";
                
                hoaDon.Status = "Pending";
                
                await _context.SaveChangesAsync();

                _logger.LogWarning($"Payment failed for order {orderId}, Response code: {vnpResponseCode}");

                return RedirectToAction("Failed", "Order", new { 
                    message = $"Thanh toán thất bại. Mã lỗi: {vnpResponseCode}" 
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing VNPay return");
            return RedirectToAction("Failed", "Order", new { message = "Có lỗi xảy ra khi xử lý thanh toán" });
        }
    }

    // GET/POST: /Payment/VnPayIPN
    [HttpPost]
    [HttpGet]
    [AllowAnonymous] // IPN callback từ VNPay server
    public async Task<IActionResult> VnPayIPN()
    {
        try
        {
            // VNPay có thể gửi IPN qua query string hoặc form data
            Dictionary<string, string> vnpayData;
            
            if (Request.HasFormContentType)
            {
                // Form data
                vnpayData = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
            }
            else if (Request.QueryString.HasValue)
            {
                // Query string
                vnpayData = _vnPayService.ParseReturnUrl(Request.QueryString.ToString());
            }
            else
            {
                // Request body
                string body;
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    body = await reader.ReadToEndAsync();
                }
                vnpayData = _vnPayService.ParseReturnUrl(body);
            }

            if (!vnpayData.Any())
            {
                _logger.LogWarning("VNPay IPN: No data received");
                return BadRequest("No data");
            }

            // Verify signature
            var vnpHashSecret = HttpContext.RequestServices.GetRequiredService<IConfiguration>()["VnPay:HashSecret"]!;
            if (!_vnPayService.VerifyPaymentSignature(vnpayData, vnpHashSecret))
            {
                _logger.LogWarning("VNPay IPN: Signature verification failed");
                return BadRequest("Invalid signature");
            }

            // Lấy thông tin từ VNPay
            var vnpResponseCode = vnpayData.GetValueOrDefault("vnp_ResponseCode", "");
            var vnpTransactionStatus = vnpayData.GetValueOrDefault("vnp_TransactionStatus", "");
            var vnpTxnRef = vnpayData.GetValueOrDefault("vnp_TxnRef", "");
            var vnpTransactionNo = vnpayData.GetValueOrDefault("vnp_TransactionNo", "");
            var vnpAmount = vnpayData.GetValueOrDefault("vnp_Amount", "0");

            // Parse order ID
            if (vnpTxnRef.Length < 14)
            {
                return BadRequest("Invalid order ID");
            }

            var orderIdStr = vnpTxnRef.Substring(14);
            if (!int.TryParse(orderIdStr, out var orderId))
            {
                return BadRequest("Invalid order ID");
            }

            // Lấy đơn hàng
            var hoaDon = await _context.HoaDons
                .Include(hd => hd.PaymentTransaction)
                .FirstOrDefaultAsync(hd => hd.MaHD == orderId);

            if (hoaDon == null)
            {
                return BadRequest("Order not found");
            }

            var paymentTransaction = hoaDon.PaymentTransaction;
            if (paymentTransaction == null)
            {
                return BadRequest("Payment transaction not found");
            }

            // Kiểm tra số tiền
            var amount = decimal.Parse(vnpAmount) / 100;
            if (Math.Abs(amount - hoaDon.TongTien) > 1000)
            {
                _logger.LogWarning($"VNPay IPN: Amount mismatch for order {orderId}");
                return BadRequest("Amount mismatch");
            }

            // Chỉ cập nhật nếu transaction chưa được xử lý
            if (paymentTransaction.Status == "Pending")
            {
                if (vnpResponseCode == "00" && vnpTransactionStatus == "00")
                {
                    paymentTransaction.Status = "Completed";
                    paymentTransaction.NgayCapNhat = DateTime.Now;
                    paymentTransaction.GhiChu = $"IPN: Thanh toán thành công. Mã GD: {vnpTransactionNo}";
                    hoaDon.Status = "Processing";
                }
                else
                {
                    paymentTransaction.Status = "Failed";
                    paymentTransaction.NgayCapNhat = DateTime.Now;
                    paymentTransaction.GhiChu = $"IPN: Thanh toán thất bại. Mã lỗi: {vnpResponseCode}";
                    hoaDon.Status = "Pending";
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"VNPay IPN processed for order {orderId}");
            }

            // Trả về success cho VNPay
            return Ok(new { RspCode = "00", Message = "Success" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing VNPay IPN");
            return StatusCode(500, new { RspCode = "99", Message = "Error" });
        }
    }
}

// DTO
public class VnPayPaymentRequest
{
    public int OrderId { get; set; }
}

