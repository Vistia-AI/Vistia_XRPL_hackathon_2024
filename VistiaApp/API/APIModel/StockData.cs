[System.Serializable]
public class StockData
{
    public string Symbol { get; set; }
    public decimal Rsi { get; set; }
    public decimal Close { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public string DateCreated { get; set; }
}
