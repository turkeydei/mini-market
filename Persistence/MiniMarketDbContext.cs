using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class MiniMarketDbContext : DbContext
{
    public MiniMarketDbContext(DbContextOptions<MiniMarketDbContext> options)
        : base(options)
    {
    }

    // 5 DTO chính
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Loai> Loais { get; set; } = null!;
    public DbSet<HangHoa> HangHoas { get; set; } = null!;
    public DbSet<HoaDon> HoaDons { get; set; } = null!;
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; } = null!;
    
    // DTO phụ trợ
    public DbSet<ChiTietHD> ChiTietHDs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relationship: Loai -> HangHoa (One-to-Many)
        modelBuilder
            .Entity<HangHoa>()
            .HasOne(h => h.Loai)
            .WithMany(l => l.HangHoas)
            .HasForeignKey(h => h.MaLoai)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: HangHoa -> ChiTietHD (One-to-Many)
        modelBuilder
            .Entity<ChiTietHD>()
            .HasOne(ct => ct.HangHoa)
            .WithMany(h => h.ChiTietHDs)
            .HasForeignKey(ct => ct.MaHH)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: HoaDon -> ChiTietHD (One-to-Many)
        modelBuilder
            .Entity<ChiTietHD>()
            .HasOne(ct => ct.HoaDon)
            .WithMany(hd => hd.ChiTietHDs)
            .HasForeignKey(ct => ct.MaHD)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: User -> HoaDon (One-to-Many)
        modelBuilder
            .Entity<HoaDon>()
            .HasOne(hd => hd.User)
            .WithMany(u => u.HoaDons)
            .HasForeignKey(hd => hd.MaUser)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: HoaDon -> PaymentTransaction (One-to-One)
        modelBuilder
            .Entity<PaymentTransaction>()
            .HasOne(pt => pt.HoaDon)
            .WithOne(hd => hd.PaymentTransaction)
            .HasForeignKey<PaymentTransaction>(pt => pt.MaHD)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
