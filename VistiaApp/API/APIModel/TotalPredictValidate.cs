using System;

public class TotalPredictValidate
{
    public decimal mae { get; set; }
    public decimal avg_err_rate { get; set; }
    public decimal max_profit_rate { get; set; }
    public decimal max_loss_rate { get; set; }
    public decimal avg_profit_rate { get; set; }
    public decimal accuracy { get; set; }
    public int n_trade { get; set; }
    public int true_pred { get; set; }
    public int false_pred { get; set; }

}
