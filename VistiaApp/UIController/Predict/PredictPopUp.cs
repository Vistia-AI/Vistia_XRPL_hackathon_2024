using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PredictPopUp : PopupBase
{
    [SerializeField] private RectTransform mainContentRect;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text symbol;
    [SerializeField] private TMP_Text currentPrice;
    [SerializeField] private TMP_Text predictionPrice;
    [SerializeField] private TMP_Text numberOfTrades;
    [SerializeField] private TMP_Text totalWinTrades;
    [SerializeField] private TMP_Text totalLossTrades;
    [SerializeField] private TMP_Text highestGainRate;
    [SerializeField] private TMP_Text highestLossRate;
    [SerializeField] private Button binanceBtn;
    [SerializeField] private Button bybitBtn;
    [SerializeField] private Color32 soldColor;
    [SerializeField] private Color32 boughtColor;
    private float yValue;
    private Tween tween;

    public RectTransform MainContentRect { get => mainContentRect; set => mainContentRect = value; }
    public float YValue { get => yValue; set => yValue = value; }
    public Tween Tween { get => tween; set => tween = value; }

    private void Awake()
    {
        binanceBtn.onClick.AddListener(() => Application.OpenURL(""));
        bybitBtn.onClick.AddListener(() => Application.OpenURL(""));

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
