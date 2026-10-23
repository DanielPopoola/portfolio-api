using Microsoft.EntityFrameworkCore;
using PortfolioApi.Common;
using PortfolioApi.Data;
using PortfolioApi.Domain;
using PortfolioApi.Dtos;
using PortfolioApi.Exceptions;


namespace PortfolioApi.Services;

public class HoldingService(AppDbContext db) : IHoldingService
{
    public async Task<List<HoldingResponse>> GetByPortfolioAsync(int portfolioId, CancellationToken ct)
    {
        var rows = await db.Holdings
            .AsNoTracking()
            .Where(h => h.PortfolioId == portfolioId && h.Quantity > 0)
            .OrderByDescending(h => h.Quantity * h.Asset.CurrentPrice)
            .ThenBy(h => h.Asset.Symbol)
            .Select(h => new
            {
                h.Asset.Symbol,
                h.Quantity,
                h.AverageCost,
                Price = h.Asset.CurrentPrice,
                h.RealizedPnl
            })
            .ToListAsync(ct);

        if (rows.Count == 0 && !(await db.Portfolios.AnyAsync(p => p.Id == portfolioId, ct)))
            throw new NotFoundException($"Portfolio {portfolioId} does not exist.");

        var response = rows.Select(row =>
        {
            var costBasis = HoldingMath.CostBasis(row.Quantity, row.AverageCost);
            var marketValue = HoldingMath.MarketValue(row.Quantity, row.Price);
            var unrealized = marketValue - costBasis;
            var unrealizedPercent = HoldingMath.UnrealizedPercent(unrealized, costBasis);

            return new HoldingResponse
            {
                Symbol = row.Symbol,
                Quantity = row.Quantity,
                AverageCost = MoneyMath.Round4(row.AverageCost),
                CurrentPrice = MoneyMath.Round2(row.Price),
                CostBasis = MoneyMath.Round2(costBasis),
                MarketValue = MoneyMath.Round2(marketValue),
                UnrealizedPnl = MoneyMath.Round2(unrealized),
                UnrealizedPnlPercent = MoneyMath.Round2(unrealizedPercent),
                RealizedPnl = MoneyMath.Round2(row.RealizedPnl)
            };
        }).ToList();

        return response;
    }
}