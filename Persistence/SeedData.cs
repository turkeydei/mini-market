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
                 new HangHoa { 
                     TenHH = "Samsung Galaxy Buds2",
                     MaLoai = loaiDienTu.MaLoai,
                     DonGia = 2500000,
                     SoLuongTon = 40,
                     GiamGia = 200000,
                     Hinh = "samsung-buds2.jpg",
                     MoTa = "Tai nghe không dây Samsung Galaxy Buds2"
                 },
                 new HangHoa { 
                     TenHH = "Apple Watch Series 9", 
                     MaLoai = loaiDienTu.MaLoai,
                     DonGia = 10990000,
                     SoLuongTon = 15,
                     GiamGia = 500000,
                     Hinh = "apple-watch-s9.jpg",
                     MoTa = "Apple Watch Series 9, GPS, 41mm"
                 },
                 new HangHoa { 
                     TenHH = "Laptop Dell XPS 15", 
                     MaLoai = loaiDienTu.MaLoai, 
                     DonGia = 35990000, 
                     SoLuongTon = 8,
                     GiamGia = 1000000,
                     Hinh = "dell-xps-15.jpg", 
                     MoTa = "Laptop Dell XPS 15, Intel i7, RAM 16GB" 
                 },

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
                 new HangHoa { 
                     TenHH = "Tai nghe AirPods Pro 2",
                     MaLoai = loaiDienTu.MaLoai, DonGia = 5990000, 
                     SoLuongTon = 35, GiamGia = 300000, 
                     Hinh = "airpods-pro-2.jpg", 
                     MoTa = "Tai nghe AirPods Pro 2 chống ồn chủ động" 
                 },
                new HangHoa {
                    TenHH = "TV Samsung 55 inch 4K",
                    MaLoai = loaiDienTu.MaLoai,
                    DonGia = 12990000,
                    SoLuongTon = 12,
                    GiamGia = 1000000,
                    Hinh = "tivi-samsung-55.jpg",
                    MoTa = "Smart TV Samsung 55 inch độ phân giải 4K" 
                },
                new HangHoa {
                    TenHH = "iPad Pro M4 11 inch",
                    MaLoai = loaiDienTu.MaLoai,
                    DonGia = 23990000, 
                    SoLuongTon = 18, 
                    GiamGia = 500000, 
                    Hinh = "ipad-pro-m4.jpg", 
                    MoTa = "iPad Pro M4 màn 11 inch, chip M4, Face ID" 
                },
                new HangHoa { 
                    TenHH = "Loa Bluetooth JBL Charge 5",
                    MaLoai = loaiDienTu.MaLoai,
                    DonGia = 3890000, SoLuongTon = 28,
                    GiamGia = 200000, 
                    Hinh = "jbl-charge-5.jpg",
                    MoTa = "Loa JBL Charge 5 chống nước IP67" 
                },
                new HangHoa {
                    TenHH = "Chuột Logitech MX Master 3S",
                    MaLoai = loaiDienTu.MaLoai,
                    DonGia = 2190000,
                    SoLuongTon = 45,
                    GiamGia = 100000,
                    Hinh = "mx-master-3s.jpg", 
                    MoTa = "Chuột không dây cao cấp Logitech"
                },
                new HangHoa {
                    TenHH = "Màn hình LG 27 inch 2K 144Hz",
                    MaLoai = loaiDienTu.MaLoai,
                    DonGia = 6990000,
                    SoLuongTon = 20,
                    GiamGia = 500000, 
                    Hinh = "lg-27-2k-144hz.jpg",
                    MoTa = "Màn hình LG 27inch 2K dành cho gaming" 
                },

                // Thực phẩm
                 new HangHoa {
                     TenHH = "Sữa tươi Vinamilk 1L",
                     MaLoai = loaiThucPham.MaLoai, 
                     DonGia = 42000, 
                     SoLuongTon = 200,
                     GiamGia = 2000, 
                     Hinh = "vinamilk-1l.jpg",
                     MoTa = "Sữa tươi tiệt trùng Vinamilk 1L" 
                 },
                new HangHoa {
                    TenHH = "Bánh mì sandwich",
                    MaLoai = loaiThucPham.MaLoai, 
                    DonGia = 15000, 
                    SoLuongTon = 180,
                    GiamGia = 0,
                    Hinh = "banh-mi-sandwich.jpg", 
                    MoTa = "Bánh mì sandwich mềm, thơm ngon"
                },
                new HangHoa { 
                    TenHH = "Trà Lipton túi lọc 100g",
                    MaLoai = loaiThucPham.MaLoai, 
                    DonGia = 35000,
                    SoLuongTon = 120, 
                    GiamGia = 5000, 
                    Hinh = "tra-lipton.jpg",
                    MoTa = "Trà túi lọc Lipton 100g" 
                },
                new HangHoa {
                    TenHH = "Nước khoáng Lavie 1.5L (Thùng 12 chai)",
                    MaLoai = loaiThucPham.MaLoai,
                    DonGia = 79000, 
                    SoLuongTon = 150, 
                    GiamGia = 5000, 
                    Hinh = "lavie-1-5l.jpg", 
                    MoTa = "Nước khoáng Lavie tinh khiết"
                },
                new HangHoa { 
                    TenHH = "Trứng gà CP hộp 10 quả",
                    MaLoai = loaiThucPham.MaLoai,
                    DonGia = 38000,
                    SoLuongTon = 200, 
                    GiamGia = 2000,
                    Hinh = "trung-ga-cp.jpg",
                    MoTa = "Trứng gà sạch CP, giàu dinh dưỡng"
                },
                new HangHoa {
                    TenHH = "Dầu ăn Neptune 1L",
                    MaLoai = loaiThucPham.MaLoai,
                    DonGia = 52000,
                    SoLuongTon = 160,
                    GiamGia = 3000, 
                    Hinh = "dau-neptune.jpg",
                    MoTa = "Dầu ăn Neptune nguyên chất" 
                },
                new HangHoa { 
                    TenHH = "Cà phê Trung Nguyên (500g)",
                    MaLoai = loaiThucPham.MaLoai,
                    DonGia = 89000,
                    SoLuongTon = 120,
                    GiamGia = 5000,
                    Hinh = "ca-phe-trung-nguyen.jpg",
                    MoTa = "Cà phê Trung Nguyên rang xay" 
                },
                new HangHoa { 
                    TenHH = "Mì Hảo Hảo Tôm Chua Cay - Thùng 30 gói",
                    MaLoai = loaiThucPham.MaLoai, 
                    DonGia = 125000,
                    SoLuongTon = 90, 
                    GiamGia = 5000,
                    Hinh = "hao-hao.jpg",
                    MoTa = "Mì Hảo Hảo Tôm Chua Cay thùng 30 gói"
                },
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
                new HangHoa { 
                    TenHH = "Máy pha cà phê Delonghi",
                    MaLoai = loaiGiaDung.MaLoai,
                    DonGia = 5490000,
                    SoLuongTon = 12,
                    GiamGia = 400000, 
                    Hinh = "may-pha-cafe-delonghi.jpg",
                    MoTa = "Máy pha cà phê Delonghi EC685"
                },
                new HangHoa { 
                    TenHH = "Quạt cây Midea", 
                    MaLoai = loaiGiaDung.MaLoai, 
                    DonGia = 780000, 
                    SoLuongTon = 25,
                    GiamGia = 50000,
                    Hinh = "quat-midea.jpg", 
                    MoTa = "Quạt cây Midea 3 tốc độ, tiết kiệm điện"
                },
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
                new HangHoa {
                    TenHH = "Quạt đứng Asia",
                    MaLoai = loaiGiaDung.MaLoai,
                    DonGia = 690000,
                    SoLuongTon = 45,
                    GiamGia = 50000, 
                    Hinh = "quat-asia.jpg",
                    MoTa = "Quạt đứng Asia 3 tốc độ gió"
                },
                new HangHoa { 
                    TenHH = "Ấm siêu tốc Lock&Lock 1.7L",
                    MaLoai = loaiGiaDung.MaLoai, 
                    DonGia = 520000, 
                    SoLuongTon = 60, 
                    GiamGia = 20000,
                    Hinh = "am-sieu-toc-locknlock.jpg", 
                    MoTa = "Ấm siêu tốc Lock&Lock vỏ inox 1.7L"
                },
                new HangHoa { 
                    TenHH = "Bình giữ nhiệt Thermos 500ml",
                    MaLoai = loaiGiaDung.MaLoai,
                    DonGia = 590000,
                    SoLuongTon = 55, 
                    GiamGia = 30000, 
                    Hinh = "binh-thermos.jpg",
                    MoTa = "Bình giữ nhiệt thép không gỉ" 
                },
                new HangHoa { 
                    TenHH = "Bộ chén sứ Minh Long 10 món", 
                    MaLoai = loaiGiaDung.MaLoai,
                    DonGia = 890000, 
                    SoLuongTon = 30,
                    GiamGia = 50000,
                    Hinh = "chen-minh-long.jpg", 
                    MoTa = "Bộ chén sứ cao cấp Minh Long"
                },
                new HangHoa { 
                    TenHH = "Máy sấy tóc Philips",
                    MaLoai = loaiGiaDung.MaLoai,
                    DonGia = 490000, 
                    SoLuongTon = 60, 
                    GiamGia = 30000, 
                    Hinh = "may-say-philips.jpg", 
                    MoTa = "Máy sấy tóc Philips 2000W"
                },
                new HangHoa {
                    TenHH = "Bàn ủi hơi nước Panasonic", 
                    MaLoai = loaiGiaDung.MaLoai,
                    DonGia = 780000,
                    SoLuongTon = 25, 
                    GiamGia = 50000, 
                    Hinh = "ban-ui-panasonic.jpg",
                    MoTa = "Bàn ủi hơi nước công suất 1200W" 
                },

                // Thời trang
                  new HangHoa { 
                      TenHH = "Áo sơ mi nam công sở",
                      MaLoai = loaiThoiTrang.MaLoai, 
                      DonGia = 350000, 
                      SoLuongTon = 50, 
                      GiamGia = 25000,
                      Hinh = "ao-so-mi-nam.jpg", 
                      MoTa = "Áo sơ mi nam form slim fit, vải cotton"
                  },
                  new HangHoa { 
                      TenHH = "Giày da nam Classic",
                      MaLoai = loaiThoiTrang.MaLoai, 
                      DonGia = 1250000, 
                      SoLuongTon = 30,
                      GiamGia = 100000,
                      Hinh = "giay-da-nam.jpg",
                      MoTa = "Giày da nam Classic, sang trọng"
                  },
                  new HangHoa { 
                      TenHH = "Túi xách nam Puma",
                      MaLoai = loaiThoiTrang.MaLoai,
                      DonGia = 890000,
                      SoLuongTon = 20, 
                      GiamGia = 50000, 
                      Hinh = "tui-xach-puma.jpg", 
                      MoTa = "Túi xách nam Puma thời trang" },

                new HangHoa { 
                    TenHH = "Quần jean nam Levi's",
                    MaLoai = loaiThoiTrang.MaLoai,
                    DonGia = 1290000,
                    SoLuongTon = 35, 
                    GiamGia = 150000, 
                    Hinh = "quan-jean-levis.jpg",
                    MoTa = "Quần jean nam Levi's form slim"
                },
                new HangHoa {
                    TenHH = "Túi xách nữ Charles & Keith",
                    MaLoai = loaiThoiTrang.MaLoai, 
                    DonGia = 1590000,
                    SoLuongTon = 22, 
                    GiamGia = 200000,
                    Hinh = "tui-charles-keith.jpg", 
                    MoTa = "Túi xách nữ Charles & Keith thời trang" 
                },
                new HangHoa { 
                    TenHH = "Áo hoodie Uniqlo",
                    MaLoai = loaiThoiTrang.MaLoai, 
                    DonGia = 690000, 
                    SoLuongTon = 70,
                    GiamGia = 50000, 
                    Hinh = "hoodie-uniqlo.jpg", 
                    MoTa = "Áo hoodie nỉ Uniqlo mềm mịn"
                },
                new HangHoa { 
                    TenHH = "Dép quai ngang Nike",
                    MaLoai = loaiThoiTrang.MaLoai,
                    DonGia = 590000,
                    SoLuongTon = 60, 
                    GiamGia = 30000, 
                    Hinh = "dep-nike.jpg",
                    MoTa = "Dép thể thao Nike Unisex" 
                },
                new HangHoa {
                    TenHH = "Đồng hồ Casio MTP-1370",
                    MaLoai = loaiThoiTrang.MaLoai,
                    DonGia = 1590000,
                    SoLuongTon = 22, 
                    GiamGia = 100000, 
                    Hinh = "casio-mtp1370.jpg",
                    MoTa = "Đồng hồ Casio dây thép không gỉ" 
                },
                new HangHoa {
                    TenHH = "Ba lô laptop Lenovo 15 inch", 
                    MaLoai = loaiThoiTrang.MaLoai, 
                    DonGia = 450000, 
                    SoLuongTon = 50, 
                    GiamGia = 20000, 
                    Hinh = "balo-lenovo.jpg",
                    MoTa = "Ba lô laptop chống sốc 15 inch" },
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
                },
                new HangHoa { 
                    TenHH = "Tư Duy Phản Biện", 
                    MaLoai = loaiSach.MaLoai, 
                    DonGia = 105000, 
                    SoLuongTon = 120, 
                    GiamGia = 5000,
                    Hinh = "tu-duy-phan-bien.jpg",
                    MoTa = "Sách phát triển tư duy logic và phản biện" 
                },
                new HangHoa { 
                    TenHH = "Harry Potter và Hòn đá Phù thủy", 
                    DonGia = 135000, 
                    MaLoai = loaiSach.MaLoai,
                    SoLuongTon = 95,
                    GiamGia = 10000, 
                    Hinh = "harry-potter-1.jpg", 
                    MoTa = "Phần 1 bộ truyện Harry Potter nổi tiếng"
                },
                new HangHoa { 
                    TenHH = "Muôn Kiếp Nhân Sinh", 
                    MaLoai = loaiSach.MaLoai, 
                    DonGia = 159000, 
                    SoLuongTon = 90, 
                    GiamGia = 10000,
                    Hinh = "muon-kiep-nhan-sinh.jpg",
                    MoTa = "Tác phẩm nổi tiếng của Nguyên Phong" 
                },
                new HangHoa { 
                    TenHH = "Tuổi Trẻ Đáng Giá Bao Nhiêu",
                    MaLoai = loaiSach.MaLoai,
                    DonGia = 89000,
                    SoLuongTon = 140,
                    GiamGia = 5000,
                    Hinh = "tuoi-tre-dang-gia.jpg",
                    MoTa = "Sách truyền cảm hứng cho giới trẻ" 
                },
                new HangHoa { 
                    TenHH = "Sapiens - Lược sử loài người",
                    MaLoai = loaiSach.MaLoai,
                    DonGia = 159000,
                    SoLuongTon = 60, 
                    GiamGia = 10000, 
                    Hinh = "sapiens.jpg", 
                    MoTa = "Sách Sapiens - Yuval Noah Harari"
                },
                new HangHoa {
                    TenHH = "Người bán hàng vĩ đại",
                    MaLoai = loaiSach.MaLoai,
                    DonGia = 99000,
                    SoLuongTon = 75,
                    GiamGia = 5000, 
                    Hinh = "nguoi-ban-hang-viet.jpg", 
                    MoTa = "Sách dạy kỹ năng bán hàng và tư duy kinh doanh" 
                },
                new HangHoa { 
                    TenHH = "7 Thói quen của người thành đạt",
                    MaLoai = loaiSach.MaLoai, 
                    DonGia = 129000,
                    SoLuongTon = 80,
                    GiamGia = 10000, 
                    Hinh = "7-thoi-quen.jpg", 
                    MoTa = "Sách phát triển bản thân, Stephen R. Covey" 
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

