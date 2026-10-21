using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Domain;
using PortfolioApi.Dtos;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/assets")]
public class AssetsController(IAssetService assetService): ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AssetResponse>> Create(CreateAssetRequest request, CancellationToken ct)
    {
        var created = await assetService.CreateAsync(request, ct);

        return Created($"/api/assets/{created.Symbol}", created);
    }

    [HttpGet("{symbol}")]
    public async Task<ActionResult<AssetResponse>> GetBySymbol(string symbol, CancellationToken ct)
        => Ok(await assetService.GetBySymbolAsync(symbol, ct));

    [HttpGet]
    public async Task<ActionResult<List<AssetResponse>>> List(AssetType? type, string? search, CancellationToken ct)
        => Ok(await assetService.ListAsync(type, search, ct));
  

    [HttpPut("{symbol}/price")]
    public async Task<ActionResult<AssetResponse>> UpdatePrice(string symbol, UpdatePriceRequest request, CancellationToken ct)
        => Ok(await assetService.UpdatePriceAsync(symbol, request, ct));
}