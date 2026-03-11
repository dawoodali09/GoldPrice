using Microsoft.EntityFrameworkCore;
using GoldPrice.Models;

namespace GoldPrice.Data;

public class GoldPriceDbContext : DbContext
{
    public GoldPriceDbContext(DbContextOptions<GoldPriceDbContext> options) : base(options)
    {
    }

    public DbSet<PriceHistory> PriceHistory { get; set; }
    public DbSet<MyPortfolio> MyPortfolio { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PriceHistory>(entity =>
        {
            entity.ToTable("PriceHistory");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MetalType).HasMaxLength(10).IsRequired();
            entity.Property(e => e.PricePerGram).HasPrecision(18, 4);
            entity.Property(e => e.PricePerOunce).HasPrecision(18, 4);
            entity.Property(e => e.Currency).HasMaxLength(10).HasDefaultValue("NZD");
            entity.HasIndex(e => new { e.MetalType, e.FetchedAt });
        });

        modelBuilder.Entity<MyPortfolio>(entity =>
        {
            entity.ToTable("MyPortfolio");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MetalType).HasMaxLength(10).IsRequired();
            entity.Property(e => e.QuantityGrams).HasPrecision(18, 4);
            entity.Property(e => e.PurchasePrice).HasPrecision(18, 4);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasIndex(e => e.MetalType);
        });
    }
}
