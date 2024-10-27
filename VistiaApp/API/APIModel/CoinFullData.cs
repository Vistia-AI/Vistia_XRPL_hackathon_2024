[System.Serializable]
public class CoinFullData
{
    public int id { get; set; }
    public string Symbol { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal Price_Change_Rate { get; set; }
    public decimal Volumn_24h { get; set; }
    public decimal Market_Cap { get; set; }
}