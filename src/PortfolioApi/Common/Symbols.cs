namespace PortfolioApi.Common;

public static class Symbols
{
    public static string Normalize(string symbol) => symbol.Trim().ToUpperInvariant();
}
