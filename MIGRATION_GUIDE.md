# Hướng dẫn tạo Migration

## Yêu cầu

- .NET SDK 6.0 trở lên
- Entity Framework Core Tools

## Cài đặt EF Core Tools

```bash
dotnet tool install --global dotnet-ef
# Hoặc cập nhật nếu đã cài
dotnet tool update --global dotnet-ef
```

## Tạo Migration mới

### Bước 1: Xóa Database cũ (nếu có)

```bash
cd Persistence
dotnet ef database drop --startup-project ../WebShop
```

### Bước 2: Tạo Migration với schema mới

```bash
dotnet ef migrations add SimplifiedSchema --startup-project ../WebShop
```

### Bước 3: Cập nhật Database

```bash
dotnet ef database update --startup-project ../WebShop
```

## Kiểm tra Connection String

Đảm bảo file `appsettings.Development.json` trong WebShop có connection string đúng:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MiniMarket;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Cấu trúc Database sau khi Migration

### Tables được tạo:

1. **User** - Người dùng (khách hàng/admin/staff)
2. **Loai** - Danh mục sản phẩm
3. **HangHoa** - Sản phẩm
4. **HoaDon** - Đơn hàng
5. **ChiTietHD** - Chi tiết đơn hàng
6. **PaymentTransaction** - Giao dịch thanh toán

### Relationships:

- Loai (1) → (N) HangHoa
- User (1) → (N) HoaDon
- HangHoa (1) → (N) ChiTietHD
- HoaDon (1) → (N) ChiTietHD
- HoaDon (1) → (1) PaymentTransaction

## Seed Data (Optional)

Sau khi tạo database, bạn có thể thêm dữ liệu mẫu:

```csharp
// Thêm vào Program.cs hoặc tạo file SeedData.cs

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MiniMarketDbContext>();

    // Seed Loai
    if (!context.Loais.Any())
    {
        context.Loais.AddRange(
            new Loai { TenLoai = "Điện tử", MoTa = "Thiết bị điện tử" },
            new Loai { TenLoai = "Thực phẩm", MoTa = "Thực phẩm tươi sống" },
            new Loai { TenLoai = "Đồ gia dụng", MoTa = "Đồ dùng gia đình" }
        );
        context.SaveChanges();
    }

    // Seed User
    if (!context.Users.Any())
    {
        context.Users.AddRange(
            new User
            {
                HoTen = "Admin",
                Email = "admin@minimarket.com",
                MatKhau = "admin123",
                VaiTro = "Admin",
                DienThoai = "0123456789"
            },
            new User
            {
                HoTen = "Nguyen Van A",
                Email = "customer@example.com",
                MatKhau = "customer123",
                VaiTro = "Customer",
                DienThoai = "0987654321"
            }
        );
        context.SaveChanges();
    }

    // Seed HangHoa
    if (!context.HangHoas.Any())
    {
        var loaiDienTu = context.Loais.First(l => l.TenLoai == "Điện tử");
        context.HangHoas.AddRange(
            new HangHoa
            {
                TenHH = "iPhone 15",
                MaLoai = loaiDienTu.MaLoai,
                DonGia = 25000000,
                SoLuongTon = 10,
                GiamGia = 0,
                MoTa = "iPhone 15 128GB"
            },
            new HangHoa
            {
                TenHH = "Samsung Galaxy S24",
                MaLoai = loaiDienTu.MaLoai,
                DonGia = 22000000,
                SoLuongTon = 15,
                GiamGia = 0,
                MoTa = "Samsung Galaxy S24 256GB"
            }
        );
        context.SaveChanges();
    }
}
```

## Troubleshooting

### Lỗi: "Unable to create an object of type 'MiniMarketDbContext'"

**Giải pháp**: Đảm bảo connection string trong appsettings.json đúng và SQL Server đang chạy.

### Lỗi: "The name 'dotnet' is not recognized"

**Giải pháp**: Cài đặt .NET SDK từ https://dotnet.microsoft.com/download

### Lỗi: "Could not execute because the specified command or file was not found"

**Giải pháp**: Cài đặt EF Core Tools:

```bash
dotnet tool install --global dotnet-ef
```

## Kiểm tra Migration

```bash
# Xem danh sách migrations
dotnet ef migrations list --startup-project ../WebShop

# Xem SQL script sẽ được thực thi
dotnet ef migrations script --startup-project ../WebShop
```
