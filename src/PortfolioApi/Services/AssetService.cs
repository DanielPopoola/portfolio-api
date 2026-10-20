using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Domain;
using PortfolioApi.Dtos;
using PortfolioApi.Exceptions;


namespace PortfolioApi.Services;

public class AssetService(AppDbContext db) : IAssetService
{
    private static readonly Expression<Func<Asset, AssetResponse>> ToResponse = a => new AssetResponse
    {
        Id = a.Id,
        Symbol = a.Symbol,
        Name = a.Name,
        AssetType = a.AssetType,
        CurrentPrice = a.CurrentPrice,
        PriceUpdatedAtUtc = a.PriceUpdatedAtUtc
    };

    private static readonly Func<Asset, AssetResponse> MapToResponse = ToResponse.Compile();
    public async Task<AssetResponse> CreateAsync(CreateAssetRequest request, CancellationToken ct)
    {
        var asset = new Asset
        {
            Symbol = request.Symbol.Trim().ToUpperInvariant(),
            Name = request.Name,
            AssetType = request.AssetType!.Value,
            CurrentPrice = request.CurrentPrice,
            PriceUpdatedAtUtc = DateTimeOffset.UtcNow
        };

        db.Assets.Add(asset);

        try
        {
            await db.SaveChangesAsync(ct); 
        } catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            throw new ConflictException("Duplicate symbol", $"Asset '{asset.Symbol}' already exists.");
        }

        return MapToResponse(asset);
    }

}
