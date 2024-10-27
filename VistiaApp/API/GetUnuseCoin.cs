using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class GetUnuseCoin : MonoBehaviour
{
    int skip = 0;
    int limit = 20;
    string value = string.Empty;
    int index = 0;
    Dictionary<string, string> coinDict = new Dictionary<string, string>();
    List<char> chars = new List<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };


    private string imageURL(string a) => $"https://bin.bnbstatic.com/static/assets/logos/{a}.png";

    private void Start()
    {
        value = chars[index].ToString();
        SearchTikers(value);
    }

    private void SearchTikers(string value)
    {
        AlchemistAPIHandle.Instance.SearchCurrency(value, skip, limit, OnGetData);
    }

    private async void OnGetData(List<Currency> dataList, long code)
    {
        int dataCount = dataList.Count;
        Debug.Log(dataCount);
        if (dataCount == 0)
        {
            if (index < chars.Count - 1)
            {
                int nextIndex = index + 1;
                index = nextIndex;
                value = chars[nextIndex].ToString();
                skip = 0;
                SearchTikers(value);
            }
            else
                OnFinish();
            return;
        }

        foreach (var data in dataList)
        {
            if (coinDict.ContainsKey(data.Symbol))
            {
                continue;
            }
            await LoadImageAsync(data.Symbol, data.Name);
        }

        if (dataCount < limit)
        {
            if (index < chars.Count - 1)
            {
                int nextIndex = index + 1;
                index = nextIndex;
                value = chars[nextIndex].ToString();
                skip = 0;
                SearchTikers(value);
            }
            else
                OnFinish();
        }
        else
        {
            skip += limit;
            SearchTikers(value);
        }
    }

    private void OnFinish()
    {
        // sort dict by key
        var sortedCoinDict = coinDict.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        SaveSortedCoinDictToJson(coinDict);
    }

    void SaveSortedCoinDictToJson(Dictionary<string, string> coinDict)
    {
        string json = JsonConvert.SerializeObject(coinDict, Formatting.Indented);

        // Get the path to the desktop
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string filePath = Path.Combine(desktopPath, "UnuseCryptocurrency.json");

        // Save JSON to the desktop
        File.WriteAllText(filePath, json);
    }

    async Task LoadImageAsync(string name, string fullName)
    {
        // Create a UnityWebRequest to fetch the image
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageURL(name));

        // Await the request to send and complete
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            coinDict.Add(name, fullName);
        }
    }
}
