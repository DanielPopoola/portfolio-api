using PortfolioApi.Dtos;

namespace PortfolioApi.Services;

public interface IPortfolioService
{
    Task<PortfolioResponse> CreateAsync(CreatePortfolioRequest request, CancellationToken ct);
    Task<List<PortfolioResponse>> ListByOwnerAsync(string ownerEmail, CancellationToken ct);
    Task<PortfolioResponse> GetByIdAsync(int id, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}