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

    [HttpGet]
    public async Task<ActionResult<List<PortfolioResponse>>> List(string ownerEmail, CancellationToken ct)
        => Ok(await portfolioService.ListByOwnerAsync(ownerEmail, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PortfolioResponse>> GetById(int id, CancellationToken ct)
        => Ok(await portfolioService.GetByIdAsync(id, ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await portfolioService.DeleteAsync(id, ct);
        return NoContent();
    }
} 