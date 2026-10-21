using Microsoft.EntityFrameworkCore;
using PortfolioApi.Common;
using PortfolioApi.Data;
using PortfolioApi.Domain;
using PortfolioApi.Dtos;
using PortfolioApi.Exceptions;


namespace PortfolioApi.Services;

public class TransactionService(AppDbContext db) : ITransactionService
{
    public async Task<TransactionResponse> CreateAsync(int portfolioId, CreateTransactionRequest request, CancellationToken ct)
    {
        var portfolioExists = await db.Portfolios.AnyAsync(p => p.Id == portfolioId, ct);
        if (!portfolioExists)
            throw new NotFoundException($"Portfolio {portfolioId} does not exist.");

        var symbol = Symbols.Normalize(request.Symbol);

        var asset = await db.Assets
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Symbol == symbol, ct) 
            ?? throw new NotFoundException($"Asset '{symbol}' does not exist.");

        var unitPrice = request.UnitPrice ?? asset.CurrentPrice;

        if (request.Type == TransactionType.Sell)
            throw new NotImplementedException();

        var holding = await db.Holdings
                .FirstOrDefaultAsync(h => h.PortfolioId == portfolioId && h.AssetId == asset.Id, ct);

        if (holding is null)
        {
            holding = new Holding { PortfolioId = portfolioId, AssetId = asset.Id };
            db.Holdings.Add(holding);
        }

        holding.AverageCost = MoneyMath.Round4(HoldingMath.AverageCostAfterBuy(
            holding.Quantity, holding.AverageCost, request.Quantity, unitPrice, request.Fee));

        holding.Quantity += request.Quantity;

        var transaction = new Transaction
        {
            PortfolioId = portfolioId,
            AssetId = asset.Id,
            Type = request.Type!.Value,
            Quantity = request.Quantity,
            UnitPrice = unitPrice,
            Fee = request.Fee,
            ExecutedAtUtc = DateTimeOffset.UtcNow
        };

        db.Transactions.Add(transaction);

        try
        {
            await db.SaveChangesAsync(ct);
        } catch (DbUpdateException ex) when(ex is DbUpdateConcurrencyException || ex.IsUniqueViolation())
        {
            throw new ConflictException("Concurrent change", "The holding was modified by another request. Please retry.");
        }

        return ToResponse(transaction, symbol, null);
    }

    private static TransactionResponse ToResponse(Transaction t, string symbol, decimal? realizedPnl)
    {
        var gross = t.Quantity * t.UnitPrice;
        return new TransactionResponse
        {
            Id = t.Id,
            PortfolioId = t.PortfolioId,
            Symbol = symbol,
            Type = t.Type,
            Quantity = t.Quantity,
            UnitPrice = t.UnitPrice,
            Fee = t.Fee,
            GrossAmount = MoneyMath.Round2(gross),
            NetAmount = MoneyMath.Round2(t.Type == TransactionType.Buy ? gross + t.Fee : gross - t.Fee),
            RealizedPnl = realizedPnl is null ? null : MoneyMath.Round2(realizedPnl.Value),
            ExecutedAtUtc = t.ExecutedAtUtc
        };
    }
}
