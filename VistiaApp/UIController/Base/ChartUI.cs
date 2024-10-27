using Cysharp.Threading.Tasks;
using UnityEngine;

public class ChartUI : UIBase
{
    [SerializeField] private Graph.BannerController bannerController;
    [SerializeField] private ChartController chartController;
    [SerializeField] private CoinInfo coinInfo;

    public override async UniTask OnShow(UIType previousUI, params object[] message)
    {
        await base.OnShow(previousUI, message);
        if (previousUI == UIType.Home)
        {
            string symbol = message[0].ToString();
            string name = message[1].ToString();
            chartController.ChangeSymbol(symbol);
            if (message.Length > 2)
            {
                Currency data = (Currency)message[2];
                bannerController.SetBanner(data);
            }
            else
            {
                bannerController.SetBanner(symbol);
            }
            coinInfo.SetData(symbol, name);
        }
    }

    public override async UniTask OnHide(UIType nextUI)
    {
        await base.OnHide(nextUI);
        chartController.OnExit();
    }
}
