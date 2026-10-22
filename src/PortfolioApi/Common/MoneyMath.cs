namespace PortfolioApi.Common;

public static class MoneyMath
{
    public static decimal Round2(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    public static decimal Round4(decimal value) => Math.Round(value, 4, MidpointRounding.AwayFromZero);
}

