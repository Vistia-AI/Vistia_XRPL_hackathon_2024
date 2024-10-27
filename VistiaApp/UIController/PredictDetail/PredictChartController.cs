using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using LitMotion;
using Graph;
using Cysharp.Threading.Tasks;
using System.Collections;
using TMPro;

public class PredictChartController : Singleton<PredictChartController>
{
    [Header("Preferences")]
    [SerializeField] private string symbol = "BTCUSDT";
    [SerializeField] private Interval interval = Interval.One_Day;
    [SerializeField] private OpenTime openTime = OpenTime.One_Year_Ago;
    [SerializeField] private TMP_Text price;
    [SerializeField] private TMP_Text predictPrice;
    [SerializeField] private TMP_Text offSet;
    [SerializeField] private Image dot;
    [SerializeField] private float transitionTime = .5f;

    [Header("Chart")]
    [SerializeField]
    private Chart.ChartV2<PredictChartData, PredictChartData, decimal, long> close_price_chart
        = new Chart.ChartV2<PredictChartData, PredictChartData, decimal, long>(
            item1 => item1.close,
            item2 => item2.close_time
            );
    /*    [SerializeField]
        private Chart.ChartV2<PredictChartData, PredictChartData, decimal, long> predict_close_price_chart
            = new Chart.ChartV2<PredictChartData, PredictChartData, decimal, long>(
                item1 => item1.close_predict,
                item2 => item2.close_time
                );*/

    [Header("Axis")]
    [SerializeField] private TimePointController timePointController;
    [SerializeField] private PricePointController pricePointController;

    [Header("Container")]
    [SerializeField] private RectTransform chartContainer;
    [SerializeField]
    private RectTransform parentRect;

    [Header("Slider")]
    [SerializeField] private Slider slider;
    [SerializeField] private Text label;

    [Header("Lines")]
    [SerializeField]
    private Transform horizontalLine_normal;
    /*[SerializeField]
    private Transform horizontalLine_predict;*/
    [SerializeField]
    private Transform verticalLine;

    [Header("Color")]
    [SerializeField] private Color upColor = Color.green;
    [SerializeField] private Color downColor = Color.red;

    private MotionHandle horizontalLine_normal_motion;
    //private MotionHandle horizontalLine_predict_motion;
    private MotionHandle verticalLine_motion;

    private bool lineControlable = true;
    private bool changable;
    public bool Changable { get => changable; set => changable = value; }

    private List<Point> predictPoints = new List<Point>();

    private Point lastPredictPoint;

    private Predict.IntervalButton currentSelectBtn;

    public Predict.IntervalButton CurrentSelectBtn { get => currentSelectBtn; set => currentSelectBtn = value; }

    private void Awake()
    {
        //chartContainer.anchorMin = chartContainer.anchorMax = Vector2.one * .5f;
        Debug.Log(parentRect.rect.size);
        Debug.Log(chartContainer.rect.size);
        Canvas.ForceUpdateCanvases();
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log(parentRect.rect.size);
        Debug.Log(chartContainer.rect.size);
        chartContainer.sizeDelta = new Vector2(parentRect.rect.size.x, parentRect.rect.size.y);
        chartContainer.localPosition = Vector3.zero;
        slider.onValueChanged.AddListener(OnSliderChange);
        close_price_chart.onFinishDraw += OnChartFinishDraw;
        close_price_chart.onCurrentDataChange += OnCurrentDataChange;
        close_price_chart.Init();
        //predict_close_price_chart.onFinishDraw += OnPredictChartFinishDraw;
        //predict_close_price_chart.Init();
        horizontalLine_normal_motion = LMotion.Create(0, 0, 0).RunWithoutBinding();
        //horizontalLine_predict_motion = LMotion.Create(0, 0, 0).RunWithoutBinding();
        verticalLine_motion = LMotion.Create(0, 0, 0).RunWithoutBinding();

        yield return new WaitForSeconds(1f);
    }

    [ContextMenu("IncreaseXScale")]
    public void IncreaseXScale()
    {
        Vector3 scale = close_price_chart.ChangeScaleX(0.1f);

        foreach (var item in predictPoints)
        {
            item.RecalculateScale(scale);
        }
    }

    [ContextMenu("DecreaseXScale")]
    public void DecreaseXScale()
    {
        Vector3 scale = close_price_chart.ChangeScaleX(-0.1f);

        foreach (var item in predictPoints)
        {
            item.RecalculateScale(scale);
        }
    }


