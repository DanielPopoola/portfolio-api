using PortfolioApi.Dtos;

namespace PortfolioApi.Services;

public interface IPortfolioService
{
    Task<PortfolioResponse> CreateAsync(CreatePortfolioRequest request, CancellationToken ct);
}