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

    public async Task<List<PortfolioResponse>> ListByOwnerAsync(string ownerEmail, CancellationToken ct)
    {
        var email = ownerEmail.Trim().ToLowerInvariant();

        var portfolios = await db.Portfolios
                .AsNoTracking()
                .Where(p => p.OwnerEmail == email)
                .OrderBy(p => p.Name)
                .Select(ToResponse)
                .ToListAsync(ct);

        return portfolios;
    }

    public async Task<PortfolioResponse> GetByIdAsync(int id, CancellationToken ct)
    {
        return await db.Portfolios
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(ToResponse)
                .FirstOrDefaultAsync(ct) ?? throw new NotFoundException($"Portfolio {id} does not exist.");
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var portfolio = await db.Portfolios.FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new NotFoundException($"Portfolio {id} does not exist.");

        db.Portfolios.Remove(portfolio);

        try
        {
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.IsForeignKeyViolation())
        {
            throw new ConflictException("Portfolio has transactions", $"Portfolio {id} has transactions and cannot be deleted.");
        }
    }
}
