namespace PortfolioApi.Dtos;

public record HoldingResponse
{
    public string Symbol { get; init; } = null!;
    public decimal Quantity { get; init; }
    public decimal AverageCost { get; init; }
    public decimal CostBasis { get; init; }
    public decimal CurrentPrice { get; init; }
    public decimal MarketValue { get; init; }
    public decimal UnrealizedPnl { get; init; }
    public decimal UnrealizedPnlPercent { get; init; }
    public decimal RealizedPnl { get; init; }
}