using PortfolioApi.Dtos;

namespace PortfolioApi.Services;

public interface ITransactionService
{
    Task<TransactionResponse> CreateAsync(int portfolioId, CreateTransactionRequest request, CancellationToken ct);
}