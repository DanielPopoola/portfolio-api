using System.ComponentModel.DataAnnotations;
using PortfolioApi.Domain;

namespace PortfolioApi.Dtos;

public record CreateAssetRequest
{
    [Required]
    [StringLength(10, MinimumLength = 1)]
    [RegularExpression(@"^[a-zA-Z0-9]+$")]
    public string Symbol { get; init; } = null!;

    [Required]
    [StringLength(100)]
    public string Name { get; init;  } = null!;

    [Required]
    [EnumDataType(typeof(AssetType))]
    public AssetType? AssetType { get; init; }

    [Range(typeof(decimal), "0.0001", "99999999999999.0")]
    public decimal CurrentPrice { get; init; }
}