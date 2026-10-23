using PortfolioApi.Dtos;

namespace PortfolioApi.Services;

public interface ISummaryService
{
    Task<SummaryResponse> GetAsync(int portfolioId, CancellationToken ct);
}