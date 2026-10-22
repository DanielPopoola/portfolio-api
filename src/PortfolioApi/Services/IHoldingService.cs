using PortfolioApi.Dtos;

namespace PortfolioApi.Services;

public interface IHoldingService
{
    Task<List<HoldingResponse>> GetByPortfolioAsync(int portfolioId, CancellationToken ct);
}
