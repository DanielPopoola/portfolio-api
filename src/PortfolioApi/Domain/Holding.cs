namespace PortfolioApi.Domain;

public class Holding
{
    public int Id { get; set; }
    public int PortfolioId { get; set; }
    public Portfolio Portfolio { get; set; } = null!;
    public int AssetId { get; set; }
    public Asset Asset { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal AverageCost { get; set; }
    public decimal RealizedPnl { get; set;  }
    public byte[] RowVersion { get; set; } = null!;
}