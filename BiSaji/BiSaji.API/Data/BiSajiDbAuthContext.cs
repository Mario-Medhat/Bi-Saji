using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BiSaji.API.Data
{
    public class BiSajiDbAuthContext : IdentityDbContext
    {

        // Constructor
        public BiSajiDbAuthContext(DbContextOptions<BiSajiDbAuthContext> options) : base(options) { }

        // Seed data for IdentityRole
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var adminId = "12345678-90ab-cdef-1234-567890abcdef";
            var leaderId = "abcdef12-3456-7890-abcd-ef1234567890";
            var servantId = "8363be11-00d4-4b6a-8e31-bc27452c477c";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = adminId,
                    ConcurrencyStamp = adminId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                },

                new IdentityRole
                {
                    Id = leaderId,
                    ConcurrencyStamp = leaderId,
                    Name = "Leader",
                    NormalizedName = "Leader".ToUpper()
                },

                new IdentityRole
                {
                    Id = servantId,
                    ConcurrencyStamp = leaderId,
                    Name = "Servant",
                    NormalizedName = "Servant".ToUpper()
                },
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }


    }
}
