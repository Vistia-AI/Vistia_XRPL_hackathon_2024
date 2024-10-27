using System.Collections.Generic;
[System.Serializable]
public class FibonacciInfo
{
    public string OriginalSymbol { get; set; }
    public string OriginalStartDate { get; set; }
    public string OriginalEndDate { get; set; }
    public List<decimal> OriginalPrices { get; set; }
    public List<decimal> OriginalFibonacci { get; set; }
    public List<string> SimilarSymbols { get; set; }
    public List<string> SimilarStartDates { get; set; }
    public List<string> SimilarEndDates { get; set; }
    public List<List<double>> SimilarPrices { get; set; }
    public List<List<double>> SimilarFibonacci { get; set; }
}
