# Trạng thái Docker - Mini Market Project

## ✅ Trạng thái hiện tại

### Containers đang chạy:
- ✅ **minimarket-sqlserver**: SQL Server 2022 (port 1433)
- ✅ **minimarket-webshop**: ASP.NET Core WebShop (ports 5000, 5001)

### Cấu hình VNPay:
- ✅ **Terminal ID**: `UOSACQHJ`
- ✅ **Hash Secret**: `AA7C75JEUDM6BOAXSO0N7QYNTY74FVPG`
- ✅ **Payment URL**: `https://sandbox.vnpayment.vn/paymentv2/vpcpay.html`
- ✅ **Return URL**: `http://localhost:5000/Payment/VnPayReturn`
- ✅ **IPN URL**: `http://localhost:5000/Payment/VnPayIPN`

### Database:
- ✅ Database đã được tạo: `MiniMarketDB`
- ✅ Migrations đã được apply
- ✅ Seed data đã được thêm

## 🚀 Truy cập ứng dụng

### Web Application:
- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001

### SQL Server:
- **Server**: localhost,1433
- **Database**: MiniMarketDB
- **User**: sa
- **Password**: Admin@123456

## 📋 Lệnh Docker hữu ích

### Kiểm tra trạng thái:
```bash
docker ps
docker-compose ps
```

### Xem logs:
```bash
# Logs của webshop
docker logs minimarket-webshop -f

# Logs của SQL Server
docker logs minimarket-sqlserver -f

# Logs của tất cả services
docker-compose logs -f
```

### Dừng containers:
```bash
docker-compose stop
```

### Khởi động lại containers:
```bash
docker-compose restart
```

### Dừng và xóa containers:
```bash
docker-compose down
```

### Rebuild và khởi động lại:
```bash
docker-compose down
docker-compose build --no-cache
docker-compose up -d
```

## 🧪 Test VNPay Payment

### Thông tin thẻ test:
- **Ngân hàng**: NCB
- **Số thẻ**: `9704198526191432198`
- **Tên chủ thẻ**: `NGUYEN VAN A`
- **Ngày phát hành**: `07/15`
- **Mật khẩu OTP**: `123456`

### Các bước test:
1. Truy cập http://localhost:5000
2. Đăng nhập với tài khoản test
3. Thêm sản phẩm vào giỏ hàng
4. Đi đến trang checkout
5. Chọn **VNPay** làm phương thức thanh toán
6. Điền thông tin giao hàng
7. Click **Đặt hàng**
8. Thanh toán trên VNPay với thẻ test
9. Kiểm tra kết quả

Xem chi tiết trong file `VNPAY_TEST_GUIDE.md`

## ⚠️ Lưu ý quan trọng

### IPN URL:
- IPN URL sẽ **KHÔNG hoạt động** với localhost
- VNPay server không thể truy cập localhost từ bên ngoài
- Để test IPN, cần sử dụng **ngrok** hoặc deploy lên server public

### Return URL:
- Return URL hoạt động tốt với localhost
- Redirect từ trình duyệt về ứng dụng local

### Hash Signature:
- Đã được cấu hình đúng với Hash Secret
- Nếu gặp lỗi "Sai chữ ký", kiểm tra lại cấu hình

## 🔧 Troubleshooting

### Ứng dụng không khởi động:
```bash
# Kiểm tra logs
docker logs minimarket-webshop

# Kiểm tra SQL Server
docker logs minimarket-sqlserver

# Rebuild và khởi động lại
docker-compose down
docker-compose build --no-cache
docker-compose up -d
```

### Database connection error:
```bash
# Kiểm tra SQL Server đang chạy
docker ps | grep sqlserver

# Kiểm tra logs
docker logs minimarket-sqlserver

# Restart SQL Server
docker restart minimarket-sqlserver
```

### Port đã được sử dụng:
```bash
# Kiểm tra port đang được sử dụng
netstat -ano | findstr :5000
netstat -ano | findstr :1433

# Dừng process đang sử dụng port
# Hoặc thay đổi port trong docker-compose.yml
```

## 📚 Tài liệu tham khảo

- **VNPay Documentation**: https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html
- **VNPay Demo Code**: https://sandbox.vnpayment.vn/apis/vnpay-demo/code-demo-tích-hợp
- **VNPay Merchant Admin**: https://sandbox.vnpayment.vn/merchantv2/

