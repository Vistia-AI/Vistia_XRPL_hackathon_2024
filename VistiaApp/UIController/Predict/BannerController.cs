using LitMotion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BannerController : MonoBehaviour
{
    [SerializeField] private TMP_Text lossRate;
    [SerializeField] private TMP_Text winRate;
    [SerializeField] private TMP_Text highestLossRate;
    [SerializeField] private TMP_Text highestGainRate;
    [SerializeField] private Image winRateChart;
    [SerializeField] private TMP_Text totalTrades;
    [SerializeField] private float duration = 1f;

    int totalTradesCount = 0;
    float lossRateValue = 0;
    float winRateValue = 0;
    float winRateChartValue = 0;
    float highestLossRateValue = 0;
    float highestGainRateValue = 0;

    MotionHandle totalTradeAnim;
    MotionHandle lossRateAnim;
    MotionHandle winRateAnim;
    MotionHandle winRateChartAnim;
    MotionHandle highestLossRateAnim;
    MotionHandle highestGainRateAnim;

    private void Start()
    {
        AlchemistAPIHandle.Instance.AIPredictionValidateUpdatedStream += SetContent;

        totalTradeAnim = LMotion.Create(0, 0, 0).RunWithoutBinding();
        lossRateAnim = LMotion.Create(0, 0, 0).RunWithoutBinding();
        winRateAnim = LMotion.Create(0, 0, 0).RunWithoutBinding();
        winRateChartAnim = LMotion.Create(0, 0, 0).RunWithoutBinding();
        highestLossRateAnim = LMotion.Create(0, 0, 0).RunWithoutBinding();
        highestGainRateAnim = LMotion.Create(0, 0, 0).RunWithoutBinding();
    }
    public void SetContentByName(string name)
    {
        AlchemistAPIHandle.Instance.PredictValidate(name, "ONE_MONTH", SetContent);
    }
    private void SetContent(TotalPredictValidate data)
    {
        if (data == null) return;
        if (totalTradeAnim.IsActive()) totalTradeAnim.Cancel();
        if (lossRateAnim.IsActive()) lossRateAnim.Cancel();
        if (winRateAnim.IsActive()) winRateAnim.Cancel();
        if (winRateChartAnim.IsActive()) winRateChartAnim.Cancel();
        if (highestLossRateAnim.IsActive()) highestLossRateAnim.Cancel();
        if (highestGainRateAnim.IsActive()) highestGainRateAnim.Cancel();

        totalTradeAnim = LMotion.Create(totalTradesCount, data.n_trade, duration)
            .BindWithState(totalTrades, (value, target) => target.text = value.ToString());

        lossRateAnim = LMotion.Create(lossRateValue, (1 - (float)data.accuracy) * 100, duration)
            .BindWithState(lossRate, (value, target) => target.text = value.ToString("F2") + "%");

        winRateAnim = LMotion.Create(winRateValue, (float)data.accuracy * 100, duration)
            .BindWithState(winRate, (value, target) => target.text = value.ToString("F2") + "%");

        winRateChartAnim = LMotion.Create(winRateChartValue, (float)data.accuracy, duration)
            .BindWithState(winRateChart, (value, target) => target.fillAmount = value);

        highestLossRateAnim = LMotion.Create(highestLossRateValue, (float)data.max_loss_rate * 100, duration)
            .BindWithState(highestLossRate, (value, target) => target.text = value.ToString("F2") + "%");

        highestGainRateAnim = LMotion.Create(highestGainRateValue, (float)data.max_profit_rate * 100, duration)
            .BindWithState(highestGainRate, (value, target) => target.text = value.ToString("F2") + "%");

        totalTradesCount = data.n_trade;
        lossRateValue = (1 - (float)data.accuracy) * 100;
        winRateValue = (float)data.accuracy * 100;
        winRateChartValue = (float)data.accuracy;
        highestLossRateValue = (float)data.max_loss_rate * 100;
        highestGainRateValue = (float)data.max_profit_rate * 100;
    }
}
