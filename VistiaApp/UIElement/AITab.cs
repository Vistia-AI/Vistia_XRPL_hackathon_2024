using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AITab : MonoBehaviour, IPoolable<AITab>
{
    [SerializeField] private Button thisBtn;
    [SerializeField] private Image iconImg;
    [SerializeField] private TMP_Text nameCoinTxt;
    [SerializeField] private TMP_Text priceChangeTxt;
    [SerializeField] private TMP_Text priceCurrentTxt;
    [SerializeField] private TMP_Text timeTxt;
    [SerializeField] private TMP_Text predictionTxt;
    [SerializeField] private Image frameImg;
    [SerializeField] private Image arrowImg;

    [SerializeField] private Color32 upColor;
    [SerializeField] private Color32 downColor;
    [SerializeField] private Color32 upTxtColor;
    [SerializeField] private Color32 downTxtColor;

    private string symbol;
    private Action<AITab> returnAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.thisBtn.onClick.AddListener(AITabClick);
    }

    private async void AITabClick()
    {
        UIManager.Instance.ChangePageAsync(UIType.PredictDetail, symbol, iconImg.sprite, priceCurrentTxt.text, priceChangeTxt.text);
    }

    public void SetContent(PredictionData data)
    {
        this.symbol = SymbolUtility.GetSymbolWithoutXRP(data.Symbol);
        this.nameCoinTxt.text = symbol;
        TimeZoneInfo tzinfo = TimeZoneInfo.Local;
        DateTime temp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(data.Target_Time)).DateTime;
        this.timeTxt.text = string.Format("{0:t}", TimeZoneInfo.ConvertTimeFromUtc(temp, tzinfo));
        this.predictionTxt.text = data.Price_Change.ToString("0.######") + "%";
        this.priceChangeTxt.text = "$" + data.Prediction.ToString("0.######");
        this.priceCurrentTxt.text = "$" + data.Price.ToString("0.######");
        if (data.Price_Change >= 0m)
        {
            this.frameImg.color = upColor;
            this.predictionTxt.color = upTxtColor;
            this.arrowImg.color = upTxtColor;
            this.arrowImg.transform.DOLocalRotate(new Vector3(0f, 0f, -90f), 0f, RotateMode.Fast);
        }
        else
        {
            this.frameImg.color = downColor;
            this.predictionTxt.color = downTxtColor;
            this.arrowImg.color = downTxtColor;
            this.arrowImg.transform.DOLocalRotate(new Vector3(0f, 0f, 90f), 0f, RotateMode.Fast);
        }
        AlchemistAPIHandle.Instance.GetCurrencyLogo(symbol, OnGetSprite);
    }

    private void OnGetSprite(string symbol, Sprite sprite)
    {
        iconImg.sprite = sprite;
        nameCoinTxt.text = symbol + "/XRP";
    }

    public void Initialize(Action<AITab> returnAction)
    {
        this.returnAction = returnAction;
    }

    public void ReturnToPool()
    {
        this.returnAction(this);
    }
}
