using PortfolioApi.Domain;
using PortfolioApi.Dtos;

namespace PortfolioApi.Services;

public interface IAssetService
{
    Task<AssetResponse> CreateAsync(CreateAssetRequest request, CancellationToken ct);

    Task<AssetResponse> GetBySymbolAsync(string symbol, CancellationToken ct);

    Task<List<AssetResponse>> ListAsync(AssetType? type, string? search, CancellationToken ct);
}