using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MiniMarketDbContext>
    {
        public MiniMarketDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MiniMarketDbContext>();

            // Điều chỉnh đường dẫn tới appsettings.json của dự án WebMiniShop
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "WebShop"))  // Điều chỉnh đúng với dự án WebMiniShop
                .AddJsonFile("appsettings.json")
                .Build();
    
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);


            optionsBuilder.UseSqlServer(connectionString, options => options.EnableRetryOnFailure());
            return new MiniMarketDbContext(optionsBuilder.Options);
        }
    }
}
