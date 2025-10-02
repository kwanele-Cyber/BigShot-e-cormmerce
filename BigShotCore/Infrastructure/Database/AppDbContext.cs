using BigShotCore.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BigShotCore.Infrastructure.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public DbSet<Order> Orders { get; set; }
        //public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<AppRole> Roles { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed default roles
            modelBuilder.Entity<AppRole>().HasData(
                new AppRole { Id = 1, Name = "Admin" },
                new AppRole { Id = 2, Name = "Customer" }
            );

            modelBuilder.Entity<AppUser>().HasData(
                new AppUser {
                    Id = -1,
                    UserName = "Admin",
                    Email = "therealadmin@gmail.com",
                    ApiKey = "jjTaQ96",
                    RoleId = 1
                },
                new AppUser
                {
                    Id = -2,
                    UserName = "Jake",
                    Email = "jake@gmail.com",
                    ApiKey = "utpqr48",
                    RoleId = 1
                }
            );
        }
    }
}
