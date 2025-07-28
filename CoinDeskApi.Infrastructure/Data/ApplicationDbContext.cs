using Microsoft.EntityFrameworkCore;
using CoinDeskApi.Core.Entities;

namespace CoinDeskApi.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Currency> Currencies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Currency entity configuration
            modelBuilder.Entity<Currency>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasMaxLength(10).IsRequired();
                entity.Property(e => e.ChineseName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.EnglishName).HasMaxLength(100);
                entity.Property(e => e.Symbol).HasMaxLength(10);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETDATE()");

                // Index for better query performance
                entity.HasIndex(e => e.Id).IsUnique();
            });

            // Seed data
            modelBuilder.Entity<Currency>().HasData(
                new Currency { Id = "USD", ChineseName = "美元", EnglishName = "US Dollar", Symbol = "$", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Currency { Id = "EUR", ChineseName = "歐元", EnglishName = "Euro", Symbol = "€", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Currency { Id = "GBP", ChineseName = "英鎊", EnglishName = "British Pound Sterling", Symbol = "£", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
        }
    }
}
