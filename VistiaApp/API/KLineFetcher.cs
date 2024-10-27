using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class KLineFetcher : Singleton<KLineFetcher>
{
    public async void GetHistoricalKlinesAsync(string symbol, Interval interval, OpenTime openTime, Action<List<Tuple<decimal, long>>, decimal, decimal> callback)
    {
        await GetHistoricalKlinesTask(symbol, interval, openTime, callback);
    }

    private async UniTask GetHistoricalKlinesTask(string symbol, Interval interval, OpenTime openTime, Action<List<Tuple<decimal, long>>, decimal, decimal> callback)
    {
        long startTime = GetUnixTime(openTime);
        long endTime = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

        string url = $"https://api.binance.com/api/v3/uiKlines?symbol={symbol}&interval={interval.GetEnumDescription()}&startTime={startTime}&endTime={endTime}";
        Debug.Log(url);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
            {
                await UniTask.Yield(); // Yield control back to the main thread until the request is done
            }

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // Process the response
                string jsonResponse = webRequest.downloadHandler.text;

                JArray klineArray = JArray.Parse(jsonResponse);

                decimal mx_closePrice = decimal.MinValue;
                decimal mn_closePrice = decimal.MaxValue;
                List<Tuple<decimal, long>> kLine = new List<Tuple<decimal, long>>();
                List<decimal> yAxisValues = new List<decimal>();
                foreach (var kline in klineArray)
                {
                    decimal closePrice = (decimal)kline[4];
                    if (closePrice > mx_closePrice) mx_closePrice = closePrice;
                    if (closePrice < mn_closePrice) mn_closePrice = closePrice;
                    kLine.Add(new Tuple<decimal, long>(closePrice, (long)kline[0]));
                }
                // You can now use this data to draw the graph
                callback?.Invoke(kLine, mx_closePrice, mn_closePrice);

            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
            }
        }
    }

    private long GetUnixTime(OpenTime openTime)
    {
        DateTime dateTime = DateTime.UtcNow;
        switch (openTime)
        {
            case OpenTime.One_Day_Ago:
                dateTime = dateTime.AddDays(-1);
                break;
            case OpenTime.Three_Days_Ago:
                dateTime = dateTime.AddDays(-3);
                break;
            case OpenTime.One_Week_Ago:
                dateTime = dateTime.AddDays(-7);
                break;
            case OpenTime.One_Month_Ago:
                dateTime = dateTime.AddMonths(-1);
                break;
            case OpenTime.One_Year_Ago:
                dateTime = dateTime.AddYears(-1);
                break;
            case OpenTime.Five_Years_Ago:
                dateTime = dateTime.AddYears(-5);
                break;
        }
        return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
    }
}
