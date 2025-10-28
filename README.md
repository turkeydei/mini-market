# Mini Market - E-Commerce System

## Cấu trúc dự án đã được thu gọn

Dự án sử dụng 5 DTO chính:

1. **Loai** - Danh mục sản phẩm
2. **HangHoa** - Sản phẩm
3. **HoaDon** - Đơn hàng
4. **User** - Người dùng (Khách hàng/Nhân viên)
5. **PaymentTransaction** - Giao dịch thanh toán

## Luồng hoạt động

```
Khách → Đặt hàng → /checkout
          ↓
Tạo đơn hàng (HoaDon, ChiTietHD)
          ↓
Tạo bản ghi PaymentTransaction (status = Pending)
          ↓
Xử lý thanh toán
          ↓
Cập nhật Status (Pending → Completed/Failed)
```

## Cấu trúc Database

### User

- MaUser (PK)
- HoTen, Email, MatKhau
- DienThoai, DiaChi
- VaiTro (Customer/Admin/Staff)
- GioiTinh, NgaySinh

### Loai (Category)

- MaLoai (PK)
- TenLoai
- MoTa

### HangHoa (Product)

- MaHH (PK)
- TenHH
- MaLoai (FK → Loai)
- DonGia, GiamGia
- SoLuongTon
- Hinh, MoTa
- SoLanXem

### HoaDon (Order)

- MaHD (PK)
- MaUser (FK → User)
- NgayDat, NgayGiao
- DiaChiGiao, SoDienThoai
- TongTien, PhiVanChuyen
- Status (Pending/Processing/Completed/Cancelled)
- GhiChu

### ChiTietHD (Order Details)

- MaCT (PK)
- MaHD (FK → HoaDon)
- MaHH (FK → HangHoa)
- DonGia, SoLuong, GiamGia
- ThanhTien (computed)

### PaymentTransaction

- MaGD (PK)
- MaHD (FK → HoaDon)
- SoTien
- Status (Pending/Completed/Failed/Cancelled)
- PhuongThucTT (COD/Bank Transfer/E-Wallet)
- NgayTao, NgayCapNhat
- GhiChu

## Hướng dẫn cài đặt

### 1. Cài đặt dependencies

```bash
dotnet restore
```

### 2. Tạo migration mới

```bash
cd Persistence
dotnet ef migrations add SimplifiedSchema --startup-project ../WebShop
```

### 3. Cập nhật database

```bash
dotnet ef database update --startup-project ../WebShop
```

### 4. Chạy ứng dụng

```bash
cd ../WebShop
dotnet run
```

## Cấu trúc dự án

```
mini-market/
├── Domain/              # Entities
│   └── Entities/
│       ├── User.cs
│       ├── Loai.cs
│       ├── HangHoa.cs
│       ├── HoaDon.cs
│       ├── ChiTietHD.cs
│       └── PaymentTransaction.cs
├── Application/         # Business Logic
├── Persistence/         # Database Context & Migrations
│   ├── MiniMarketDbContext.cs
│   └── Migrations/
└── WebShop/            # Web Application
    ├── Controllers/
    ├── Views/
    └── wwwroot/
```

## API Endpoints (Ví dụ)

### Checkout Flow

```
POST /api/checkout/create-order
{
    "maUser": 1,
    "diaChiGiao": "123 Main St",
    "soDienThoai": "0123456789",
    "items": [
        {
            "maHH": 1,
            "soLuong": 2
        }
    ]
}

Response:
{
    "maHD": 1,
    "tongTien": 100000,
    "status": "Pending",
    "paymentTransaction": {
        "maGD": 1,
        "status": "Pending"
    }
}
```

## Trạng thái

### HoaDon Status

- **Pending**: Chờ xác nhận
- **Processing**: Đang xử lý
- **Completed**: Hoàn thành
- **Cancelled**: Đã hủy

### PaymentTransaction Status

- **Pending**: Chờ thanh toán
- **Completed**: Đã thanh toán
- **Failed**: Thanh toán thất bại
- **Cancelled**: Đã hủy

## License

MIT
