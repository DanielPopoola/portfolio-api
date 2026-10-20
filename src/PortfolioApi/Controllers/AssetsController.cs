using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Domain;
using PortfolioApi.Dtos;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/assets")]
public class AssetsController(IAssetService assets): ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AssetResponse>> Create(CreateAssetRequest request, CancellationToken ct)
    {
        var created = await assets.CreateAsync(request, ct);

        return Created($"/api/assets/{created.Symbol}", created);
    }

    [HttpGet("{symbol}")]
    public async Task<ActionResult<AssetResponse>> GetBySymbol(string symbol, CancellationToken ct)
    {
        return Ok(await assets.GetBySymbolAsync(symbol, ct));
    }

    [HttpGet]
    public async Task<ActionResult<List<AssetResponse>>> List(AssetType? type, string? search, CancellationToken ct)
    {
        return Ok(await assets.ListAsync(type, search, ct));
    }
}