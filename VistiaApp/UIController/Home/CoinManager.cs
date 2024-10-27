using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    [SerializeField] private TabController iconHandlerPrefab;
    [SerializeField] private RectTransform parentScrollRect;
    private Dictionary<string, TokenInfo> tokenInfoCache = new Dictionary<string, TokenInfo>();

    private List<TabController> iconHandle = new List<TabController>();
    private ObjectPool<TabController> tabPool;

    private void Start()
    {
        AlchemistAPIHandle.Instance.CoinDatasUpdatedStream += OnCoinDatasUpdated;
        AlchemistAPIHandle.Instance.OnGetTokenInfoEven += OnGetTokenInfo;
        tabPool = new ObjectPool<TabController>(iconHandlerPrefab, 0);
    }

    private void OnGetTokenInfo(Dictionary<string, TokenInfo> tokenInfos)
    {
        tokenInfoCache = tokenInfos;

        int count = iconHandle.Count;
        for (int i = 0; i < count; i++)
        {
            TabController tab = iconHandle[i];
            string code = tab.Code;
            if (tokenInfoCache.ContainsKey(code))
                tab.SetIcon(tokenInfoCache[code]);
        }
    }

    private void OnCoinDatasUpdated(List<CoinPrice> coinDatas)
    {
        int totalCoin = coinDatas.Count;

        if (iconHandle.Count < totalCoin)
        {
            int count = totalCoin - iconHandle.Count;
            for (int i = 0; i < count; i++)
            {
                TabController tab = tabPool.Pull(Vector3.zero, transform);
                tab.transform.localScale = Vector3.one;
                iconHandle.Add(tab);
            }
        }
        else if (iconHandle.Count > totalCoin)
        {
            int count = iconHandle.Count - totalCoin;
            for (int i = iconHandle.Count - 1, j = count; i >= totalCoin && count > 0; i--, j--)
            {
                iconHandle[i].ReturnToPool();
                iconHandle.RemoveAt(i);
            }
        }

        for (int i = 0; i < coinDatas.Count; i++)
        {
            TabController tab = iconHandle[i];
            CoinPrice coinPrice = coinDatas[i];
            tab.SetData(coinPrice);
            string code = coinPrice.Coin.GetSymbolWithoutXRP();
            if (tokenInfoCache.ContainsKey(code))
                tab.SetIcon(tokenInfoCache[code]);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(parentScrollRect);
    }

    private void OnDestroy()
    {
        AlchemistAPIHandle.Instance.CoinDatasUpdatedStream -= OnCoinDatasUpdated;
    }
}
