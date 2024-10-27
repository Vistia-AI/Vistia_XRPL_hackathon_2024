using LitMotion;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionPopUp : PopupBase
{
    [SerializeField] private Image icon;
    [SerializeField] private Button tryAgainBtn;
    bool isConnectionActive = false;
    MotionHandle motionHandler;

    private void Start()
    {
        tryAgainBtn.onClick.AddListener(TryAgain);
        motionHandler = LMotion.Create(0, 0, 0).RunWithoutBinding();
        NetworkMonitor.Instance.OnConnectionStatusChangedEvent += OnNetworkStatusChange;
    }

    private void OnNetworkStatusChange(bool status)
    {
        Debug.Log(status);
        isConnectionActive = status;
        if (isConnectionActive)
        {
            motionHandler.Complete();
            this.gameObject.SetActive(false);
        }
        else
        {
            if (motionHandler.IsActive()) motionHandler.Cancel();
            this.gameObject.SetActive(true);
        }
    }

    private void TryAgain()
    {
        motionHandler = icon.transform.ChangeRotation(new Vector3(0, 0, -360), 2f, null, -1, LoopType.Incremental, Ease.InOutCubic);
        NetworkMonitor.Instance.ForceCheckConnection();
    }
}
