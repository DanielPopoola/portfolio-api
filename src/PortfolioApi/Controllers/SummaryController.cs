using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Dtos;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/portfolios/{portfolioId:int}/summary")]
public class SummaryController(ISummaryService summaryService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SummaryResponse>>> GetByPortfolio(int portfolioId, CancellationToken ct)
        => Ok(await summaryService.GetAsync(portfolioId, ct));
}
