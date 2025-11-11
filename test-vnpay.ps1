# Script test VNPay integration
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  VNPay Integration Test" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Test 1: Kiểm tra ứng dụng đang chạy
Write-Host "Test 1: Kiểm tra ứng dụng đang chạy..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000" -Method GET -UseBasicParsing -ErrorAction Stop
    if ($response.StatusCode -eq 200) {
        Write-Host "✓ Ứng dụng đang chạy trên http://localhost:5000" -ForegroundColor Green
    }
} catch {
    Write-Host "✗ Ứng dụng không chạy hoặc không thể kết nối" -ForegroundColor Red
    Write-Host "  Vui lòng chạy ứng dụng: cd WebShop; dotnet run" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "Test 2: Kiểm tra cấu hình VNPay..." -ForegroundColor Yellow

# Kiểm tra appsettings.json
$appsettingsPath = "WebShop\appsettings.json"
if (Test-Path $appsettingsPath) {
    $appsettings = Get-Content $appsettingsPath -Raw | ConvertFrom-Json
    if ($appsettings.VnPay) {
        Write-Host "✓ Cấu hình VNPay đã có trong appsettings.json" -ForegroundColor Green
        Write-Host "  - TmnCode: $($appsettings.VnPay.TmnCode)" -ForegroundColor Gray
        Write-Host "  - HashSecret: $($appsettings.VnPay.HashSecret.Substring(0, 10))..." -ForegroundColor Gray
        Write-Host "  - Url: $($appsettings.VnPay.Url)" -ForegroundColor Gray
    } else {
        Write-Host "✗ Không tìm thấy cấu hình VNPay" -ForegroundColor Red
    }
} else {
    Write-Host "✗ Không tìm thấy appsettings.json" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Tóm tắt Test" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "🌐 Ứng dụng đang chạy tại: http://localhost:5000" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Các bước test thủ công:" -ForegroundColor Yellow
Write-Host "1. Mở trình duyệt: http://localhost:5000" -ForegroundColor White
Write-Host "2. Đăng nhập (admin / Admin@123 hoặc user1 / User@123)" -ForegroundColor White
Write-Host "3. Thêm sản phẩm vào giỏ hàng" -ForegroundColor White
Write-Host "4. Vào trang Checkout (/Checkout)" -ForegroundColor White
Write-Host "5. Chọn phương thức thanh toán VNPay" -ForegroundColor White
Write-Host "6. Nhập thông tin giao hàng và click 'Đặt Hàng'" -ForegroundColor White
Write-Host "7. Kiểm tra xem có redirect đến VNPay không" -ForegroundColor White
Write-Host ""
Write-Host "💳 Thông tin thẻ test VNPay:" -ForegroundColor Yellow
Write-Host "  - Ngân hàng: NCB" -ForegroundColor White
Write-Host "  - Số thẻ: 9704198526191432198" -ForegroundColor White
Write-Host "  - Tên chủ thẻ: NGUYEN VAN A" -ForegroundColor White
Write-Host "  - Ngày phát hành: 07/15" -ForegroundColor White
Write-Host "  - Mật khẩu OTP: 123456" -ForegroundColor White
Write-Host ""
Write-Host "📝 Lưu ý:" -ForegroundColor Yellow
Write-Host "  - IPN URL cần được cấu hình trong VNPay Merchant Admin" -ForegroundColor White
Write-Host "  - Trong development, sử dụng ngrok để expose local server" -ForegroundColor White
Write-Host "  - Xem VNPAY_SETUP.md để biết thêm chi tiết" -ForegroundColor White
Write-Host ""
