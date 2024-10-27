[System.Serializable]
public class CoinPrice
{
    public string Coin { get; set; }
    public int Time { get; set; }
    public decimal Price { get; set; }
    public decimal Price_Change { get; set; }
    public decimal Price_Change_Percent { get; set; }
}
