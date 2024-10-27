using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;

namespace Analysis
{
    public class BannerController : MonoBehaviour
    {
        [SerializeField] private TMP_Text coinPair;
        [SerializeField] private TMP_Text price;
        [SerializeField] private TMP_Text change;
        [SerializeField] private float duration = .5f;
        [SerializeField] private Color32 downColor;
        [SerializeField] private Color32 upColor;

        private decimal priceValue = 0m;
        private decimal changeValue = 0m;
        private string symbol;
        [SerializeField]
        private List<CoinPrice> data;

        private void Start()
        {
            AlchemistAPIHandle.Instance.CoinDatasUpdatedStream += OnCoinDataUpdate;
        }

        private void OnCoinDataUpdate(List<CoinPrice> data)
        {
            this.data = data;
            SetBanner(data[0].Coin);

        }

        public async void SetBanner(string coinName)
        {
            if (data == null || data.Count == 0) return;
            if (coinName != null)
                this.coinPair.text = coinName + "/XRP";
            foreach (var coin in data)
            {
                if (coin.Coin.Equals(coinName))
                {
                    await LerpDecimalAsync(priceValue, coin.Price, duration, SetPrice);
                    await LerpDecimalAsync(changeValue, coin.Price_Change, duration, SetChange);
                    /*price.DOColor(coin.Price_Change > 0 ? upColor : downColor, duration);
                    change.DOColor(coin.Price_Change > 0 ? upColor : downColor, duration);*/
                    await price.ChangeColorAsync(coin.Price_Change > 0 ? upColor : downColor, duration);
                    await change.ChangeColorAsync(coin.Price_Change > 0 ? upColor : downColor, duration);
                    break;
                }
            }
        }

        private async UniTask LerpDecimalAsync(decimal startValue, decimal endValue, float duration, Action<decimal> onValueChange)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // Calculate the interpolation factor (t) from 0 to 1
                decimal t = (decimal)Mathf.Clamp01(elapsedTime / duration);

                // Get the interpolated value
                decimal currentValue = Lerp(startValue, endValue, t);

                // Wait for the next frame
                await UniTask.Yield();

                // Update the elapsed time
                elapsedTime += Time.deltaTime;

                // Invoke the callback with the interpolated value
                onValueChange(currentValue);
            }
        }

        private decimal Lerp(decimal a, decimal b, decimal t)
        {
            return a + (b - a) * t;
        }

        private void SetPrice(decimal price)
        {
            priceValue = price;
            this.price.text = "$" + priceValue.ToString("0.######");
        }

        private void SetChange(decimal change)
        {
            changeValue = change;
            this.change.text = changeValue.ToString("0.######") + "%";
        }
    }
}
