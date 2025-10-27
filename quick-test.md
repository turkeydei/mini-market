# Quick Test - Chạy Demo Localhost

## Bước 1: Ứng dụng đang chạy tại:
👉 **http://localhost:5073**

## Bước 2: Test theo thứ tự:

### ✅ Test 1: Đăng Ký
1. Mở browser: http://localhost:5073
2. Click "Đăng Ký"
3. Điền form:
   - Họ tên: Nguyễn Văn A
   - Email: test123@test.com
   - Mật khẩu: 123456
   - Xác nhận mật khẩu: 123456
   - Số điện thoại: 0123456789
4. Click "Đăng Ký"

### ✅ Test 2: Xem Sản Phẩm
1. Sau khi đăng nhập, click "Sản Phẩm"
2. Click vào một sản phẩm

### ✅ Test 3: Thêm Vào Giỏ Hàng
1. Tại trang chi tiết sản phẩm
2. Nhập số lượng (ví dụ: 2)
3. Click "Thêm vào giỏ hàng"
4. Sẽ có thông báo "Đã thêm sản phẩm vào giỏ hàng"

### ✅ Test 4: Xem Giỏ Hàng & Thanh Toán
1. Click menu "Giỏ Hàng"
2. Xem danh sách sản phẩm trong giỏ
3. Điền form:
   - Địa chỉ giao hàng: 123 Đường ABC, Quận 1, TP.HCM
   - Số điện thoại: 0123456789
   - Ghi chú: (có thể để trống)
4. Click "Thanh Toán"
5. Sẽ redirect đến trang đơn hàng

### ✅ Test 5: Xem Lịch Sử Giao Dịch
1. Click menu "Lịch Sử GD"
2. Xem danh sách các giao dịch đã thực hiện
3. Click "Chi tiết" để xem thông tin chi tiết

## Nếu có LỖI:

### Lỗi 1: "This site can't be reached"
→ Ứng dụng chưa chạy. Chạy lại:
```bash
cd WebShop
dotnet run
```

### Lỗi 2: "Database error"
→ Database chưa được tạo. Chạy:
```bash
dotnet ef database update --project Persistence/Persistence.csproj --startup-project WebShop/WebShop.csproj
```

### Lỗi 3: "Session not working"
→ Xóa cookies và thử lại

### Lỗi 4: Không thấy menu "Giỏ Hàng"
→ Chỉ hiện khi đã đăng nhập. Cần login trước.

## Screenshots cần có:
1. Trang Home
2. Danh sách sản phẩm
3. Chi tiết sản phẩm với nút "Thêm vào giỏ hàng"
4. Giỏ hàng với sản phẩm
5. Trang thanh toán
6. Đơn hàng sau khi thanh toán
7. Lịch sử giao dịch

## Video Demo Steps:
1. Đăng nhập
2. Xem sản phẩm
3. Thêm 2-3 sản phẩm vào giỏ
4. Thanh toán
5. Xem lịch sử giao dịch
6. Xem đơn hàng

