using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoggerMassage : MonoBehaviour
{
    [SerializeField] TMP_Text log;
    [SerializeField] Button button;

    private string stackTrace;
    private Action<string> _callBack;

    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            _callBack?.Invoke(stackTrace);
        });
    }

    public void SetLog(string logString, string stackTraceString, LogType type, Action<string> callBack)
    {
        string color;

        switch (type)
        {
            case LogType.Error:
                color = "red";
                break;
            case LogType.Warning:
                color = "yellow";
                break;
            case LogType.Log:
            default:
                color = "white";
                break;
        }
        _callBack = callBack;
        stackTrace = stackTraceString;
        log.text = $"<b><color={color}>{type.ToString()}:</color></b> <color=black>{logString}</color>";
    }
}
