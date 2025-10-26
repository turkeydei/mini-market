using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public static class SeedData
    {
        public static async Task SeedAsync(MiniMarketDbContext context)
        {
            // Seed Loai (Categories)
            if (!await context.Loais.AnyAsync())
            {
                var loais = new List<Loai>
                {
                    new Loai { TenLoai = "Điện Thoại", MoTa = "Các loại điện thoại di động" },
                    new Loai { TenLoai = "Laptop", MoTa = "Máy tính xách tay" },
                    new Loai { TenLoai = "Phụ Kiện", MoTa = "Phụ kiện điện tử" },
                    new Loai { TenLoai = "Đồ Gia Dụng", MoTa = "Đồ dùng trong gia đình" }
                };

                await context.Loais.AddRangeAsync(loais);
                await context.SaveChangesAsync();
            }

            // Seed TrangThai (Status)
            if (!await context.TrangThais.AnyAsync())
            {
                var trangThais = new List<TrangThai>
                {
                    new TrangThai { TenTrangThai = "Chờ Xác Nhận", MoTa = "Đơn hàng đang chờ xác nhận" },
                    new TrangThai { TenTrangThai = "Đã Xác Nhận", MoTa = "Đơn hàng đã được xác nhận" },
                    new TrangThai { TenTrangThai = "Đang Giao", MoTa = "Đơn hàng đang được giao" },
                    new TrangThai { TenTrangThai = "Đã Giao", MoTa = "Đơn hàng đã giao thành công" },
                    new TrangThai { TenTrangThai = "Đã Hủy", MoTa = "Đơn hàng đã bị hủy" }
                };

                await context.TrangThais.AddRangeAsync(trangThais);
                await context.SaveChangesAsync();
            }

            // Seed Users
            if (!await context.Users.AnyAsync())
            {
                var users = new List<User>
                {
                    new User 
                    { 
                        HoTen = "Admin User", 
                        Email = "admin@example.com", 
                        MatKhau = HashPassword("admin123"), 
                        VaiTro = 1,
                        DienThoai = "0123456789",
                        DiaChi = "123 Admin Street"
                    },
                    new User 
                    { 
                        HoTen = "Nguyễn Văn A", 
                        Email = "user1@example.com", 
                        MatKhau = HashPassword("user123"), 
                        VaiTro = 0,
                        DienThoai = "0987654321",
                        DiaChi = "456 User Street"
                    },
                    new User 
                    { 
                        HoTen = "Trần Thị B", 
                        Email = "user2@example.com", 
                        MatKhau = HashPassword("user123"), 
                        VaiTro = 0,
                        DienThoai = "0369258147",
                        DiaChi = "789 Customer Street"
                    }
                };

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }

            // Seed HangHoa (Products)
            if (!await context.HangHoas.AnyAsync())
            {
                var loais = await context.Loais.ToListAsync();
                var hangHoas = new List<HangHoa>
                {
                    new HangHoa
                    {
                        TenHH = "iPhone 15 Pro",
                        MaLoai = loais.First(l => l.TenLoai == "Điện Thoại").MaLoai,
                        DonGia = 29990000,
                        Hinh = "/images/iphone15pro.jpg",
                        NgaySX = DateTime.Now.AddDays(-30),
                        GiamGia = 5,
                        SoLanXem = 150,
                        MoTa = "iPhone 15 Pro với chip A17 Pro mạnh mẽ, camera 48MP chuyên nghiệp"
                    },
                    new HangHoa
                    {
                        TenHH = "Samsung Galaxy S24",
                        MaLoai = loais.First(l => l.TenLoai == "Điện Thoại").MaLoai,
                        DonGia = 24990000,
                        Hinh = "/images/galaxys24.jpg",
                        NgaySX = DateTime.Now.AddDays(-20),
                        GiamGia = 10,
                        SoLanXem = 120,
                        MoTa = "Samsung Galaxy S24 với màn hình Dynamic AMOLED 2X 6.2 inch"
                    },
                    new HangHoa
                    {
                        TenHH = "MacBook Pro M3",
                        MaLoai = loais.First(l => l.TenLoai == "Laptop").MaLoai,
                        DonGia = 45990000,
                        Hinh = "/images/macbookpro.jpg",
                        NgaySX = DateTime.Now.AddDays(-15),
                        GiamGia = 0,
                        SoLanXem = 80,
                        MoTa = "MacBook Pro 14 inch với chip M3, 16GB RAM, 512GB SSD"
                    },
                    new HangHoa
                    {
                        TenHH = "Dell XPS 13",
                        MaLoai = loais.First(l => l.TenLoai == "Laptop").MaLoai,
                        DonGia = 32990000,
                        Hinh = "/images/dellxps13.jpg",
                        NgaySX = DateTime.Now.AddDays(-10),
                        GiamGia = 8,
                        SoLanXem = 95,
                        MoTa = "Dell XPS 13 với Intel Core i7, 16GB RAM, 512GB SSD"
                    },
                    new HangHoa
                    {
                        TenHH = "AirPods Pro 2",
                        MaLoai = loais.First(l => l.TenLoai == "Phụ Kiện").MaLoai,
                        DonGia = 5990000,
                        Hinh = "/images/airpodspro.jpg",
                        NgaySX = DateTime.Now.AddDays(-25),
                        GiamGia = 15,
                        SoLanXem = 200,
                        MoTa = "AirPods Pro thế hệ 2 với chống ồn chủ động"
                    },
                    new HangHoa
                    {
                        TenHH = "Sạc Dự Phòng 20000mAh",
                        MaLoai = loais.First(l => l.TenLoai == "Phụ Kiện").MaLoai,
                        DonGia = 890000,
                        Hinh = "/images/powerbank.jpg",
                        NgaySX = DateTime.Now.AddDays(-5),
                        GiamGia = 20,
                        SoLanXem = 180,
                        MoTa = "Sạc dự phòng 20000mAh với sạc nhanh 18W"
                    },
                    new HangHoa
                    {
                        TenHH = "Máy Lọc Nước RO",
                        MaLoai = loais.First(l => l.TenLoai == "Đồ Gia Dụng").MaLoai,
                        DonGia = 3990000,
                        Hinh = "/images/waterfilter.jpg",
                        NgaySX = DateTime.Now.AddDays(-12),
                        GiamGia = 12,
                        SoLanXem = 60,
                        MoTa = "Máy lọc nước RO 5 cấp lọc, công suất 10L/h"
                    },
                    new HangHoa
                    {
                        TenHH = "Nồi Cơm Điện Tử",
                        MaLoai = loais.First(l => l.TenLoai == "Đồ Gia Dụng").MaLoai,
                        DonGia = 1290000,
                        Hinh = "/images/ricecooker.jpg",
                        NgaySX = DateTime.Now.AddDays(-8),
                        GiamGia = 25,
                        SoLanXem = 140,
                        MoTa = "Nồi cơm điện tử 1.8L với nồi chống dính"
                    }
                };

                await context.HangHoas.AddRangeAsync(hangHoas);
                await context.SaveChangesAsync();
            }
        }

        private static string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
