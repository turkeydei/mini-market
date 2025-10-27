# Hướng Dẫn Demo Localhost - Thanh Toán và Lịch Sử Giao Dịch

## Tổng Quan
Đã thêm chức năng **Thanh Toán (Checkout)** và **Xem Lịch Sử Giao Dịch** vào dự án Mini Market.

## Các Chức Năng Mới

### 1. Thanh Toán (Checkout)
- **Thêm vào giỏ hàng**: Tại trang chi tiết sản phẩm, người dùng có thể thêm sản phẩm vào giỏ hàng
- **Xem giỏ hàng**: Menu "Giỏ Hàng" để xem các sản phẩm đã thêm
- **Thanh toán**: Nhập địa chỉ, số điện thoại và xác nhận đơn hàng
- **Tạo đơn hàng**: Tự động tạo HoaDon và LichSuGiaoDich sau khi thanh toán

### 2. Lịch Sử Giao Dịch
- **Xem tất cả giao dịch**: Menu "Lịch Sử GD"
- **Chi tiết giao dịch**: Xem thông tin chi tiết từng giao dịch
- **Liên kết đơn hàng**: Có thể xem đơn hàng liên quan

## Cách Sử Dụng

### Bước 1: Chạy ứng dụng
```bash
cd WebShop
dotnet run
```

### Bước 2: Đăng nhập hoặc đăng ký
1. Truy cập http://localhost:5000
2. Click "Đăng Nhập" hoặc "Đăng Ký"
3. Nhập email và mật khẩu

### Bước 3: Mua sắm và thanh toán
1. Vào "Sản Phẩm" để xem danh sách sản phẩm
2. Click vào một sản phẩm để xem chi tiết
3. Nhập số lượng và click "Thêm vào giỏ hàng"
4. Click "Giỏ Hàng" ở menu để xem giỏ hàng
5. Nhập địa chỉ và số điện thoại
6. Click "Thanh Toán"
7. Đơn hàng sẽ được tạo và chuyển đến trang chi tiết đơn hàng

### Bước 4: Xem lịch sử giao dịch
1. Click "Lịch Sử GD" ở menu
2. Xem danh sách tất cả các giao dịch
3. Click "Chi tiết" để xem thông tin chi tiết

## Cấu Trúc Code

### DTOs (Application/Features/DTOs/)
- `CheckoutItemDto.cs` - Thông tin sản phẩm trong giỏ hàng
- `CheckoutRequestDto.cs` - Dữ liệu khi checkout
- `PaymentRequestDto.cs` - Dữ liệu thanh toán

### Services (Application/Features/Services/)
- `TransactionHistoryService.cs` - Service xử lý lịch sử giao dịch

### Controllers (WebShop/Controllers/)
- `CheckoutController.cs` - Xử lý giỏ hàng và thanh toán
  - Index: Hiển thị giỏ hàng
  - AddToCart: Thêm sản phẩm vào giỏ hàng
  - RemoveFromCart: Xóa sản phẩm khỏi giỏ hàng
  - UpdateCart: Cập nhật số lượng
  - ProcessPayment: Xử lý thanh toán
  
- `TransactionHistoryController.cs` - Quản lý lịch sử giao dịch
  - Index: Danh sách giao dịch
  - Details: Chi tiết giao dịch

### Views (WebShop/Views/)
- `Checkout/Index.cshtml` - Giao diện giỏ hàng và thanh toán
- `TransactionHistory/Index.cshtml` - Danh sách lịch sử giao dịch
- `TransactionHistory/Details.cshtml` - Chi tiết giao dịch

## Tính Năng Nổi Bật

1. **Session-based shopping cart**: Sử dụng session để lưu giỏ hàng tạm thời
2. **Validation**: Kiểm tra sản phẩm có tồn tại trước khi thêm vào giỏ hàng
3. **User-specific data**: Chỉ hiển thị giao dịch của người dùng đang đăng nhập
4. **Transaction history**: Tự động lưu lịch sử sau mỗi thanh toán
5. **Security**: Sử dụng AntiForgeryToken cho tất cả các POST request

## Lưu Ý

- Giỏ hàng chỉ lưu trong session, sẽ mất khi logout hoặc hết hạn session (30 phút)
- Cần đăng nhập để sử dụng chức năng thanh toán
- Payment provider mặc định là "VNPAY" (có thể mở rộng thêm các provider khác)
- Trạng thái thanh toán mặc định là "Success" (trong thực tế cần tích hợp với gateway thanh toán)

## Kế Hoạch Mở Rộng (Nếu cần)

1. Tích hợp gateway thanh toán thực tế (VNPAY, MoMo, etc.)
2. Thêm email notification khi đặt hàng thành công
3. Theo dõi trạng thái giao hàng
4. Thêm voucher/khuyến mãi
5. Xem lịch sử chi tiết với filter và search
6. Export PDF hóa đơn

