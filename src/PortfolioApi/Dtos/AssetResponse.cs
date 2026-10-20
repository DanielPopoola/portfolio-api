using PortfolioApi.Domain;

namespace PortfolioApi.Dtos;

public record AssetResponse
{
    public int Id { get; init; }
    public string Symbol { get; init; } = null!;
    public string Name { get; init; } = null!;
    public AssetType AssetType { get; init; }
    public decimal CurrentPrice { get; init; }
    public DateTimeOffset PriceUpdatedAtUtc { get; init; }
}