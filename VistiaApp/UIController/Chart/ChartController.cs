using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using LitMotion;
using Graph;
using Cysharp.Threading.Tasks;
using System.Collections;

public class ChartController : Singleton<ChartController>
{
    [Header("Preferences")]
    [SerializeField] private string symbol = "BTCUSDT";
    [SerializeField] private Interval interval = Interval.One_Day;
    [SerializeField] private OpenTime openTime = OpenTime.One_Year_Ago;
    [SerializeField] private float transitionTime = .5f;

    [Header("Chart")]
    [SerializeField] private Chart.ChartV2<decimal, long, decimal, long> chartV2;

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
    private Transform horizontalLine;
    [SerializeField]
    private Transform verticalLine;

    private bool lineControlable = true;
    private bool changable;

    private Graph.IntervalButton currentSelectBtn;

    public Graph.IntervalButton CurrentSelectBtn { get => currentSelectBtn; set => currentSelectBtn = value; }
    public bool Changable { get => changable; set => changable = value; }

    private void Awake()
    {
        //chartContainer.anchorMin = chartContainer.anchorMax = Vector2.one * .5f;
        Debug.Log(parentRect.rect.size);
        Debug.Log(chartContainer.rect.size);
        Canvas.ForceUpdateCanvases();

        horizontalLineHandle = LMotion.Create(0, 0, 0).RunWithoutBinding();
        verticalLineHandle = LMotion.Create(0, 0, 0).RunWithoutBinding();
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log(parentRect.rect.size);
        Debug.Log(chartContainer.rect.size);
        chartContainer.sizeDelta = new Vector2(parentRect.rect.size.x, parentRect.rect.size.y);
        chartContainer.localPosition = Vector3.zero;
        slider.onValueChanged.AddListener(OnSliderChange);
        chartV2.onFinishDraw += OnChartFinishDraw;
        chartV2.onCurrentDataChange += OnCurrentDataChange;
        //chart.Start();
        chartV2.Init();
    }

    private void OnCurrentDataChange(Tuple<decimal, long> data)
    {
        if (!lineControlable) return;
        decimal price = data.Item1;
        long time = data.Item2;
        label.text = $"{DateTimeOffset.FromUnixTimeMilliseconds(time)}";
        pricePointController.SetCurrentPricePos(price);
    }

    MotionHandle horizontalLineHandle;
    MotionHandle verticalLineHandle;

    public void OnExit()
    {
        if (horizontalLineHandle.IsActive())
            horizontalLineHandle.Complete();
        if (verticalLineHandle.IsActive())
            verticalLineHandle.Complete();
    }

    private void OnChartFinishDraw()
    {
        Tuple<decimal, long, Point> data = chartV2.GetDataAndHighLightPointAtIndex(0);
        Point point = data.Item3;
        string time = DateTimeOffset.FromUnixTimeMilliseconds(data.Item2).ToString();
        if (point != null)
        {
            if (horizontalLineHandle.IsActive()) horizontalLineHandle.Cancel();
            if (verticalLineHandle.IsActive()) verticalLineHandle.Cancel();
            horizontalLineHandle = horizontalLine.ChangePosition(new Vector3(horizontalLine.position.x, point.transform.position.y), transitionTime, null, Ease.InOutCubic);
            verticalLineHandle = verticalLine.ChangePosition(new Vector3(point.transform.position.x, verticalLine.position.y), transitionTime, null, Ease.InOutCubic);
        }
        pricePointController.SetCurrentPricePos(data.Item1, transitionTime);
        slider.ChangeValue(0f, transitionTime,
            (Action)(() =>
            {
                slider.interactable = true;
                lineControlable = true;
                chartV2.GetDataAndHighLightPointAtIndex(0);
            }),
             Ease.InOutCubic);
        changable = true;
    }

    private void OnSliderChange(float value)
    {
        if (!lineControlable || chartV2.IsDrawing) return;
        Point point = chartV2.GetDataAndHighLightPointAtIndex((int)value).Item3;
        if (point != null)
        {
            if (horizontalLineHandle.IsActive()) horizontalLineHandle.Cancel();
            if (verticalLineHandle.IsActive()) verticalLineHandle.Cancel();
            horizontalLineHandle = horizontalLine.ChangePosition(new Vector3(horizontalLine.position.x, point.transform.position.y), transitionTime, null, Ease.InOutCubic);
            verticalLineHandle = verticalLine.ChangePosition(new Vector3(point.transform.position.x, verticalLine.position.y), transitionTime, null, Ease.InOutCubic);
        }
    }

    public void GetGraphData()
    {
        changable = false;
        AlchemistAPIHandle.Instance.GetHistoricalKlinesAsync(symbol, OnGetData);
        AlchemistAPIHandle.Instance.PredictValidateChart(symbol, null);
    }

    private void OnGetData(List<Tuple<decimal, long>> newData, decimal maxY, decimal minY)
    {
        OnGetDataAsync(newData, maxY, minY).Forget();
    }

    private async UniTaskVoid OnGetDataAsync(List<Tuple<decimal, long>> newData, decimal maxY, decimal minY)
    {
        if (newData == null || newData.Count == 0)
        {
            Debug.LogError("No data to draw");
            return;
        }

        lineControlable = false;

        horizontalLine.ChangeLocalPositionAsync(new Vector3(horizontalLine.localPosition.x, chartV2.Height / 2f, 0f), .5f, null, Ease.InOutCubic).Forget();
        verticalLine.ChangeLocalPositionAsync(new Vector3(chartV2.Width / 2f, verticalLine.localPosition.y, 0f), .5f, null, Ease.InOutCubic).Forget();

        await UniTask.Delay(500);

        slider.interactable = false;
        slider.maxValue = newData.Count - 1;

        Tuple<decimal, decimal> tuple = pricePointController.SetPricePoints(maxY, minY);

        maxY = tuple.Item2;
        minY = tuple.Item1;

        float xScale = chartV2.Width / (newData.Count - 1);
        float yScale = chartV2.Height / (float)(maxY - minY);

        timePointController.SetTimePoint(newData[0].Item2, newData[^1].Item2, openTime, 4, false);
        timePointController.SetTimePointsPos(chartV2.Width);
        pricePointController.SetPricePointsPos(yScale, minY);

        chartV2.Draw(newData, xScale, yScale, minY);
    }

    public void ChangeSetting(Interval interval, OpenTime openTime, Graph.IntervalButton btn)
    {
        if (chartV2.IsDrawing) return;
        if (this.interval == interval && this.openTime == openTime) return;
        currentSelectBtn?.OnDeselect();
        currentSelectBtn = btn;
        currentSelectBtn?.OnSelect();
        this.interval = interval;
        this.openTime = openTime;
        GetGraphData();
    }

    public void ChangeSymbol(string symbol)
    {
        if (chartV2.IsDrawing || this.symbol.Equals(symbol)) return;
        this.symbol = symbol;
        GetGraphData();
    }
}
