using Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Persistence;

public class MiniMarketDbContext: DbContext
    {
        public MiniMarketDbContext(DbContextOptions<MiniMarketDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<TrangThai> TrangThais { get; set; } = null!;
        public DbSet<Loai> Loais { get; set; } = null!;
        public DbSet<NhaCungCap> NhaCungCaps { get; set; } = null!;
        public DbSet<KhuyenMai> KhuyenMais { get; set; } = null!;
        public DbSet<KhachHang> KhachHangs { get; set; } = null!;
        public DbSet<HangHoa> HangHoas { get; set; } = null!;
        public DbSet<HoaDon> HoaDons { get; set; } = null!;
        public DbSet<ChiTietHD> ChiTietHDs { get; set; } = null!;
        public DbSet<NhapKho> NhapKhos { get; set; } = null!;
        public DbSet<TonKho> TonKhos { get; set; } = null!;
        public DbSet<ChiTietKhuyenMai> ChiTietKhuyenMais { get; set; } = null!;
        public DbSet<PhanQuyen> PhanQuyens { get; set; } = null!;
        public DbSet<LichSuGiaoDich> LichSuGiaoDiches { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           // Định nghĩa khóa chính tổng hợp cho thực thể ChiTietKhuyenMai
        modelBuilder.Entity<ChiTietKhuyenMai>().HasKey(ct => new { ct.MaKM, ct.MaHH });

        modelBuilder
            .Entity<HangHoa>()
            .HasOne(h => h.Loai)
            .WithMany(l => l.HangHoas)
            .HasForeignKey(h => h.MaLoai);

        // Define relationship between ChiTietHD and HangHoa
        modelBuilder
            .Entity<ChiTietHD>()
            .HasOne(ct => ct.HangHoa) // Navigation property
            .WithMany(h => h.ChiTietHDs) // Navigation property
            .HasForeignKey(ct => ct.MaHH) // Foreign key in ChiTietHD
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete if required

        modelBuilder
            .Entity<ChiTietHD>()
            .HasOne(ct => ct.HoaDon) // Navigation property in ChiTietHD
            .WithMany(hd => hd.ChiTietHDs) // Navigation property in HoaDon
            .HasForeignKey(ct => ct.MaHD) // Foreign key in ChiTietHD
            .OnDelete(DeleteBehavior.Restrict); // Adjust DeleteBehavior as needed

        modelBuilder
            .Entity<TonKho>()
            .HasOne(tk => tk.HangHoa)
            .WithMany(hh => hh.TonKhos)
            .HasForeignKey(tk => tk.MaHH)
            .OnDelete(DeleteBehavior.Cascade); // Adjust DeleteBehavior as needed

        // Define relationship between HoaDon and TrangThai
        modelBuilder
            .Entity<HoaDon>()
            .HasOne(hd => hd.TrangThai)
            .WithMany(tt => tt.HoaDons)
            .HasForeignKey(hd => hd.MaTrangThai)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
