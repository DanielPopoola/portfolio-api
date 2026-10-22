using Microsoft.AspNetCore.Mvc;
using PortfolioApi.Dtos;
using PortfolioApi.Services;

namespace PortfolioApi.Controllers;

[ApiController]
[Route("api/portfolios/{portfolioId:int}/transactions")]
public class TransactionsController(ITransactionService transactionService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<TransactionResponse>> Create(
        int portfolioId, CreateTransactionRequest request, CancellationToken ct)
    {
        var created = await transactionService.CreateAsync(portfolioId, request, ct);

        return Created($"/api/portfolios/{portfolioId}/transactions", created);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<TransactionResponse>>> List(
        int portfolioId, [FromQuery] TransactionListQuery query, CancellationToken ct)
        => Ok(await transactionService.ListAsync(portfolioId, query, ct));
}