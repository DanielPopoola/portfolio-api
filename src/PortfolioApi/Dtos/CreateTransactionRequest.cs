using System.ComponentModel.DataAnnotations;
using PortfolioApi.Domain;

namespace PortfolioApi.Dtos;

public record CreateTransactionRequest
{
    [Required]
    [StringLength(10, MinimumLength = 1)]
    [RegularExpression(@"^[a-zA-Z0-9]+$")]
    public string Symbol { get; init; } = null!;

    [Required]
    public TransactionType? Type { get; init; }

    [Range(typeof(decimal), "0.000001", "99999999999999.0", ErrorMessage = "Quantity must be greater than 0.")]
    public decimal Quantity { get; init; }

    [Range(typeof(decimal), "", "99999999999999.0")]
    public decimal? UnitPrice { get; init; }

    [Range(typeof(decimal), "0", "99999999999999.0", ErrorMessage = "Fee cannot be negative.")]
    public decimal Fee { get; init; } = 0;
}

