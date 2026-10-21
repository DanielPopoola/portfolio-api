using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Domain;
using PortfolioApi.Dtos;
using PortfolioApi.Services;
using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/portfolios")]
public class PortfoliosController(IPortfolioService portfolioService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<PortfolioResponse>> Create(CreatePortfolioRequest request, CancellationToken ct)
    {
        var created = await portfolioService.CreateAsync(request, ct);

        return Created($"/api/portfolios/{created.Id}", created);
    }
} 