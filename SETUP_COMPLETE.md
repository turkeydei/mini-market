# ✅ SETUP HOÀN TẤT - HƯỚNG DẪN CHẠY PROJECT

## 🎉 Các file đã được chuẩn bị

Tôi đã tạo và cấu hình đầy đủ các file cho bạn:

### ✅ Database & Entities

- ✅ 5 DTO chính: `User`, `Loai`, `HangHoa`, `HoaDon`, `PaymentTransaction`
- ✅ DTO phụ: `ChiTietHD` (Chi tiết đơn hàng)
- ✅ `MiniMarketDbContext` - Database context
- ✅ `SeedData.cs` - Dữ liệu mẫu (12 sản phẩm, 4 users, 2 đơn hàng)

### ✅ Controllers & Views

- ✅ `CheckoutController.cs` - Xử lý checkout với 3 endpoints
- ✅ `Views/Checkout/Index.cshtml` - Trang checkout với form thanh toán

### ✅ Configuration

- ✅ `appsettings.json` - Connection string đã được cấu hình
- ✅ `Program.cs` - Tự động migrate và seed data khi khởi động

### ✅ Scripts & Documentation

- ✅ `setup.sh` (macOS/Linux) - Script setup tự động
- ✅ `setup.ps1` (Windows) - Script setup tự động cho Windows
- ✅ `run.sh` / `run.ps1` - Script chạy ứng dụng
- ✅ `clean.sh` - Script xóa database và migrations
- ✅ `QUICKSTART.md` - Hướng dẫn nhanh
- ✅ `INSTALL_DOTNET.md` - Hướng dẫn cài .NET SDK
- ✅ `MIGRATION_GUIDE.md` - Hướng dẫn chi tiết về migrations
- ✅ `README.md` - Tài liệu đầy đủ

---

## 🚦 BƯỚC TIẾP THEO

### ⚠️ Quan trọng: Cài đặt .NET SDK

Máy của bạn **chưa có .NET SDK**. Đây là yêu cầu bắt buộc để chạy project.

**Chọn một trong hai cách:**

#### Cách 1: Homebrew (Nhanh nhất - Khuyến nghị)

```bash
brew install --cask dotnet-sdk
```

#### Cách 2: Download từ Microsoft

1. Truy cập: https://dotnet.microsoft.com/download
2. Download **.NET SDK 8.0** (hoặc 6.0+)
3. Cài đặt và khởi động lại Terminal

**Kiểm tra sau khi cài:**

```bash
dotnet --version
# Kết quả mong đợi: 8.0.xxx hoặc 6.0.xxx
```

📖 **Chi tiết đầy đủ**: Xem [INSTALL_DOTNET.md](INSTALL_DOTNET.md)

---

## 🔧 Cài đặt SQL Server

Bạn cũng cần SQL Server để lưu trữ dữ liệu.

### Option 1: Docker (Đơn giản nhất cho macOS)

```bash
# Pull image
docker pull mcr.microsoft.com/mssql/server:2022-latest

# Run container
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Admin@123456" \
   -p 1433:1433 --name sqlserver \
   -d mcr.microsoft.com/mssql/server:2022-latest

# Sau đó cập nhật connection string trong appsettings.json:
# "Server=localhost,1433;Database=MiniMarketDB;User Id=sa;Password=Admin@123456;TrustServerCertificate=True"
```

### Option 2: Azure SQL Database (Cloud - Free Tier)

Tạo free tier tại: https://azure.microsoft.com/free/sql-database/

📖 **Chi tiết**: Xem phần SQL Server trong [INSTALL_DOTNET.md](INSTALL_DOTNET.md)

---

## 🚀 CHẠY PROJECT (3 Bước)

Sau khi đã cài .NET SDK và SQL Server:

### Bước 1: Cài đặt EF Core Tools

```bash
dotnet tool install --global dotnet-ef
```

### Bước 2: Chạy Setup

```bash
# macOS/Linux
./setup.sh

# Windows PowerShell
.\setup.ps1
```

Script sẽ tự động:

- ✅ Restore dependencies
- ✅ Tạo migrations
- ✅ Tạo database
- ✅ Build project

### Bước 3: Chạy ứng dụng

```bash
# macOS/Linux
./run.sh

# Windows PowerShell
.\run.ps1

# Hoặc chạy trực tiếp
cd WebShop
dotnet run
```

---

## 🌐 Truy cập ứng dụng

Sau khi khởi động thành công, mở trình duyệt:

- **HTTPS**: https://localhost:5001
- **HTTP**: http://localhost:5000

Bạn sẽ thấy dữ liệu mẫu đã được tự động seed:

- 📦 12 sản phẩm
- 👥 4 người dùng
- 🛒 2 đơn hàng mẫu

---

## 🔐 Tài khoản mẫu

### Admin

- **Email**: admin@minimarket.com
- **Password**: Admin@123

### Khách hàng

- **Email**: nguyenvana@example.com
- **Password**: Customer@123

---

## 📊 Kiểm tra Database

### Xem các table đã được tạo

```bash
cd Persistence
dotnet ef migrations list --startup-project ../WebShop
```

### Xem SQL script

```bash
dotnet ef migrations script --startup-project ../WebShop > migration.sql
```

---

## 🛠️ Commands hữu ích

```bash
# Rebuild database từ đầu
./clean.sh        # Xóa database và migrations
./setup.sh        # Setup lại từ đầu

# Xem logs khi chạy
cd WebShop
dotnet run --verbosity detailed

# Build release
dotnet build --configuration Release

# Publish
dotnet publish --configuration Release --output ./publish
```

