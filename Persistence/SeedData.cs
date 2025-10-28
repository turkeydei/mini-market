using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public static class SeedData
{
    public static async Task Initialize(MiniMarketDbContext context)
    {
        // Đảm bảo database đã được tạo
        await context.Database.EnsureCreatedAsync();

        // Seed Loai (Categories)
        if (!await context.Loais.AnyAsync())
        {
            var loais = new List<Loai>
            {
                new Loai
                {
                    TenLoai = "Điện tử",
                    MoTa = "Thiết bị điện tử, điện thoại, laptop, tablet"
                },
                new Loai
                {
                    TenLoai = "Thực phẩm",
                    MoTa = "Thực phẩm tươi sống, đồ ăn, đồ uống"
                },
                new Loai
                {
                    TenLoai = "Đồ gia dụng",
                    MoTa = "Đồ dùng gia đình, nội thất, đồ trang trí"
                },
                new Loai
                {
                    TenLoai = "Thời trang",
                    MoTa = "Quần áo, giày dép, phụ kiện thời trang"
                },
                new Loai
                {
                    TenLoai = "Sách",
                    MoTa = "Sách văn học, giáo khoa, truyện tranh"
                }
            };

            await context.Loais.AddRangeAsync(loais);
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Đã seed 5 danh mục sản phẩm");
        }

        // Seed Users
        if (!await context.Users.AnyAsync())
        {
            var users = new List<User>
            {
                new User
                {
                    HoTen = "Admin System",
                    Email = "admin@minimarket.com",
                    MatKhau = "Admin@123", // Trong thực tế cần hash password
                    DienThoai = "0123456789",
                    VaiTro = "Admin",
                    DiaChi = "123 Nguyễn Huệ, Q1, TP.HCM"
                },
                new User
                {
                    HoTen = "Nguyễn Văn An",
                    Email = "nguyenvana@example.com",
                    MatKhau = "Customer@123",
                    DienThoai = "0987654321",
                    VaiTro = "Customer",
                    DiaChi = "456 Lê Lợi, Q3, TP.HCM",
                    GioiTinh = 0,
                    NgaySinh = new DateTime(1990, 5, 15)
                },
                new User
                {
                    HoTen = "Trần Thị Bình",
                    Email = "tranthibinh@example.com",
                    MatKhau = "Customer@123",
                    DienThoai = "0912345678",
                    VaiTro = "Customer",
                    DiaChi = "789 Trần Hưng Đạo, Q5, TP.HCM",
                    GioiTinh = 1,
                    NgaySinh = new DateTime(1992, 8, 20)
                },
                new User
                {
                    HoTen = "Lê Văn Cường",
                    Email = "levancuong@example.com",
                    MatKhau = "Staff@123",
                    DienThoai = "0923456789",
                    VaiTro = "Staff",
                    DiaChi = "321 Võ Văn Tần, Q3, TP.HCM"
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Đã seed 4 người dùng (1 Admin, 2 Customer, 1 Staff)");
        }

        // Seed HangHoa (Products)
        if (!await context.HangHoas.AnyAsync())
        {
            var loaiDienTu = await context.Loais.FirstAsync(l => l.TenLoai == "Điện tử");
            var loaiThucPham = await context.Loais.FirstAsync(l => l.TenLoai == "Thực phẩm");
            var loaiGiaDung = await context.Loais.FirstAsync(l => l.TenLoai == "Đồ gia dụng");
            var loaiThoiTrang = await context.Loais.FirstAsync(l => l.TenLoai == "Thời trang");
            var loaiSach = await context.Loais.FirstAsync(l => l.TenLoai == "Sách");

            var hangHoas = new List<HangHoa>
            {
                // Điện tử
                new HangHoa
                {
                    TenHH = "iPhone 15 Pro Max 256GB",
                    MaLoai = loaiDienTu.MaLoai,
                    DonGia = 29990000,
                    SoLuongTon = 15,
                    GiamGia = 1000000,
                    Hinh = "iphone-15-pro-max.jpg",
                    MoTa = "iPhone 15 Pro Max với chip A17 Pro, camera 48MP"
                },
                new HangHoa
                {
                    TenHH = "Samsung Galaxy S24 Ultra",
                    MaLoai = loaiDienTu.MaLoai,
                    DonGia = 26990000,
                    SoLuongTon = 20,
                    GiamGia = 500000,
                    Hinh = "samsung-s24-ultra.jpg",
                    MoTa = "Samsung Galaxy S24 Ultra với bút S-Pen, màn hình 6.8 inch"
                },
                new HangHoa
                {
                    TenHH = "MacBook Air M3 13 inch",
                    MaLoai = loaiDienTu.MaLoai,
                    DonGia = 27990000,
                    SoLuongTon = 10,
                    GiamGia = 0,
                    Hinh = "macbook-air-m3.jpg",
                    MoTa = "MacBook Air M3 với chip Apple Silicon mới nhất"
                },

                // Thực phẩm
                new HangHoa
                {
                    TenHH = "Gạo ST25 - 5kg",
                    MaLoai = loaiThucPham.MaLoai,
                    DonGia = 150000,
                    SoLuongTon = 100,
                    GiamGia = 10000,
                    Hinh = "gao-st25.jpg",
                    MoTa = "Gạo ST25 thơm ngon, hạt dài"
                },
                new HangHoa
                {
                    TenHH = "Thịt bò Úc nhập khẩu - 500g",
                    MaLoai = loaiThucPham.MaLoai,
                    DonGia = 250000,
                    SoLuongTon = 50,
                    GiamGia = 0,
                    Hinh = "thit-bo-uc.jpg",
                    MoTa = "Thịt bò Úc tươi ngon, nhập khẩu chính hãng"
                },

                // Đồ gia dụng
                new HangHoa
                {
                    TenHH = "Nồi cơm điện Panasonic 1.8L",
                    MaLoai = loaiGiaDung.MaLoai,
                    DonGia = 1290000,
                    SoLuongTon = 30,
                    GiamGia = 100000,
                    Hinh = "noi-com-panasonic.jpg",
                    MoTa = "Nồi cơm điện Panasonic 1.8L, lòng nồi chống dính"
                },
                new HangHoa
                {
                    TenHH = "Máy hút bụi Xiaomi",
                    MaLoai = loaiGiaDung.MaLoai,
                    DonGia = 2490000,
                    SoLuongTon = 25,
                    GiamGia = 200000,
                    Hinh = "may-hut-bui-xiaomi.jpg",
                    MoTa = "Máy hút bụi cầm tay Xiaomi, pin 60 phút"
                },

                // Thời trang
                new HangHoa
                {
                    TenHH = "Áo thun nam Nike",
                    MaLoai = loaiThoiTrang.MaLoai,
                    DonGia = 450000,
                    SoLuongTon = 80,
                    GiamGia = 50000,
                    Hinh = "ao-thun-nike.jpg",
                    MoTa = "Áo thun nam Nike cotton cao cấp"
                },
                new HangHoa
                {
                    TenHH = "Giày thể thao Adidas",
                    MaLoai = loaiThoiTrang.MaLoai,
                    DonGia = 1890000,
                    SoLuongTon = 40,
                    GiamGia = 100000,
                    Hinh = "giay-adidas.jpg",
                    MoTa = "Giày thể thao Adidas Ultraboost"
                },

                // Sách
                new HangHoa
                {
                    TenHH = "Đắc Nhân Tâm",
                    MaLoai = loaiSach.MaLoai,
                    DonGia = 89000,
                    SoLuongTon = 200,
                    GiamGia = 0,
                    Hinh = "dac-nhan-tam.jpg",
                    MoTa = "Sách Đắc Nhân Tâm - Dale Carnegie"
                },
                new HangHoa
                {
                    TenHH = "Nhà Giả Kim",
                    MaLoai = loaiSach.MaLoai,
                    DonGia = 79000,
                    SoLuongTon = 150,
                    GiamGia = 10000,
                    Hinh = "nha-gia-kim.jpg",
                    MoTa = "Tiểu thuyết Nhà Giả Kim - Paulo Coelho"
                }
            };

            await context.HangHoas.AddRangeAsync(hangHoas);
            await context.SaveChangesAsync();
            Console.WriteLine("✓ Đã seed 12 sản phẩm");
        }

        // Seed đơn hàng mẫu
        if (!await context.HoaDons.AnyAsync())
        {
            var customer = await context.Users.FirstAsync(u => u.Email == "nguyenvana@example.com");
            var iPhone = await context.HangHoas.FirstAsync(h => h.TenHH.Contains("iPhone"));
            var gao = await context.HangHoas.FirstAsync(h => h.TenHH.Contains("Gạo"));

            // Đơn hàng 1: Đã hoàn thành
            var hoaDon1 = new HoaDon
            {
                MaUser = customer.MaUser,
                NgayDat = DateTime.Now.AddDays(-5),
                NgayGiao = DateTime.Now.AddDays(-2),
                DiaChiGiao = customer.DiaChi!,
                SoDienThoai = customer.DienThoai!,
                PhiVanChuyen = 30000,
                TongTien = iPhone.DonGia - iPhone.GiamGia + 30000,
                Status = "Completed",
                GhiChu = "Giao hàng buổi chiều"
            };

            context.HoaDons.Add(hoaDon1);
            await context.SaveChangesAsync();

            var chiTiet1 = new ChiTietHD
            {
                MaHD = hoaDon1.MaHD,
                MaHH = iPhone.MaHH,
                DonGia = iPhone.DonGia,
                SoLuong = 1,
                GiamGia = iPhone.GiamGia
            };

            context.ChiTietHDs.Add(chiTiet1);

            var payment1 = new PaymentTransaction
            {
                MaHD = hoaDon1.MaHD,
                SoTien = hoaDon1.TongTien,
                Status = "Completed",
                PhuongThucTT = "Bank Transfer",
                NgayTao = hoaDon1.NgayDat,
                NgayCapNhat = DateTime.Now.AddDays(-2),
                GhiChu = "Đã thanh toán qua chuyển khoản"
            };

            context.PaymentTransactions.Add(payment1);
            await context.SaveChangesAsync();

            // Đơn hàng 2: Đang xử lý
            var hoaDon2 = new HoaDon
            {
                MaUser = customer.MaUser,
                NgayDat = DateTime.Now.AddHours(-2),
                DiaChiGiao = customer.DiaChi!,
                SoDienThoai = customer.DienThoai!,
                PhiVanChuyen = 20000,
                TongTien = (gao.DonGia - gao.GiamGia) * 2 + 20000,
                Status = "Processing",
                GhiChu = "Giao sáng sớm"
            };

            context.HoaDons.Add(hoaDon2);
            await context.SaveChangesAsync();

            var chiTiet2 = new ChiTietHD
            {
                MaHD = hoaDon2.MaHD,
                MaHH = gao.MaHH,
                DonGia = gao.DonGia,
                SoLuong = 2,
                GiamGia = gao.GiamGia * 2
            };

            context.ChiTietHDs.Add(chiTiet2);

            var payment2 = new PaymentTransaction
            {
                MaHD = hoaDon2.MaHD,
                SoTien = hoaDon2.TongTien,
                Status = "Completed",
                PhuongThucTT = "COD",
                NgayTao = hoaDon2.NgayDat,
                NgayCapNhat = DateTime.Now.AddHours(-1),
                GhiChu = "Thanh toán khi nhận hàng"
            };

            context.PaymentTransactions.Add(payment2);
            await context.SaveChangesAsync();

            Console.WriteLine("✓ Đã seed 2 đơn hàng mẫu");
        }

        Console.WriteLine("\n✅ Seed data hoàn tất!");
    }
}

