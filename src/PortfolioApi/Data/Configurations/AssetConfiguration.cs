using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApi.Domain;

namespace PortfolioApi.Data.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.Property(a => a.Symbol).HasMaxLength(10);
        builder.HasIndex(a => a.Symbol).IsUnique();
        builder.Property(a => a.Name).HasMaxLength(100).IsRequired();
        builder.Property(a => a.AssetType).HasConversion<string>();
        builder.Property(a => a.CurrentPrice).HasPrecision(18, 4);
    }
}