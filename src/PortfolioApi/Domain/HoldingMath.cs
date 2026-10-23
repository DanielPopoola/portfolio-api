namespace PortfolioApi.Domain;

public static class HoldingMath
{
    public static decimal AverageCostAfterBuy(
        decimal oldQty, decimal oldAvgCost, decimal qty, decimal unitPrice, decimal fee)
        => (oldQty * oldAvgCost + qty * unitPrice + fee) / (oldQty + qty);

    public static decimal RealizedPnlOnSell(
        decimal qty, decimal unitPrice, decimal fee, decimal avgCost)
        => (qty * unitPrice - fee) - qty * avgCost;

    public static decimal CostBasis(decimal qty, decimal avgCost) => qty * avgCost;
    public static decimal MarketValue(decimal qty, decimal currentPrice) => qty * currentPrice;
    public static decimal UnrealizedPercent(decimal unrealizedPnl, decimal costBasis) =>
        costBasis == 0 ? 0 : (unrealizedPnl / costBasis) * 100;
}
