using System.ComponentModel.DataAnnotations;
using PortfolioApi.Domain;

namespace PortfolioApi.Dtos;

public record TransactionListQuery
{
    public string? Symbol { get; init; }
    public TransactionType? Type { get; init; }
    public DateOnly? From { get; init; }
    public DateOnly? To { get; init; }
    [Range(1, int.MaxValue, ErrorMessage = "Page must start from 1")]
    public int Page { get; init; } = 1;
    [Range(1, 100, ErrorMessage = "PagSize must be between 1-100")]
    public int PageSize { get; init; } = 20;
}