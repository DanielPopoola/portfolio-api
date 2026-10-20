namespace PortfolioApi.Domain;

public class Asset
{
    public int Id { get; set;  }
    public string Symbol { get; set; } = null!;
    public string Name { get; set; } = null!;
    public AssetType AssetType { get; set; }
    public decimal CurrentPrice { get; set; }
    public DateTimeOffset PriceUpdatedAtUtc { get; set; }
}