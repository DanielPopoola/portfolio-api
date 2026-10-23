using Microsoft.EntityFrameworkCore;
using PortfolioApi.Common;
using PortfolioApi.Data;
using PortfolioApi.Domain;
using PortfolioApi.Dtos;
using PortfolioApi.Exceptions;

namespace PortfolioApi.Services;

public class SummaryService(AppDbContext db) : ISummaryService
{
    public async Task<SummaryResponse> GetAsync(int portfolioId, CancellationToken ct)
    {
        var portfolio = await db.Portfolios
                .AsNoTracking()
                .Where(p => p.Id == portfolioId)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.BaseCurrency
                }).FirstOrDefaultAsync(ct) ?? throw new NotFoundException($"Portfolio {portfolioId} does not exist.");

        var holdings = await db.Holdings
                .AsNoTracking()
                .Where(h => h.PortfolioId == portfolioId)
                .Select(h => new
                {
                    h.Asset.AssetType,
                    h.Quantity,
                    h.AverageCost,
                    Price = h.Asset.CurrentPrice,
                    h.RealizedPnl
                }).ToListAsync(ct);

        var realized = holdings.Sum(h => h.RealizedPnl);

        var openHoldings = holdings
                .Where(h => h.Quantity > 0)
                .Select(h => new
                {
                    h.AssetType,
                    CostBasis = HoldingMath.CostBasis(h.Quantity, h.AverageCost),
                    MarketValue = HoldingMath.MarketValue(h.Quantity, h.Price)
                })
                .ToList();

        var totalMarketValue = openHoldings.Sum(h => h.MarketValue);
        var totalCostBasis = openHoldings.Sum(h => h.CostBasis);
        var unrealized = totalMarketValue - totalCostBasis;
        var unrealizedPercent = HoldingMath.UnrealizedPercent(unrealized, totalCostBasis);

        var allocation = openHoldings
            .GroupBy(h => h.AssetType)
            .Select(g => new
            {
                AssetType = g.Key,
                TypeValue = g.Sum(h => h.MarketValue),
                Percent = totalMarketValue == 0
                    ? 0
                    : g.Sum(h => h.MarketValue) / totalMarketValue * 100
            })
            .OrderByDescending(g => g.TypeValue)
            .Select(g => new AllocationItem
            {
                AssetType = g.AssetType,
                MarketValue = MoneyMath.Round2(g.TypeValue),
                Percent = MoneyMath.Round2(g.Percent)
            })
            .ToList();

        return new SummaryResponse
        {
            PortfolioId = portfolio.Id,
            Name = portfolio.Name,
            BaseCurrency = portfolio.BaseCurrency,
            HoldingsCount = openHoldings.Count,
            TotalMarketValue = MoneyMath.Round2(totalMarketValue),
            TotalCostBasis = MoneyMath.Round2(totalCostBasis),
            UnrealizedPnl = MoneyMath.Round2(unrealized),
            UnrealizedPnlPercent = MoneyMath.Round2(unrealizedPercent),
            RealizedPnl = MoneyMath.Round2(realized),
            TotalReturn = MoneyMath.Round2(unrealized + realized),
            Allocation = allocation
        };
    }
}

