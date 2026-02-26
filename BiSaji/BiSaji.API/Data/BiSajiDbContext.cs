using BiSaji.API.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BiSaji.API.Data
{
    /// <summary>
    /// Represents the main database context for the BiSaji application,
    /// combining Identity and business domain entities.
    /// </summary>
    public class BiSajiDbContext : IdentityDbContext<Servant>
    {
        public BiSajiDbContext(DbContextOptions<BiSajiDbContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<Batch> Batches { get; set; }
        public DbSet<Day> Days { get; set; }
        public DbSet<MovementGroup> MovementGroups { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<PeriodAssignment> PeriodAssignments { get; set; }
        public DbSet<RoomAssignment> RoomAssignments { get; set; }
        public DbSet<RoomRole> RoomRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureIdentitySchema(modelBuilder);
            ConfigureRelationships(modelBuilder);
            SeedData(modelBuilder);
        }
        /// <summary>
        /// Moves all Identity tables into the "auth" schema, keeping business tables in "dbo".
        /// </summary>
        private static void ConfigureIdentitySchema(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Servant>().ToTable("AspNetUsers", "auth");
            modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles", "auth");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", "auth");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", "auth");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", "auth");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", "auth");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", "auth");
        }

        /// <summary>
        /// Configures the many-to-many relationships that EF Core cannot resolve automatically.
        /// </summary>
        private static void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Day>()
                .HasMany(d => d.DayServants)
                .WithMany()
                .UsingEntity("DayServants");

            modelBuilder.Entity<Day>()
                .HasMany(d => d.DayLeaders)
                .WithMany()
                .UsingEntity("DayLeaders");

            modelBuilder.Entity<Day>()
                .HasOne(d => d.CreatedBy)
                .WithMany(s => s.CreatedDays)
                .HasForeignKey(d => d.CreatedByServantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MovementGroup>()
                .HasMany(mg => mg.Students)
                .WithMany()
                .UsingEntity("MovementGroupStudents");

            modelBuilder.Entity<MovementGroup>()
            .HasOne(mg => mg.ResponsibleServant)
            .WithMany()
            .HasForeignKey(mg => mg.ResponsibleServantId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Batch>()
                .HasOne(b => b.CreatedBy)
                .WithMany()
                .HasForeignKey(b => b.CreatedByServantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Batch>()
                .HasOne(b => b.Leader)
                .WithMany()
                .HasForeignKey(b => b.LeaderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PeriodAssignment>()
                .HasOne(pa => pa.Period)
                .WithMany(p => p.Assignments)
                .HasForeignKey(pa => pa.PeriodId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PeriodAssignment>()
                .HasOne(pa => pa.Room)
                .WithMany()
                .HasForeignKey(pa => pa.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PeriodAssignment>()
                .HasOne(pa => pa.MovementGroup)
                .WithMany()
                .HasForeignKey(pa => pa.MovementGroupId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        /// <summary>
        /// Seeds the database with initial data for places and identity roles.
        /// </summary>
        private static void SeedData(ModelBuilder modelBuilder)
        {
            SeedPlaces(modelBuilder);
            SeedRoles(modelBuilder);
        }

        /// <summary>
        /// Seeds the initial set of known places.
        /// </summary>
        private static void SeedPlaces(ModelBuilder modelBuilder)
        {
            var places = new List<Place>
            {
                new Place
                {
                    Id = Guid.Parse("abfed5c9-7adf-49e7-8a15-35982eb11694"),
                    Name = "القاعة",
                    Description = "القاعة الرئيسية"
                },
                new Place
                {
                    Id = Guid.Parse("cc13100e-4378-429a-a768-e7699e546c47"),
                    Name = "السندره",
                    Description = "السندره فوق في وش السلم"
                },
                new Place
                {
                    Id = Guid.Parse("f4a22e27-1dff-4977-b15c-6547183f7163"),
                    Name = "الشباك",
                    Description = "الغرفة اللي فيها الشباك"
                },
                new Place
                {
                    Id = Guid.Parse("b477c12f-ae19-46a4-89e4-c920c4c0497b"),
                    Name = "جنب الشباك",
                    Description = "الغرفة اللي جنب الشباك"
                }
            };

            modelBuilder.Entity<Place>().HasData(places);
        }

        /// <summary>
        /// Seeds the initial identity roles.
        /// </summary>
        private static void SeedRoles(ModelBuilder modelBuilder)
        {
            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = "12345678-90ab-cdef-1234-567890abcdef",
                    ConcurrencyStamp = "12345678-90ab-cdef-1234-567890abcdef",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Id = "abcdef12-3456-7890-abcd-ef1234567890",
                    ConcurrencyStamp = "abcdef12-3456-7890-abcd-ef1234567890",
                    Name = "BatchLeader",
                    NormalizedName = "BATCHLEADER"
                },
                new IdentityRole
                {
                    Id = "bc14dbc5-5b14-4020-8ba4-1441d4de31d0",
                    ConcurrencyStamp = "bc14dbc5-5b14-4020-8ba4-1441d4de31d0",
                    Name = "Servant",
                    NormalizedName = "SERVANT"
                }
            };

            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
    }
}