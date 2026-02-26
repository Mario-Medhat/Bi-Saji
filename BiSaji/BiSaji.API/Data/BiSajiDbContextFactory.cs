using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BiSaji.API.Data
{
    public class BiSajiDbContextFactory
        : IDesignTimeDbContextFactory<BiSajiDbContext>
    {
        public BiSajiDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<BiSajiDbContext>();

            var connectionString = configuration.GetConnectionString("BiSajiConnectionString");

            optionsBuilder.UseSqlServer(connectionString);

            return new BiSajiDbContext(optionsBuilder.Options);
        }
    }
}