using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApi.Domain;

namespace PortfolioApi.Data.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.Property(t => t.Type).HasConversion<string>();
        builder.Property(t => t.Quantity).HasPrecision(18, 6);
        builder.Property(t => t.UnitPrice).HasPrecision(18, 4);
        builder.Property(t => t.Fee).HasPrecision(18, 2);

        builder.HasOne(t => t.Portfolio)
            .WithMany()
            .HasForeignKey(t => t.PortfolioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Asset)
            .WithMany()
            .HasForeignKey(t => t.AssetId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
