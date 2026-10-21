namespace PortfolioApi.Domain;

public static class HoldingMath
{
    public static decimal AverageCostAfterBuy(
        decimal oldQty, decimal oldAvgCost, decimal qty, decimal unitPrice, decimal fee)
        => (oldQty * oldAvgCost + qty * unitPrice + fee) / (oldQty + qty);

    public static decimal RealizedPnlOnSell(
        decimal qty, decimal unitPrice, decimal fee, decimal avgCost)
        => (qty * unitPrice - fee) - qty * avgCost;
}
