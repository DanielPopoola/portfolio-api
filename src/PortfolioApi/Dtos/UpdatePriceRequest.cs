using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Dtos;

public record UpdatePriceRequest
{
    [Range(typeof(decimal), "0.0001", "99999999999999.0")]
    public decimal Price { get; init; }
}