namespace PortfolioApi.Dtos;

public record PortfolioResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
    public string OwnerEmail { get; init; } = null!;
    public string BaseCurrency { get; init; } = null!;
    public DateTimeOffset CreatedAtUtc { get; init; }
}