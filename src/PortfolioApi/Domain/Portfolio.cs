namespace PortfolioApi.Domain;

public class Portfolio
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string OwnerEmail { get; set; } = null!;
    public string BaseCurrency { get; set; } = "USD";
    public DateTimeOffset CreatedAtUtc { get; set; }
}
