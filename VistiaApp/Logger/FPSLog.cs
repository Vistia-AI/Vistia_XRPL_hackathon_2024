using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class FPSLog : MonoBehaviour
{
    [SerializeField] private TMP_Text fpsText;

    private void Start()
    {
        UpdateFps().Forget();
    }

    private async UniTask UpdateFps()
    {
        while (true)
        {
            fpsText.text = $"FPS: {(1 / Time.deltaTime).ToString("F2")}";
            await UniTask.Delay(1000);
        }
    }
}
