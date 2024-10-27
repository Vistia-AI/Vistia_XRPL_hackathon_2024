using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinInfo : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField]
    private TMP_Text symbol;
    [SerializeField] private TMP_Text fullName;

    public void SetData(string symbol, string name)
    {
        this.symbol.text = symbol;
        this.fullName.text = name;
        AlchemistAPIHandle.Instance.GetCurrencyLogo(symbol, OnCurrencyLogoReceived);
    }

    private void OnCurrencyLogoReceived(string symbol, Sprite sprite)
    {
        this.symbol.text = symbol;
        image.sprite = sprite;
    }
}
