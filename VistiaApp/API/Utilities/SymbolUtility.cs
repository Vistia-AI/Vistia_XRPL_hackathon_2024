using System;

public static class SymbolUtility
{
    public static string GetSymbolWithXRP(this string symbol, bool isUpperCase = true)
    {
        if (!symbol.Contains("XRP", StringComparison.OrdinalIgnoreCase))
            symbol = symbol + "XRP";

        return isUpperCase ? symbol.ToUpper() : symbol;
    }

    public static string GetSymbolWithoutXRP(this string symbol, bool isUpperCase = true)
    {
        if (symbol.Contains("XRP", StringComparison.OrdinalIgnoreCase))
            symbol = symbol.Substring(0, symbol.Length - 3);

        return isUpperCase ? symbol.ToUpper() : symbol;
    }
}
