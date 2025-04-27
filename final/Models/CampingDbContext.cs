using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace final.Models
{
    public class CampingDbContext : IdentityDbContext<ApplicationUser>
    {
        public CampingDbContext(DbContextOptions<CampingDbContext> options) : base(options)
        {
        }
        public DbSet<Camp> Camps { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Camp)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CampId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed admin user
            SeedAdminUser(modelBuilder);
        }

        private void SeedAdminUser(ModelBuilder modelBuilder)
        {
            // Create admin role
            var adminRoleId = "1";
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "9b295fed-0d5d-4877-9b3d-5b60a551e351" // Static GUID
                }
            );

            // Create admin user
            var adminUserId = "1";
            var adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@camping.com",
                NormalizedUserName = "ADMIN@CAMPING.COM",
                Email = "admin@camping.com",
                NormalizedEmail = "ADMIN@CAMPING.COM",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                IsAdmin = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            // Set password hash for "Admin123!" - this is a properly generated hash
            adminUser.PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(adminUser, "Admin123!");

            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

            // Assign admin role to admin user
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                }
            );
        }

        // Add this method to suppress the warning if needed
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));

            base.OnConfiguring(optionsBuilder);
        }
    }
}
