using Microsoft.EntityFrameworkCore;

namespace BiSaji.API.Data
{
    public class BiSajiDbContext : DbContext
    {
        public BiSajiDbContext(DbContextOptions<BiSajiDbContext> dbContextOptions) : base(dbContextOptions)
        {
            
        }
    }
}
