using Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Persistence;

public class MiniMarketDbContext: DbContext
    {
        public MiniMarketDbContext(DbContextOptions<MiniMarketDbContext> options)
            : base(options)
        {
        }

        public DbSet<HangHoa> HangHoas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình bảng
             modelBuilder
            .Entity<HangHoa>()
            .HasOne(h => h.Loai)
            .WithMany(l => l.HangHoas)
            .HasForeignKey(h => h.MaLoai);
        }
    }
