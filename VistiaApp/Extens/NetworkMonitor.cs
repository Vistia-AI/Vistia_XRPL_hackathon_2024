using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class NetworkMonitor : Singleton<NetworkMonitor>
{
    [SerializeField]
    private string pingAddress = "8.8.8.8"; // Google DNS address
    [SerializeField]
    private float checkInterval = 5f;
    private CancellationTokenSource cancellationTokenSource;
    private bool isConnected = false;
    private bool firstTime = false;

    public bool IsConnected
    {
        get => isConnected;
        private set
        {
            if (isConnected != value || !firstTime)
            {
                if (value)
                {
                    // Handle active connection
                    Debug.Log("Internet connection is active.");
                    NetworkReachability connectionType = Application.internetReachability;
                    HandleConnectionType(connectionType);
                }
                else
                {
                    UIManager.Instance.ShowPopup(PopupType.Connection, null);
                    Debug.Log("Internet connection is lost.");
                }
                OnConnectionStatusChanged?.Invoke(value);
            }
            firstTime = true;
            isConnected = value;
        }
    }

    private Action<bool> OnConnectionStatusChanged;

    public event Action<bool> OnConnectionStatusChangedEvent
    {
        add
        {
            if (isConnected) value?.Invoke(isConnected);
            else if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                Init();
            }
            OnConnectionStatusChanged += value;
        }
        remove => OnConnectionStatusChanged -= value;
    }

    void Awake()
    {
        Debug.Log("NetworkMonitor Awake");
        Init();
    }

    void Init()
    {
        cancellationTokenSource = new CancellationTokenSource();
        MonitorInternetConnectionAsync(cancellationTokenSource.Token).Forget();
    }

    public void ForceCheckConnection()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
            Init();
        }
    }

    private void OnDestroy()
    {
        cancellationTokenSource?.Cancel();
    }

    private void OnApplicationQuit()
    {
        cancellationTokenSource?.Cancel();
    }

    async UniTaskVoid MonitorInternetConnectionAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            IsConnected = await CheckInternetConnectionAsync(pingAddress);

            await UniTask.Delay((int)(checkInterval * 1000), cancellationToken: token);
        }
    }

    private async UniTask<bool> CheckInternetConnectionAsync(string address)
    {
        Ping ping = new Ping(address);

        // Wait until the ping completes or times out (set a timeout limit, e.g., 2 seconds)
        float timeout = 2f; // seconds
        while (!ping.isDone && timeout > 0)
        {
            await UniTask.Yield(); // Wait for the next frame
            timeout -= Time.deltaTime;
        }

        // Return true if the ping was successful, otherwise false
        return ping.isDone && ping.time >= 0;
    }

    private void HandleConnectionType(NetworkReachability connectionType)
    {
        switch (connectionType)
        {
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                Debug.Log("Connected via Wi-Fi or Ethernet.");
                // Handle Wi-Fi or Ethernet connection (could be used to prioritize downloads, etc.)
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                Debug.Log("Connected via Mobile Data.");
                // Handle mobile data connection (could warn the user about data usage)
                break;
            case NetworkReachability.NotReachable:
                Debug.Log("No internet connection.");
                // Handle no connection case
                break;
        }
    }
}
