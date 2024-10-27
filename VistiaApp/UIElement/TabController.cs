using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.U2D;

public class TabController : MonoBehaviour, IPoolable<TabController>
{
    [SerializeField] private Image icon;

    [SerializeField] private TMP_Text price;
    [SerializeField] private TMP_Text symbol;
    [SerializeField] private TMP_Text fullName;
    [SerializeField] private TMP_Text changeRate;
    [SerializeField] private Button button;
    [SerializeField] private Color32 upColor;
    [SerializeField] private Color32 downColor;

    private string tempSym;
    private decimal priceChange;
    private decimal priceValue = 0m;

    public string Code => tempSym;

    private Action<TabController> returnAction;

    public void Initialize(Action<TabController> returnAction)
    {
        this.returnAction = returnAction;
    }

    public void ReturnToPool()
    {
        this.returnAction(this);
    }

    public void Start()
    {
        button.onClick.AddListener(ViewChart);
    }

    private async void ViewChart()
    {
        Currency currency = new Currency();
        currency.Name = this.fullName.text;
        currency.Symbol = tempSym;
        currency.Percent_Change_24h = priceChange;
        currency.Price = priceValue;
        await UIManager.Instance.ChangePageAsync(UIType.Chart, tempSym.GetSymbolWithoutXRP(), fullName.text, currency);
    }

    internal void SetData(CoinPrice coinData)
    {
        priceChange = coinData.Price_Change_Percent;
        priceValue = coinData.Price;
        tempSym = coinData.Coin.GetSymbolWithoutXRP();
        this.symbol.text = tempSym + "/XRP";
        this.price.text = "$" + coinData.Price.ToString("0.######");
        if (priceChange >= 0)
        {
            this.changeRate.text = "+" + coinData.Price_Change_Percent.ToString("0.#####") + "%";
            this.changeRate.color = upColor;
        }
        else
        {
            this.changeRate.text = coinData.Price_Change_Percent.ToString("0.#####") + "%";
            this.changeRate.color = downColor;
        }
        //AlchemistAPIHandle.Instance.GetTokenSprite(tempSym, OnGetSprite);
    }

    internal void SetIcon(TokenInfo tokenInfo)
    {
        this.fullName.text = tokenInfo.name;

        icon.sprite = tokenInfo.image_sprite;
    }

    private void OnGetSprite(Sprite sprite)
    {
        icon.sprite = sprite;
    }
}
