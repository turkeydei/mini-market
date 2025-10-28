# 🚀 HƯỚNG DẪN NHANH - MINI MARKET

## ⚡ Chạy nhanh (Quick Start)

### Yêu cầu

- ✅ .NET SDK 6.0 hoặc cao hơn
- ✅ SQL Server (LocalDB hoặc SQL Server Express)
- ✅ EF Core Tools (sẽ tự động cài đặt)

---

## 📦 Cài đặt .NET SDK

### macOS

```bash
brew install --cask dotnet-sdk
```

Hoặc tải từ: https://dotnet.microsoft.com/download

### Windows

Tải và cài đặt từ: https://dotnet.microsoft.com/download

---

## 🎯 Chạy Project (3 lệnh)

### Trên macOS/Linux:

```bash
# 1. Cấp quyền thực thi cho scripts
chmod +x setup.sh run.sh clean.sh

# 2. Chạy setup (chỉ cần chạy 1 lần)
./setup.sh

# 3. Chạy ứng dụng
./run.sh
```

### Trên Windows (PowerShell):

```powershell
# 1. Cho phép chạy scripts (chạy PowerShell as Administrator)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# 2. Chạy setup (chỉ cần chạy 1 lần)
.\setup.ps1

# 3. Chạy ứng dụng
.\run.ps1
```

---

## 🌐 Truy cập ứng dụng

Sau khi chạy thành công, mở trình duyệt và truy cập:

- **HTTPS**: https://localhost:5001
- **HTTP**: http://localhost:5000

---

## 📊 Dữ liệu mẫu

Ứng dụng sẽ tự động tạo dữ liệu mẫu khi khởi động lần đầu:

### 👥 Users (4 tài khoản)

| Email                   | Password     | Vai trò  |
| ----------------------- | ------------ | -------- |
| admin@minimarket.com    | Admin@123    | Admin    |
| nguyenvana@example.com  | Customer@123 | Customer |
| tranthibinh@example.com | Customer@123 | Customer |
| levancuong@example.com  | Staff@123    | Staff    |

### 📦 Sản phẩm (12 sản phẩm)

- **Điện tử**: iPhone 15 Pro Max, Samsung Galaxy S24, MacBook Air M3
- **Thực phẩm**: Gạo ST25, Thịt bò Úc
- **Đồ gia dụng**: Nồi cơm điện, Máy hút bụi
- **Thời trang**: Áo thun Nike, Giày Adidas
- **Sách**: Đắc Nhân Tâm, Nhà Giả Kim

### 🛒 Đơn hàng mẫu (2 đơn)

- 1 đơn đã hoàn thành
- 1 đơn đang xử lý

---

## 🛠️ Commands hữu ích

### Chỉ chạy migrations (không seed data)

```bash
cd Persistence
dotnet ef migrations add MigrationName --startup-project ../WebShop
dotnet ef database update --startup-project ../WebShop
```

### Xóa database và bắt đầu lại

```bash
# macOS/Linux
./clean.sh
./setup.sh

# Windows
.\clean.ps1  # (Cần tạo file này nếu chưa có)
.\setup.ps1
```

### Build project

```bash
dotnet build
```

### Xem migrations

```bash
cd Persistence
dotnet ef migrations list --startup-project ../WebShop
```

### Xem SQL script

```bash
cd Persistence
dotnet ef migrations script --startup-project ../WebShop
```

---

## 🐛 Troubleshooting

### Lỗi: "dotnet command not found"

**Giải pháp**: Cài đặt .NET SDK và khởi động lại terminal

### Lỗi: "Cannot connect to SQL Server"

**Giải pháp**:

1. Kiểm tra SQL Server đang chạy
2. Cập nhật connection string trong `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=MiniMarketDB;..."
  }
}
```

### Lỗi: "EF Core Tools not found"

**Giải pháp**:

```bash
dotnet tool install --global dotnet-ef
```

### Lỗi khi chạy script PowerShell

**Giải pháp**: Chạy PowerShell as Administrator và thực hiện:

```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Database đã tồn tại

**Giải pháp**: Xóa database cũ:

```bash
cd Persistence
dotnet ef database drop --startup-project ../WebShop --force
```

---

## 🔄 Luồng Checkout

```
Khách hàng → Chọn sản phẩm → Thêm vào giỏ
              ↓
         Trang Checkout (/Checkout)
              ↓
    Điền thông tin giao hàng
              ↓
      Chọn phương thức thanh toán
        (COD / Bank Transfer / E-Wallet)
              ↓
         Nhấn "Đặt Hàng"
              ↓
    POST /Checkout/CreateOrder
              ↓
  ┌──────────────────────────┐
  │ 1. Tạo HoaDon            │
  │    Status = "Pending"    │
  ├──────────────────────────┤
  │ 2. Tạo ChiTietHD         │
  │    (Chi tiết sản phẩm)   │
  ├──────────────────────────┤
  │ 3. Cập nhật SoLuongTon   │
  ├──────────────────────────┤
  │ 4. Tạo PaymentTransaction│
  │    Status = "Pending"    │
  └──────────────────────────┘
              ↓
    Trả về thông tin đơn hàng
              ↓
      Chuyển đến trang xác nhận
```

---

## 📁 Cấu trúc Project

```
mini-market/
├── Domain/              # Entities (5 DTO chính)
│   └── Entities/
│       ├── User.cs
│       ├── Loai.cs
│       ├── HangHoa.cs
│       ├── HoaDon.cs
│       ├── ChiTietHD.cs
│       └── PaymentTransaction.cs
├── Persistence/         # Database Context
│   ├── MiniMarketDbContext.cs
│   ├── SeedData.cs
│   └── Migrations/
├── WebShop/            # Web Application
│   ├── Controllers/
│   │   ├── HomeController.cs
│   │   └── CheckoutController.cs
│   ├── Views/
│   │   ├── Home/
│   │   └── Checkout/
│   └── wwwroot/
├── setup.sh            # Setup script (macOS/Linux)
├── setup.ps1           # Setup script (Windows)
├── run.sh              # Run script (macOS/Linux)
├── run.ps1             # Run script (Windows)
└── README.md           # Tài liệu chi tiết
```

---

## 🎯 API Endpoints

### Checkout

- `GET /Checkout` - Trang checkout
- `POST /Checkout/CreateOrder` - Tạo đơn hàng
- `POST /Checkout/UpdatePaymentStatus` - Cập nhật trạng thái thanh toán
- `GET /Checkout/GetOrderStatus/{maHD}` - Xem chi tiết đơn hàng

### Home

- `GET /` - Trang chủ
- `GET /Home/Privacy` - Trang privacy

---

## 📞 Hỗ trợ

Nếu gặp vấn đề, hãy kiểm tra:

1. ✅ .NET SDK đã được cài đặt: `dotnet --version`
2. ✅ SQL Server đang chạy
3. ✅ Connection string đúng trong `appsettings.json`
4. ✅ Port 5000/5001 không bị chiếm

---

## 🎉 Hoàn tất!

Chúc bạn phát triển thành công! 🚀
