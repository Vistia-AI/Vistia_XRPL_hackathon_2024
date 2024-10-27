using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatisticPopup : PopupBase
{
    [SerializeField] private RectTransform mainContentRect;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text tradingPair;
    [SerializeField] private TMP_Text signal;
    [SerializeField] private TMP_Text indicator;
    [SerializeField] private TMP_Text entryPoint;
    [SerializeField] private TMP_Text _DCA1;
    [SerializeField] private TMP_Text _DCA2;
    [SerializeField] private TMP_Text targetPoint;
    [SerializeField] private TMP_Text stopLoss;
    [SerializeField] private Button saveBtn;

    [SerializeField] private Color32 soldColor;
    [SerializeField] private Color32 boughtColor;


    private float yValue;
    private Tween tween;

    public RectTransform MainContentRect { get => mainContentRect; set => mainContentRect = value; }
    public float YValue { get => yValue; set => yValue = value; }
    public Tween Tween { get => tween; set => tween = value; }

    private void Awake()
    {
        saveBtn.onClick.AddListener(OnSave);
        yValue = -mainContentRect.rect.height;
        mainContentRect.offsetMax = new Vector2(0, yValue);
        closeButton.onClick.AddListener(OnClose);
    }

    private void OnEnable()
    {
        tween = DOTween.To(() => yValue, x => mainContentRect.offsetMax = new Vector2(0, x), 0, 0.5f)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                tween = null;
            });

    }

    public void SetData(StockData stockData, Signal signal)
    {
        string heatMapType = AlchemistAPIHandle.Instance.HeatMapType.ToString();
        switch (signal)
        {
            case Signal.Oversold:
                this.signal.text = "Signal: <color=#008B38>Oversold</color>";
                indicator.text = $"Indicator: <b><color=blue>{heatMapType} ~ {stockData.Rsi.ToString("F2")}</color></b>";
                tradingPair.text = stockData.Symbol;
                tradingPair.color = soldColor;
                entryPoint.text = "$" + stockData.High.ToString("G29");
                _DCA1.text = "$" + ((stockData.High + stockData.Low) / 2m).ToString("G29");
                _DCA2.text = "$" + stockData.Low.ToString("G29");
                targetPoint.text = "$" + (stockData.High * 1.05m).ToString();
                stopLoss.text = "$" + (stockData.Low * 0.95m).ToString("G29");
                break;
            case Signal.Overbought:
                this.signal.text = "Signal: <color=#E10000>Overbought</color>";
                indicator.text = $"Indicator: <b><color=blue>{heatMapType} ~ {stockData.Rsi.ToString("F2")}</color></b>";
                tradingPair.text = stockData.Symbol;
                tradingPair.color = boughtColor;
                entryPoint.text = "$" + stockData.Low.ToString("G29");
                _DCA1.text = "$" + ((stockData.High + stockData.Low) / 2m).ToString("G29");
                _DCA2.text = "$" + stockData.High.ToString("G29");
                targetPoint.text = "$" + (stockData.Low * 0.95m).ToString("G29");
                stopLoss.text = "$" + (stockData.High * 1.05m).ToString("G29");
                break;
        }
    }

    private void OnClose()
    {
        if (tween != null) return;
        tween = DOTween.To(() => 0, x => mainContentRect.offsetMax = new Vector2(0, x), yValue, 0.5f)
            .SetEase(Ease.InCubic).OnComplete(() =>
            {
                gameObject.SetActive(false);
                tween = null;
            });
    }

    private void OnSave()
    {
        // Save the data
        OnClose();
    }

    public async UniTask OnCloseAsync()
    {
        if (tween != null) return;
        tween = DOTween.To(() => 0, x => mainContentRect.offsetMax = new Vector2(0, x), yValue, 0.5f)
            .SetEase(Ease.InCubic).OnComplete(() =>
            {
                gameObject.SetActive(false);
                tween = null;
            });
        await UniTask.Delay(500);
    }
}

public enum Signal
{
    Overbought,
    Oversold,
}