---

## 📁 Cấu trúc Project

```
mini-market/
├── 📂 Domain/              # Entities
│   └── Entities/
│       ├── User.cs
│       ├── Loai.cs
│       ├── HangHoa.cs
│       ├── HoaDon.cs
│       ├── ChiTietHD.cs
│       └── PaymentTransaction.cs
│
├── 📂 Persistence/         # Database
│   ├── MiniMarketDbContext.cs
│   ├── SeedData.cs
│   └── Migrations/         # (sẽ được tạo khi chạy setup.sh)
│
├── 📂 WebShop/            # Web App
│   ├── Controllers/
│   │   ├── HomeController.cs
│   │   └── CheckoutController.cs
│   ├── Views/
│   │   ├── Home/
│   │   └── Checkout/
│   └── wwwroot/
│
├── 📜 setup.sh            # Setup script
├── 📜 run.sh              # Run script
├── 📜 clean.sh            # Clean script
├── 📄 QUICKSTART.md       # Quick guide
├── 📄 INSTALL_DOTNET.md   # .NET installation
└── 📄 README.md           # Full documentation
```

---

## 🎯 Luồng Checkout đã được implement

```
👤 Khách hàng
    ↓
📋 Chọn sản phẩm → Thêm vào giỏ
    ↓
🛒 Trang Checkout (/Checkout)
    ↓
📝 Điền thông tin:
   - Họ tên, số điện thoại
   - Địa chỉ giao hàng
   - Phương thức thanh toán (COD/Bank/E-Wallet)
    ↓
🔘 Nhấn "Đặt Hàng"
    ↓
⚙️ POST /Checkout/CreateOrder
    ↓
┌───────────────────────────┐
│ 1️⃣ Tạo HoaDon           │
│    Status = "Pending"     │
├───────────────────────────┤
│ 2️⃣ Tạo ChiTietHD        │
│    (Chi tiết sản phẩm)    │
├───────────────────────────┤
│ 3️⃣ Cập nhật SoLuongTon  │
├───────────────────────────┤
│ 4️⃣ Tạo PaymentTransaction│
│    Status = "Pending"     │
└───────────────────────────┘
    ↓
✅ Trả về thông tin đơn hàng
    ↓
📄 Chuyển đến trang xác nhận
```

---

## 🎯 API Endpoints đã sẵn sàng

### Checkout

| Method | Endpoint                          | Mô tả                          |
| ------ | --------------------------------- | ------------------------------ |
| GET    | `/Checkout`                       | Trang checkout                 |
| POST   | `/Checkout/CreateOrder`           | Tạo đơn hàng mới               |
| POST   | `/Checkout/UpdatePaymentStatus`   | Cập nhật trạng thái thanh toán |
| GET    | `/Checkout/GetOrderStatus/{maHD}` | Xem chi tiết đơn hàng          |

### Home

| Method | Endpoint        | Mô tả          |
| ------ | --------------- | -------------- |
| GET    | `/`             | Trang chủ      |
| GET    | `/Home/Privacy` | Privacy policy |

---

## 🐛 Troubleshooting

### Lỗi: "dotnet command not found"

➡️ Cài đặt .NET SDK (xem [INSTALL_DOTNET.md](INSTALL_DOTNET.md))

### Lỗi: "Cannot connect to SQL Server"

➡️ Kiểm tra SQL Server đang chạy:

```bash
# Docker
docker ps | grep sqlserver

# Start nếu stopped
docker start sqlserver
```

### Lỗi: "EF Core Tools not found"

➡️ Cài đặt:

```bash
dotnet tool install --global dotnet-ef
```

### Lỗi: Database already exists

➡️ Xóa và tạo lại:

```bash
./clean.sh
./setup.sh
```

---

## 📚 Tài liệu tham khảo

- 🚀 [QUICKSTART.md](QUICKSTART.md) - Hướng dẫn nhanh
- 📥 [INSTALL_DOTNET.md](INSTALL_DOTNET.md) - Cài đặt .NET SDK & SQL Server
- 🔄 [MIGRATION_GUIDE.md](MIGRATION_GUIDE.md) - Chi tiết về EF Migrations
- 📖 [README.md](README.md) - Tài liệu đầy đủ

---

## 🎉 Sẵn sàng để chạy!

Tất cả đã được chuẩn bị sẵn sàng. Bạn chỉ cần:

1. ✅ Cài .NET SDK
2. ✅ Cài SQL Server (Docker hoặc local)
3. ✅ Chạy `./setup.sh`
4. ✅ Chạy `./run.sh`

**Happy Coding! 🚀**

---

## 💬 Câu hỏi thường gặp

**Q: Tôi có thể dùng database khác thay vì SQL Server không?**
A: Có! Chỉ cần thay đổi provider trong `Program.cs`:

- MySQL: `UseMySql()`
- PostgreSQL: `UseNpgsql()`
- SQLite: `UseSqlite()`

**Q: Dữ liệu mẫu có được tự động tạo không?**
A: Có! Khi bạn chạy ứng dụng lần đầu, nó sẽ tự động:

- Chạy migrations (tạo tables)
- Seed dữ liệu mẫu (products, users, orders)

**Q: Tôi muốn thay đổi connection string?**
A: Sửa file `WebShop/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION_STRING_HERE"
  }
}
```

**Q: Làm sao để deploy lên production?**
A:

```bash
cd WebShop
dotnet publish -c Release -o ./publish
```

Sau đó copy folder `publish` lên server.

---

Chúc bạn thành công! 🎯
