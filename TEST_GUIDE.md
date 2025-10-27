# Hướng Dẫn Test Demo

## URL ứng dụng đang chạy:
**http://localhost:5073**

## Các bước test:

### 1. Đăng nhập/Đăng ký tài khoản mới
```
URL: http://localhost:5073/Authentication/Register
- Email: test@example.com
- Mật khẩu: Test123456
- Họ tên: Nguyễn Văn Test
```

### 2. Xem sản phẩm
```
URL: http://localhost:5073/HangHoa
```

### 3. Thêm vào giỏ hàng
- Click vào một sản phẩm
- Nhập số lượng
- Click "Thêm vào giỏ hàng"

### 4. Thanh toán
```
URL: http://localhost:5073/Checkout
- Nhập địa chỉ giao hàng
- Nhập số điện thoại
- Click "Thanh Toán"
```

### 5. Xem lịch sử giao dịch
```
URL: http://localhost:5073/TransactionHistory
```

## Nếu có lỗi:

### Lỗi database connection
Nếu thấy lỗi SQL Server:
```bash
# Kiểm tra LocalDB đã cài chưa
sqllocaldb versions

# Nếu chưa có, cài SQL Server LocalDB
# https://www.microsoft.com/en-us/sql-server/sql-server-downloads
```

### Lỗi "session expired"
- Refresh lại trang
- Đăng nhập lại

### Lỗi 404
- Kiểm tra route có đúng không
- Kiểm tra controller đã có trong Program.cs chưa

## Debug Console (Browser):
1. Mở Developer Tools (F12)
2. Vào tab "Console" 
3. Xem lỗi JavaScript (nếu có)

## Debug Server:
Kiểm tra terminal output để xem log lỗi chi tiết.

