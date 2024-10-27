[System.Serializable]
public class PredictChartData
{
    public long close_time { get; set; }
    public decimal close_predict { get; set; }
    public decimal open { get; set; }
    public decimal close { get; set; }
    public decimal high { get; set; }
    public decimal low { get; set; }
    public string pred_trend { get; set; }
    public string trend { get; set; }
}
