using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Dtos;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/portfolios/{portfolioId:int}/holdings")]
public class HoldingsController(IHoldingService holdingService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<HoldingResponse>>> GetByPortfolio(int portfolioId, CancellationToken ct)
        => Ok(await holdingService.GetByPortfolioAsync(portfolioId, ct));
}
