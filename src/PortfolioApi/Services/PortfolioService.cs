using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Domain;
using PortfolioApi.Dtos;
using PortfolioApi.Exceptions;

namespace PortfolioApi.Services;

public class PortfolioService(AppDbContext db) : IPortfolioService
{
    private static readonly Expression<Func<Portfolio, PortfolioResponse>> ToResponse = p => new PortfolioResponse
    {
        Id = p.Id,
        Name = p.Name,
        OwnerEmail = p.OwnerEmail,
        BaseCurrency = p.BaseCurrency,
        CreatedAtUtc = p.CreatedAtUtc 
    };

    private static readonly Func<Portfolio, PortfolioResponse> MapToResposne = ToResponse.Compile();

    public async Task<PortfolioResponse> CreateAsync(CreatePortfolioRequest request, CancellationToken ct)
    {
        var portfolio = new Portfolio
        {
            Name = request.Name.Trim(),
            OwnerEmail = request.OwnerEmail.Trim().ToLowerInvariant(),
            BaseCurrency = request.BaseCurrency?.ToUpperInvariant() ?? "USD",
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        db.Portfolios.Add(portfolio);

        try
        {
            await db.SaveChangesAsync(ct);
        } catch(DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            throw new ConflictException("Duplicate portfolio", $"Portfolio {portfolio.Name} already exists for {portfolio.OwnerEmail}.");
        }

        return MapToResposne(portfolio);
    }
}
