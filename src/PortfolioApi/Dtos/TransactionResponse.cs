using PortfolioApi.Domain;

namespace PortfolioApi.Dtos;

public record TransactionResponse
{
    public int Id { get; init; }
    public int PortfolioId { get; init; }
    public string Symbol { get; init; } = null!;
    public TransactionType Type { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Fee { get; init; }
    public decimal GrossAmount { get; init; }
    public decimal NetAmount { get; init; }
    public decimal? RealizedPnl { get; init; }
    public DateTimeOffset ExecutedAtUtc { get; init; }
}