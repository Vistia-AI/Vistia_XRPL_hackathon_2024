using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class AlchemistAPIHandle : Singleton<AlchemistAPIHandle>
{
    private const string baseUrl = "https://api.vistia.co/api";
    private const string version = "v2_1";
    private CancellationTokenSource cancellationTokenSource;

    [SerializeField] private float UpdateInterval = 300f; // Update interval in seconds
    [SerializeField] private HeatMapType heatMapType = HeatMapType.RSI7;
    [SerializeField] private TimeType timeType = TimeType.ONE_DAY;

    // Params
    public HeatMapType HeatMapType { get => heatMapType; set => heatMapType = value; }
    public TimeType TimeType { get => timeType; set => timeType = value; }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;
        cancellationTokenSource = new CancellationTokenSource();
        StartDataUpdateLoop(cancellationTokenSource.Token).Forget();

        NetworkMonitor.Instance.OnConnectionStatusChangedEvent += OnConnectionStatusChanged;
    }

    private void OnConnectionStatusChanged(bool isConnected)
    {
        Debug.Log("Connection status changed: " + isConnected);
        cancellationTokenSource.Cancel();
        if (isConnected)
        {
            cancellationTokenSource = new CancellationTokenSource();
            StartDataUpdateLoop(cancellationTokenSource.Token).Forget();
        }
    }

    private void OnDestroy()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
        }
    }

    private void OnApplicationQuit()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
        }
    }

    private async UniTask StartDataUpdateLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && Application.isPlaying)
        {
            Call_Al_API();
            Call_API_Price();
            await UniTask.Delay(TimeSpan.FromSeconds(UpdateInterval), cancellationToken: cancellationToken);
        }
    }

    private string GetAPIURL(string query, params string[] endPoint)
    {
        return $"{baseUrl}/{version}/{string.Join("/", endPoint)}{(string.IsNullOrEmpty(query) ? string.Empty : query)}";
    }

    private string GetQuery(params string[] queries)
    {
        return "?" + string.Join("&", queries);
    }

    public async void Call_API_Price()
    {
        if (OnCoinDatasUpdated != null)
            await GetCoinPricesAsync();
    }

    public async void Call_Al_API()
    {
        if (OnTopOverSoldUpdated != null) await GetTopOverSold();

        if (OnTopOverBoughtUpdated != null) await GetTopOverBought();

        if (OnFibonacciInfoUpdated != null) await GetFibonacciInfo();

        if (OnOriginalPairUpdated != null) await GetOriginalPairListAsync();

        if (OnChartDatasUpdated != null) await GetChartData();

        if (AIPredictionUpdated != null) await GetAIPredictionsAsync();
    }

    #region AI_ANALYSIS_GET_PREDICTIONS
    private List<PredictionData> predictionDatas;
    public List<PredictionData> PredictionDatas
    {
        private set
        {
            predictionDatas = value;
            if (value != null)
                AIPredictionUpdated?.Invoke(predictionDatas);
        }
        get => predictionDatas;
    }

    private Action<List<PredictionData>> AIPredictionUpdated;

    public event Action<List<PredictionData>> AIPredictionUpdatedStream
    {
        add
        {
            if (predictionDatas == null) GetAIPredictionsAsync().Forget();
            else value(predictionDatas);
            AIPredictionUpdated += value;
        }
        remove => AIPredictionUpdated -= value;
    }

    private async UniTask GetAIPredictionsAsync()
    {
        string url = GetAPIURL(null, "ai-analysis", "get-predictions");

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
            {
                await UniTask.Yield(); // Yield control back to the main thread until the request is done
            }

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                // Deserialize JSON response
                PredictionDatas = JsonConvert.DeserializeObject<List<PredictionData>>(jsonResponse);

            }
            else
            {
                ErrorHandle(webRequest);
            }
        }
    }
    #endregion

    #region AI_ANALYSIS_PREDICT_VALIDATE
    private TotalPredictValidate predictionValidateDatas;
    public TotalPredictValidate PredictionValidateDatas
    {
        private set
        {
            predictionValidateDatas = value;
            if (value != null)
                AIPredictionValidateUpdated?.Invoke(predictionValidateDatas);
        }
        get => predictionValidateDatas;
    }

    private Action<TotalPredictValidate> AIPredictionValidateUpdated;

    public event Action<TotalPredictValidate> AIPredictionValidateUpdatedStream
    {
        add
        {
            if (predictionValidateDatas == null) GetAIPredictionsValidateAsync().Forget();
            else value(predictionValidateDatas);
            AIPredictionValidateUpdated += value;
        }
        remove => AIPredictionValidateUpdated -= value;
    }

    private async UniTask GetAIPredictionsValidateAsync()
    {
        string url = GetAPIURL(null, "ai-analysis", "predict-validate", GetQuery("interval=" + Interval.One_Month.ToString()));

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
            {
                await UniTask.Yield(); // Yield control back to the main thread until the request is done
            }

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                // Deserialize JSON response
                PredictionValidateDatas = JsonConvert.DeserializeObject<TotalPredictValidate>(jsonResponse);
            }
            else
            {
                ErrorHandle(webRequest);
                // error callback
            }
        }
    }
    #endregion

    #region AI_ANALYSIS_PREDICT_VALIDATE_SYMBOL
    public async void PredictValidate(string symbol, string n_predict, Action<TotalPredictValidate> callback)
    {
        await PredictValidateAsync(symbol, n_predict, callback);
    }

    private async UniTask PredictValidateAsync(string symbol, string n_predict, Action<TotalPredictValidate> callback)
    {
        string url = GetAPIURL(
            GetQuery("interval=" + n_predict),
            "ai-analysis",
            "predict-validate",
            symbol
            );
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
                string jsonResponse = webRequest.downloadHandler.text;
                TotalPredictValidate temp = JsonConvert.DeserializeObject<TotalPredictValidate>(jsonResponse);
                // Deserialize JSON response
                callback?.Invoke(temp);
            }
            else
            {
                ErrorHandle(webRequest);
                // error callback
            }
        }
    }
    #endregion

    #region AI_ANALYSIS_PREDICT_VALIDATE_CHART
    public async void PredictValidateChart(string symbol, Action<List<PredictChartData>> callback)
    {
        await PredictValidateChartAsync(symbol, callback);
    }

    private async UniTask PredictValidateChartAsync(string symbol, Action<List<PredictChartData>> callback)
    {
        string url = GetAPIURL(
            GetQuery("limit=-1"),
            "ai-analysis",
            "predict-validate",
            symbol,
            "chart");
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
                string jsonResponse = webRequest.downloadHandler.text;
                List<PredictChartData> temp = JsonConvert.DeserializeObject<List<PredictChartData>>(jsonResponse);
                callback?.Invoke(temp);
            }
            else
            {
                ErrorHandle(webRequest);
                // error callback
            }
        }
    }
    #endregion

    #region AI_TRADE_ORIGINAL_PAIR_LIST
    private Action<List<OriginalPair>> OnOriginalPairUpdated;
    public event Action<List<OriginalPair>> OriginalPairUpdatedStream
    {
        add
        {
            if (OnOriginalPairUpdated == null) GetOriginalPairListAsync().Forget();
            OnOriginalPairUpdated += value;
        }
        remove => OnOriginalPairUpdated -= value;
    }

    private async UniTask GetOriginalPairListAsync()
    {
        using (UnityWebRequest webRequest = UnityWebRequest
            .Get(GetAPIURL(
                GetQuery("timeType=" + timeType.ToString()),
                "fibonacci",
                "original-pair-list")))
        {
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
            {
                await UniTask.Yield(); // Yield control back to the main thread until the request is done
            }
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                //Debug.Log(jsonResponse);

                // Deserialize JSON response
                List<OriginalPair> temp = JsonConvert.DeserializeObject<List<OriginalPair>>(jsonResponse);
            }
            else
            {
                ErrorHandle(webRequest);
            }
        }
    }
    #endregion

    #region AI_TRADE_FIBONACCI_INFO
    private Action<FibonacciInfo> OnFibonacciInfoUpdated;
    public event Action<FibonacciInfo> FibonacciInfoUpdatedStream
    {
        add
        {
            if (OnFibonacciInfoUpdated == null) GetFibonacciInfo().Forget();
            OnFibonacciInfoUpdated += value;
        }
        remove => OnFibonacciInfoUpdated -= value;
    }

    private async UniTask GetFibonacciInfo()
    {
        using (UnityWebRequest webRequest = UnityWebRequest
            .Get(GetAPIURL(
                GetQuery("originalPair=LINAUSDT", "timeType=" + timeType.ToString()),
                "fibonacci",
                "fibonacci-info")))
        {
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
            {
                await UniTask.Yield(); // Yield control back to the main thread until the request is done
            }

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                //Debug.Log(jsonResponse);

                // Deserialize JSON response
                FibonacciInfo temp = JsonConvert.DeserializeObject<FibonacciInfo>(jsonResponse);
            }
            else
            {
                ErrorHandle(webRequest);
            }
        }
    }
    #endregion

    #region AI_TRADE_TOP_OVER_SOLD
    private List<StockData> tosList;
    public List<StockData> TosList
    {
        private set
        {
            tosList = value;
            if (value != null)
                OnTopOverSoldUpdated?.Invoke(tosList);
        }
        get => tosList;
    }
    private Action<List<StockData>> OnTopOverSoldUpdated;

    public event Action<List<StockData>> TopOverSoldUpdatedStream
    {
        add
        {
            if (tosList == null) GetTopOverSold().Forget();
            else value(tosList);
            OnTopOverSoldUpdated += value;
        }
        remove => OnTopOverSoldUpdated -= value;
    }

    private async UniTask GetTopOverSold()
    {
        string url = GetAPIURL(
            GetQuery("heatMapType=" + heatMapType.ToString(), "interval=" + timeType.ToString()),
            "al-trade",
            "top-over-sold");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
            {
                await UniTask.Yield(); // Yield control back to the main thread until the request is done
            }

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                //Debug.Log(jsonResponse);

                // Deserialize JSON response
                TosList = JsonConvert.DeserializeObject<List<StockData>>(jsonResponse);
            }
            else
            {
                ErrorHandle(webRequest);
            }
        }
    }
    #endregion

    #region AI_TRADE_TOP_OVER_BOUGHT
    private List<StockData> tobList;
    public List<StockData> TobList
    {
        private set
        {
            tobList = value;
            if (value != null)
                OnTopOverBoughtUpdated?.Invoke(tobList);
        }
        get => tobList;
    }
    private Action<List<StockData>> OnTopOverBoughtUpdated;

    public event Action<List<StockData>> TopOverBoughtUpdatedStream
    {
        add
        {
            if (tobList == null) GetTopOverBought().Forget();
            else value(tobList);
            OnTopOverBoughtUpdated += value;
        }
        remove => OnTopOverBoughtUpdated -= value;
    }

    private async UniTask GetTopOverBought()
    {
        string url = GetAPIURL(
            GetQuery("heatMapType=" + heatMapType.ToString(), "interval=" + timeType.ToString()),
            "al-trade",
            "top-over-bought");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
            {
                await UniTask.Yield(); // Yield control back to the main thread until the request is done
            }

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                //Debug.Log(jsonResponse);

                // Deserialize JSON response
                TobList = JsonConvert.DeserializeObject<List<StockData>>(jsonResponse);

            }
            else
            {
                ErrorHandle(webRequest);
            }
        }
    }
    #endregion

    #region AI_TRADE_CHART_DATA
    private List<ChartData> chartDatas;
    public List<ChartData> ChartDatas
    {
        private set
        {
            chartDatas = value;
            if (value != null)
                OnChartDatasUpdated?.Invoke(chartDatas);
        }
        get => chartDatas;
    }

    private Action<List<ChartData>> OnChartDatasUpdated;
    public event Action<List<ChartData>> ChartDatasUpdatedStream
    {
        add
        {
            if (chartDatas == null) GetChartData().Forget();
            else value(chartDatas);
            OnChartDatasUpdated += value;
        }
        remove => OnChartDatasUpdated -= value;
    }
    private async UniTask GetChartData()
    {
        string url = GetAPIURL(
            GetQuery("heatMapType=" + heatMapType.ToString(), "interval=" + timeType.ToString()),
            "al-trade",
            "chart-data");
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
                string jsonResponse = webRequest.downloadHandler.text;

                // Deserialize JSON response
                ChartDatas = JsonConvert.DeserializeObject<List<ChartData>>(jsonResponse);
            }
            else
            {
                ErrorHandle(webRequest);
            }
        }
    }
    #endregion

    #region AI_CHAT_BOT

    public async void ChatBot(string message, Action<string> callback, Action<string> errorCallback)
    {
        await ChatBotAsync(message, callback, errorCallback);
    }

    private async UniTask ChatBotAsync(string message, Action<string> callback, Action<string> errorCallBack)
    {
        string url = GetAPIURL(
            GetQuery("user_query=" + message),
            "ai-chat", "chat");

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
            {
                await UniTask.Yield(); // Yield control back to the main thread until the request is done
            }

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                callback?.Invoke(jsonResponse);
            }
            else
            {
                ErrorHandle(webRequest);
                errorCallBack?.Invoke(webRequest.error);
                // error callback
            }
        }
    }
    #endregion

    #region PRICES_COIN_PRICES
    private List<CoinPrice> coinDatas;
    public List<CoinPrice> CoinDatas
    {
        private set
        {
            coinDatas = value;
            if (value != null)
                OnCoinDatasUpdated?.Invoke(coinDatas);
        }
        get => coinDatas;
    }

    private Action<List<CoinPrice>> OnCoinDatasUpdated;
    public event Action<List<CoinPrice>> CoinDatasUpdatedStream
    {
        add
        {
            if (coinDatas == null) GetCoinPricesAsync().Forget();
            else value(coinDatas);
            OnCoinDatasUpdated += value;
        }
        remove => OnCoinDatasUpdated -= value;
    }
    private async UniTask GetCoinPricesAsync()
    {
        string url = GetAPIURL(null, "prices", "latest-prices/");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
            {
                await UniTask.Yield(); // Yield control back to the main thread until the request is done
            }

            long code = webRequest.responseCode;

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;

                // Deserialize JSON response
                CoinDatas = JsonConvert.DeserializeObject<List<CoinPrice>>(jsonResponse);
            }
            else
            {
                ErrorHandle(webRequest);
            }
        }
    }
    #endregion

    #region PRICES_KLINE
    public async void GetHistoricalKlinesAsync(string symbol, Action<List<Tuple<decimal, long>>, decimal, decimal> callback, int offset = 0, int limit = 24)
    {
        await GetHistoricalKlinesTask(symbol, callback, offset, limit);
    }

    private async UniTask GetHistoricalKlinesTask(string symbol, Action<List<Tuple<decimal, long>>, decimal, decimal> callback, int offset, int limit)
    {
        string url = GetAPIURL(GetQuery($"offset={offset}", $"limit={limit}"), "prices", "coin-price", symbol);
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
                    decimal closePrice = kline["price"].Value<decimal>();
                    if (closePrice > mx_closePrice) mx_closePrice = closePrice;
                    if (closePrice < mn_closePrice) mn_closePrice = closePrice;
                    kLine.Add(new Tuple<decimal, long>(closePrice, kline["time"].Value<long>()));
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
    #endregion

    #region SEARCH_CURRENCY
    private List<Currency> searchDatas;
    public async void SearchCurrency(string value, int skip, int limit, Action<List<Currency>, long> callback)
    {
        await SearchCurrencyAsync(value, skip, limit, callback);
    }

    private async UniTask SearchCurrencyAsync(string value, int skip, int limit, Action<List<Currency>, long> callback)
    {
        string url = GetAPIURL(
            GetQuery("key=" + value, "skip=" + skip.ToString(), "limit=" + limit.ToString()),
            "search",
            "currency");

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
            {
                await UniTask.Yield(); // Yield control back to the main thread until the request is done
            }

            long code = webRequest.responseCode;

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                // Deserialize JSON response
                searchDatas = JsonConvert.DeserializeObject<List<Currency>>(jsonResponse);
                callback?.Invoke(searchDatas, code);
            }
            else
            {
                ErrorHandle(webRequest);
            }
        }
    }
    #endregion

    #region Get_Currency_Logo
    private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
    public async void GetCurrencyLogo(string symbol, Action<string, Sprite> callback)
    {
        if (tokenInfoCache.ContainsKey(symbol))
        {
            callback?.Invoke(symbol, tokenInfoCache[symbol].image_sprite);
            return;
        }
        await GetCurrencyLogoAsync(symbol, callback);
    }

    private async UniTask GetCurrencyLogoAsync(string symbol, Action<string, Sprite> callback)
    {
        string url = $"https://bin.bnbstatic.com/static/assets/logos/{symbol}.png";

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        await request.SendWebRequest().ToUniTask();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Get the texture from the request
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            // Create a sprite from the texture
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            if (!spriteCache.ContainsKey(symbol))
            {
                spriteCache.Add(symbol, sprite);
            }

            callback?.Invoke(symbol, sprite);
        }
        else
        {
            ErrorHandle(request);
        }
    }
    #endregion

    #region TOKEN_INFO
    private Dictionary<string, TokenInfo> tokenInfoCache;

    public Dictionary<string, TokenInfo> TokenInfoCache
    {
        get { return tokenInfoCache; }
        private set
        {
            tokenInfoCache = value;
            if (value != null)
                OnGetTokenInfo?.Invoke(tokenInfoCache);
        }
    }

    private Action<Dictionary<string, TokenInfo>> OnGetTokenInfo;
    public event Action<Dictionary<string, TokenInfo>> OnGetTokenInfoEven
    {
        add
        {
            if (tokenInfoCache == null) GetAllTokenInfoAsync().Forget();
            else value(tokenInfoCache);
            OnGetTokenInfo += value;
        }
        remove => OnGetTokenInfo -= value;
    }

    private async UniTask GetTokenSpriteAsync(string symbol, string url, Dictionary<string, TokenInfo> temp)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        await request.SendWebRequest().ToUniTask();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Get the texture from the request
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            // Create a sprite from the texture
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            temp[symbol].image_sprite = sprite;
        }
        else
        {
            ErrorHandle(request);
        }
    }

    private async UniTask GetAllTokenInfoAsync()
    {
        string url = GetAPIURL(null, "token", "info");

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
            {
                await UniTask.Yield(); // Yield control back to the main thread until the request is done
            }

            // List to hold all async tasks
            List<UniTask> tasks = new List<UniTask>();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                List<TokenInfo> temp = JsonConvert.DeserializeObject<List<TokenInfo>>(jsonResponse);
                Dictionary<string, TokenInfo> tempTokenInfoCache = new Dictionary<string, TokenInfo>();
                foreach (var item in temp)
                {
                    if (!tempTokenInfoCache.ContainsKey(item.code))
                    {
                        tempTokenInfoCache.TryAdd(item.code, item);

                        // Start GetTokenSpriteAsync for each item and add it to the task list
                        tasks.Add(GetTokenSpriteAsync(item.code, item.image_url, tempTokenInfoCache));
                    }
                }

                // Wait for all tasks to complete
                await UniTask.WhenAll(tasks);
                TokenInfoCache = tempTokenInfoCache;
            }

            else
            {
                ErrorHandle(webRequest);
            }
        }
    }
    #endregion

    public string GetResponseMessage(long code)
    {
        switch (code)
        {
            case 200:
                return "Success";
            case 400:
                return "Bad Request";
            case 401:
                return "Unauthorized";
            case 403:
                return "Forbidden";
            case 404:
                return "Not Found";
            case 500:
                return "Internal Server Error";
            default:
                return "Unknown Error";
        }
    }

    private void ErrorHandle(UnityWebRequest webRequest)
    {
        if (webRequest.result == UnityWebRequest.Result.ConnectionError) // client side error
        {
            NetworkMonitor.Instance.ForceCheckConnection();
        }
        else
        {
            Debug.LogError("Error: " + webRequest.error);
        }
    }
}