using System;

[Serializable]
public class Currency
{
    public string Id { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal Volume_24h { get; set; }
    public decimal Percent_Change_24h { get; set; }
    public decimal Market_Cap { get; set; }
    public override string ToString()
    {
        return $"Id: {Id}, Symbol: {Symbol}, Name: {Name}, Price: {Price}, Volumn24h: {Volume_24h}, PercentageChange24h: {Percent_Change_24h}, MarketCap: {Market_Cap}";
    }
}
