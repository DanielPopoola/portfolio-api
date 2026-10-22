using System.Globalization;
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

        var holding = await db.Holdings
                .FirstOrDefaultAsync(h => h.PortfolioId == portfolioId && h.AssetId == asset.Id, ct);

        decimal? realizedPnl = null;

        if (request.Type == TransactionType.Buy)
        {
            if (holding is null)
            {
                holding = new Holding { PortfolioId = portfolioId, AssetId = asset.Id };
                db.Holdings.Add(holding);
            }

            holding.AverageCost = MoneyMath.Round4(HoldingMath.AverageCostAfterBuy(
                holding.Quantity, holding.AverageCost, request.Quantity, unitPrice, request.Fee));

            holding.Quantity += request.Quantity;
        }
        else
        {
            var held = holding?.Quantity ?? 0;

            if (holding is null || held < request.Quantity)
                throw new ConflictException("Insufficient holdings",
                    $"Cannot sell {Format(request.Quantity)} {symbol}; portfolio {portfolioId} holds {Format(held)}.");

            realizedPnl = HoldingMath.RealizedPnlOnSell(request.Quantity, unitPrice, request.Fee, holding.AverageCost);
            holding.RealizedPnl = MoneyMath.Round2(holding.RealizedPnl + realizedPnl.Value);
            holding.Quantity -= request.Quantity;
            if (holding.Quantity == 0)
                holding.AverageCost = 0;

        }

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

        return ToResponse(transaction, symbol, realizedPnl);
    }

    public async Task<PagedResponse<TransactionResponse>> ListAsync(int portfolioId, TransactionListQuery q, CancellationToken ct)
    {
        var portfolioExists = await db.Portfolios.AnyAsync(p => p.Id == portfolioId, ct);
        if (!portfolioExists)
            throw new NotFoundException($"Portfolio {portfolioId} does not exist.");

        var query = db.Transactions.AsNoTracking().Where(t => t.PortfolioId == portfolioId);

        if (q.Symbol is not null)
            query = query.Where(t => t.Asset.Symbol == Symbols.Normalize(q.Symbol));
        if (q.Type is not null)
            query = query.Where(t => t.Type == q.Type.Value);
        if (q.From is not null)
        {
            var fromUtc = new DateTimeOffset(q.From.Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            query = query.Where(t => t.ExecutedAtUtc >= fromUtc);
        }
        if (q.To is not null)
        {
            var toUtc = new DateTimeOffset(q.To.Value.AddDays(1).ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
            query = query.Where(t => t.ExecutedAtUtc <  toUtc);
        }

        var total = await query.CountAsync(ct);

        var rows = await query
            .OrderByDescending(t => t.ExecutedAtUtc).ThenByDescending(t => t.Id)
            .Skip((q.Page - 1) * q.PageSize).Take(q.PageSize)
            .Select(t => new { Transaction = t, Symbol = t.Asset.Symbol })
            .ToListAsync(ct);

        var response = rows.Select(row => ToResponse(row.Transaction, row.Symbol, null)).ToList();

        return new PagedResponse<TransactionResponse>
        {
            Items = response,
            Page = q.Page,
            PageSize = q.PageSize,
            TotalCount = total,
        };
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

    private static string Format(decimal value)
        => value.ToString("0.######", CultureInfo.InvariantCulture);
}
