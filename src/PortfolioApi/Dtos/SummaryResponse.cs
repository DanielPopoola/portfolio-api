using PortfolioApi.Domain;

namespace PortfolioApi.Dtos;

public record AllocationItem
{
    public AssetType AssetType { get; init; }
    public decimal MarketValue { get; init; }
    public decimal Percent { get; init; }
}
public record SummaryResponse
{
    public int PortfolioId { get; init; }
    public string Name { get; init; } = null!;
    public string BaseCurrency { get; init; } = null!;
    public int HoldingsCount { get; init; }
    public decimal TotalMarketValue { get; init; }
    public decimal TotalCostBasis { get; init; }
    public decimal UnrealizedPnl { get; init; }
    public decimal UnrealizedPnlPercent { get; init; }
    public decimal RealizedPnl { get; init; }
    public decimal TotalReturn { get; init; }
    public List<AllocationItem> Allocation { get; init; } = [];
}