    private void OnCurrentDataChange(Tuple<PredictChartData, PredictChartData> data)
    {
        if (!lineControlable) return;
        decimal price = data.Item1.close;
        long time = data.Item2.close_time;
        label.text = $"{DateTimeOffset.FromUnixTimeSeconds(time)}";
        pricePointController.SetCurrentPricePos(price);
    }

    /*private void OnPredictChartFinishDraw()
    {
        Tuple<PredictChartData, PredictChartData, Point> predict_data = predict_close_price_chart.GetDataAtIndex(0);
        Point predict_point = predict_data.Item3;
        if (predict_point != null)
        {
            if (horizontalLine_predict_motion.IsActive()) horizontalLine_predict_motion.Cancel();
            horizontalLine_predict_motion = horizontalLine_predict.ChangePosition(new Vector3(horizontalLine_predict.position.x, predict_point.transform.position.y), transitionTime, null, Ease.InOutCubic);
        }
    }*/

    private void OnChartFinishDraw()
    {
        Tuple<PredictChartData, PredictChartData, Point> data = close_price_chart.GetDataAndHighLightPointAtIndex(0);

        if (data == null) return;

        Point point = data.Item3;
        string time = DateTimeOffset.FromUnixTimeSeconds(data.Item2.close_time).ToString();

        AddPredictPoint(
            () =>
            {
                SelectPredictPoint(0, data.Item1);
                label.text = time;
            }).Forget();

        if (point != null)
        {
            if (horizontalLine_normal_motion.IsActive()) horizontalLine_normal_motion.Cancel();
            if (verticalLine_motion.IsActive()) verticalLine_motion.Cancel();
            horizontalLine_normal_motion = horizontalLine_normal.ChangePosition(new Vector3(horizontalLine_normal.position.x, point.transform.position.y), transitionTime, null, Ease.InOutCubic);
            verticalLine_motion = verticalLine.ChangePosition(new Vector3(point.transform.position.x, verticalLine.position.y), transitionTime, null, Ease.InOutCubic);
        }
        pricePointController.SetCurrentPricePos(data.Item1.close_time, transitionTime);
        slider.ChangeValue(0f, transitionTime,
            (Action)(() =>
            {
                slider.interactable = true;
                lineControlable = true;
                close_price_chart.GetDataAndHighLightPointAtIndex(0);
                //predict_close_price_chart.GetDataAtIndex(0);
            }),
             Ease.InOutCubic);
        changable = true;
    }


