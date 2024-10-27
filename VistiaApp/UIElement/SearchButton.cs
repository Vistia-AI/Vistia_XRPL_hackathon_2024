using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SearchButton : MonoBehaviour, IPoolable<SearchButton>
{
    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private TMP_Text volTxt;
    [SerializeField] private TMP_Text priceTxt;
    [SerializeField] private TMP_Text rateTxt;
    [SerializeField] private Image iconImg;
    [SerializeField] private Button thisButton;
    private bool available = true;
    private string coinName;
    private string id;

    private Action<SearchButton> returnAction;

    private Currency data;

    public void Initialize(Action<SearchButton> returnAction)
    {
        this.returnAction = returnAction;
    }

    public void ReturnToPool()
    {
        this.returnAction(this);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thisButton.onClick.AddListener(ViewChart);
    }

    private async void ViewChart()
    {
        if (!available) return;
        await UIManager.Instance.ChangePageAsync(UIType.Chart, coinName.GetSymbolWithoutXRP(), data.Name, data);
    }

    public void SetButton(Currency data)
    {
        this.id = data.Id;
        this.data = data;
        iconImg.sprite = null;
        this.coinName = data.Symbol;
        this.nameTxt.text = $"{coinName}";
        this.volTxt.text = "Vol 24h: $" + (data.Volume_24h / 1000000).ToString("F2") + "M";
        this.priceTxt.text = "$" + data.Price.ToString();
        if (data.Percent_Change_24h >= 0)
        {
            this.rateTxt.text = "+" + data.Percent_Change_24h.ToString() + "%";
            this.rateTxt.color = Color.green;
        }
        else
        {
            this.rateTxt.text = data.Percent_Change_24h.ToString() + "%";
            this.rateTxt.color = Color.red;

        }
        AlchemistAPIHandle.Instance.GetCurrencyLogo(coinName, OnGetSprite);
    }

    private void OnGetSprite(string symbol, Sprite sprite)
    {
        nameTxt.text = symbol;
        iconImg.sprite = sprite;
    }
}
