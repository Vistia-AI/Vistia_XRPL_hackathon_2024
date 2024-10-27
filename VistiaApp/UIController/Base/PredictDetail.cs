using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PredictDetail : UIBase
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text coinPair;
    [SerializeField] private TMP_Text currentPrice;
    [SerializeField] private TMP_Text predictPrice;
    [SerializeField] private Button tradeBtn;
    [SerializeField] private BannerController banner;
    [SerializeField] private PredictChartController predictChartController;

    void Start()
    {
        tradeBtn.onClick.AddListener(ShowTradePopup);
    }

    private void ShowTradePopup()
    {
        throw new NotImplementedException();
    }

    public override async UniTask OnShow(UIType previousUI, params object[] message)
    {
        await base.OnShow(previousUI, message);
        if (previousUI == UIType.AI)
        {
            coinPair.text = message[0].ToString() + "/XRP";
            icon.sprite = (Sprite)message[1];
            currentPrice.text = message[2].ToString();
            predictPrice.text = message[3].ToString();
            banner.SetContentByName(message[0].ToString());

            predictChartController.ChangeSymbol(message[0].ToString());
        }
    }

    public override async UniTask OnHide(UIType nextUI)
    {
        await base.OnHide(nextUI);
        predictChartController.OnExit();
    }
}
