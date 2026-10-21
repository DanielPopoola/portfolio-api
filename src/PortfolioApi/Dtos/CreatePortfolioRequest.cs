using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Dtos;

public record CreatePortfolioRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; init; } = null!;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string OwnerEmail { get; init; } = null!;

    [RegularExpression("^[A-Za-z]{3}$")]
    public string? BaseCurrency { get; init; }
}
