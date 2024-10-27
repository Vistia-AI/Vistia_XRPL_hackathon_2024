using System;

public class PredictionData
{
    public string Symbol { get; set; }
    public string Update_Time { get; set; }
    public string Target_Time { get; set; }
    public decimal Price { get; set; }
    public decimal Prediction { get; set; }
    public decimal Price_Change { get; set; }
}