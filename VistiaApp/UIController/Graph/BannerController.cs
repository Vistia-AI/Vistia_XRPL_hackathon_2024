using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;

namespace Graph
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
        public void SetBanner(string coinName)
        {
            AlchemistAPIHandle.Instance.SearchCurrency(coinName, 0, 1, OnGetData);
        }

        public async void SetBanner(Currency coin)
        {
            SetCoin(coin);
        }

        private async void OnGetData(List<Currency> data, long code)
        {
            if (data.Count == 0) return;
            Currency coin = data[0];
            SetCoin(coin);
        }
        private async void SetCoin(Currency coin)
        {
            this.coinPair.text = coin.Symbol + "/XRP";
            await LerpDecimalAsync(priceValue, coin.Price, duration, SetPrice);
            await LerpDecimalAsync(changeValue, coin.Percent_Change_24h, duration, SetChange);

            /*await LMotion.Create((float)priceValue, (float)coin.Price, duration)
                .BindToText(price, "{0:F2}");

            await LMotion.Create((float)changeValue, (float)coin.Percent_Change_24h, duration)
                .BindToText(change, "{0:F2}");*/

            /*for (int i = 0; i < price.textInfo.characterCount; i++)
            {
                LMotion.Create(price.color, coin.Percent_Change_24h > 0 ? upColor : downColor, duration)
                    .WithDelay(i * 0.1f)
                    .WithEase(Ease.OutQuad)
                    .BindToTMPCharColor(price, i);

                LMotion.Punch.Create(Vector3.zero, Vector3.up * 15f, duration)
                    .WithDelay(i * 0.1f)
                    .WithEase(Ease.OutQuad)
                    .BindToTMPCharPosition(price, i);
            }

            for (int i = 0; i < change.textInfo.characterCount; i++)
            {
                LMotion.Create(change.color, coin.Percent_Change_24h > 0 ? upColor : downColor, duration)
                    .WithDelay(i * 0.1f)
                    .WithEase(Ease.OutQuad)
                    .BindToTMPCharColor(change, i);

                LMotion.Punch.Create(Vector3.zero, Vector3.up * 15f, duration)
                    .WithDelay(i * 0.1f)
                    .WithEase(Ease.OutQuad)
                    .BindToTMPCharPosition(change, i);
            }*/
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

            onValueChange(endValue);
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
