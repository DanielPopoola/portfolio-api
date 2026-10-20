using Microsoft.EntityFrameworkCore;
using PortfolioApi.Domain;

namespace PortfolioApi.Data;


public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Portfolio> Portfolios => Set<Portfolio>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Holding> Holdings => Set<Holding>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}