    private void OnSliderChange(float value)
    {
        if (!lineControlable || close_price_chart.IsDrawing) return;

        var data = close_price_chart.GetDataAndHighLightPointAtIndex((int)value);

        if (data == null) return;
        Point point = data.Item3;

        SelectPredictPoint((int)value, data.Item1);

        if (point != null)
        {
            if (horizontalLine_normal_motion.IsActive()) horizontalLine_normal_motion.Cancel();
            //if (horizontalLine_predict_motion.IsActive()) horizontalLine_predict_motion.Cancel();
            if (verticalLine_motion.IsActive()) verticalLine_motion.Cancel();

            /*horizontalLine_normal.position = new Vector3(horizontalLine_normal.position.x, point.transform.position.y, 0f);
            horizontalLine_predict.position = new Vector3(horizontalLine_predict.position.x, predict_point.transform.position.y, 0f);
            verticalLine.position = new Vector3(point.transform.position.x, verticalLine.position.y, 0f);*/

            horizontalLine_normal_motion = horizontalLine_normal.ChangePosition(new Vector3(horizontalLine_normal.position.x, point.transform.position.y, 0f), transitionTime, null, Ease.InOutCubic);
            //horizontalLine_predict_motion = horizontalLine_predict.ChangePosition(new Vector3(horizontalLine_predict.position.x, predict_point.transform.position.y, 0f), transitionTime, null, Ease.InOutCubic);
            verticalLine_motion = verticalLine.ChangePosition(new Vector3(point.transform.position.x, verticalLine.position.y, 0f), transitionTime, null, Ease.InOutCubic);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="data"></param>
    private void SelectPredictPoint(int index, PredictChartData data)
    {
        lastPredictPoint?.SetThickness(25f);
        predictPoints[index].SetThickness(40f);
        lastPredictPoint = predictPoints[index];
        lastPredictPoint.transform.SetAsLastSibling();

        decimal price = data.close;
        decimal predictPrice = data.close_predict;
        float offset_percent = (float)((predictPrice - price) / price * 100);

        this.price.text = "Actual: $" + price.ToString();
        this.predictPrice.text = "Predict: $" + predictPrice.ToString();
        this.offSet.text = "Declination: " + offset_percent.ToString() + "%";
        if (data.pred_trend.Equals(data.trend)) dot.color = upColor;
        else dot.color = downColor;
    }

    public void GetGraphData()
    {
        changable = false;
        AlchemistAPIHandle.Instance.PredictValidateChart(symbol, OnGetData);
    }

    private void OnGetData(List<PredictChartData> data)
    {
        ResetPredictPoint();
        OnGetDataAsync(data).Forget();
    }

    private async UniTaskVoid OnGetDataAsync(List<PredictChartData> newData)
    {
        if (newData == null || newData.Count == 0)
        {
            Debug.LogError("No data to draw");
            return;
        }
        decimal maxY = decimal.MinValue;
        decimal minY = decimal.MaxValue;
        List<Tuple<PredictChartData, PredictChartData>> temp = new List<Tuple<PredictChartData, PredictChartData>>();
        foreach (var item in newData)
        {
            if (item.close > maxY)
            {
                maxY = item.close;
            }
            if (item.close_predict > maxY)
            {
                maxY = item.close_predict;
            }
            if (item.close < minY)
            {
                minY = item.close;
            }
            if (item.close_predict < minY)
            {
                minY = item.close_predict;
            }
            temp.Add(new Tuple<PredictChartData, PredictChartData>(item, item));
        }

        lineControlable = false;

        horizontalLine_normal.ChangeLocalPositionAsync(new Vector3(horizontalLine_normal.localPosition.x, close_price_chart.Height / 2f, 0f), .5f, null, Ease.InOutCubic).Forget();
        //horizontalLine_predict.ChangeLocalPositionAsync(new Vector3(horizontalLine_predict.localPosition.x, close_price_chart.Height / 2f, 0f), .5f, null, Ease.InOutCubic).Forget();
        verticalLine.ChangeLocalPositionAsync(new Vector3(close_price_chart.Width / 2f, verticalLine.localPosition.y, 0f), .5f, null, Ease.InOutCubic).Forget();

        await UniTask.Delay(500);

        slider.interactable = false;
        slider.maxValue = newData.Count - 1;
        Debug.Log("Max: " + maxY + " Min: " + minY);
        Tuple<decimal, decimal> tuple = pricePointController.SetPricePoints(maxY, minY);

        maxY = tuple.Item2;
        minY = tuple.Item1;

        float xScale = close_price_chart.Width / (newData.Count - 1);
        float yScale = close_price_chart.Height / (float)(maxY - minY);

        timePointController.SetTimePoint(newData[0].close_time, newData[^1].close_time, OpenTime.One_Day_Ago, 5, false);
        timePointController.SetTimePointsPos(close_price_chart.Width);
        pricePointController.SetPricePointsPos(yScale, minY);

        close_price_chart.Draw(temp, xScale, yScale, minY);
        //predict_close_price_chart.Draw(temp, xScale, yScale, minY);
    }

    /*public void ChangeSetting(Interval interval, Predict.IntervalButton btn)
    {
        if (close_price_chart.IsDrawing) return;
        if (this.interval == interval) return;
        currentSelectBtn?.OnDeselect();
        currentSelectBtn = btn;
        currentSelectBtn?.OnSelect();
        this.interval = interval;
        GetGraphData();
    }*/

    public void OnExit()
    {
        if (horizontalLine_normal_motion.IsActive())
            horizontalLine_normal_motion.Complete();
        if (verticalLine_motion.IsActive())
            verticalLine_motion.Complete();
    }

    public void ChangeSymbol(string symbol)
    {
        if (close_price_chart.IsDrawing || this.symbol.Equals(symbol)) return;
        this.symbol = symbol;
        GetGraphData();
    }

    private void ResetPredictPoint()
    {
        foreach (var item in predictPoints)
        {
            item.ReturnToPool();
        }

        predictPoints.Clear();
    }
    private async UniTask AddPredictPoint(Action callback)
    {
        var chartData = close_price_chart.Data;

        for (int i = 0; i < chartData.Count; i++)
        {
            SetPredictPointData(chartData[i].Item1, i, 25f);
            await UniTask.Delay(1);
        }

        callback?.Invoke();

        lastPredictPoint = predictPoints[0];
        lastPredictPoint.transform.SetAsLastSibling();
    }

    private void SetPredictPointData(PredictChartData data, int index, float size)
    {
        bool isUp = data.pred_trend.Equals(data.trend);

        // Get Previous Data Sample
        //Tuple<PredictChartData, PredictChartData> previousData = close_price_chart.GetDataAtIndex(index - 1);

        Point temp = close_price_chart.DrawCustomPoint(index, data.close_predict, isUp ? upColor : downColor, size);
        predictPoints.Add(temp);
    }
}
