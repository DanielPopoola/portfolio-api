using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApi.Domain;

namespace PortfolioApi.Data.Configurations;

public class PortfolioConfiguration : IEntityTypeConfiguration<Portfolio>
{
    public void Configure(EntityTypeBuilder<Portfolio> builder)
    {
        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(p => new { p.OwnerEmail, p.Name }).IsUnique();
        builder.Property(p => p.OwnerEmail).HasMaxLength(100).IsRequired();
        builder.Property(p => p.BaseCurrency).HasMaxLength(3).IsRequired();
        
    }
}
