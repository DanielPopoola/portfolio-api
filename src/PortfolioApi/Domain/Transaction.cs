namespace PortfolioApi.Domain;

public class Transaction
{
    public int Id { get; init; }
    public int PortfolioId { get; init; }
    public Portfolio Portfolio { get; init; } = null!;
    public int AssetId { get; init; }
    public Asset Asset { get; init; } = null!;
    public TransactionType Type { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Fee { get; init; }
    public DateTimeOffset ExecutedAtUtc { get; init; }
}
