using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApi.Domain;

namespace PortfolioApi.Data.Configurations;

public class HoldingConfiguration : IEntityTypeConfiguration<Holding>
{
    public void Configure(EntityTypeBuilder<Holding> builder)
    { 
        builder.Property(h => h.Quantity).HasPrecision(18, 6);
        builder.Property(h => h.AverageCost).HasPrecision(18, 4);
        builder.Property(h => h.RealizedPnl).HasPrecision(18, 2);
        builder.HasIndex(h => new { h.PortfolioId, h.AssetId }).IsUnique();
        builder.Property(h => h.RowVersion).IsRowVersion();

        builder.ToTable(t => t.HasCheckConstraint("CK_Holding_Quantity_Min", "[Quantity] >= 0"));

        builder.HasOne(h => h.Portfolio)
            .WithMany()
            .HasForeignKey(h => h.PortfolioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.Asset)
            .WithMany()
            .HasForeignKey(h => h.AssetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
