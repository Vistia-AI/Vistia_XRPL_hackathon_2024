using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Logger : MonoBehaviour
{
    [SerializeField] private LoggerMassage loggerMassagePrefabs;
    [SerializeField] private Transform container;
    [SerializeField] TMP_Text stackTrace;

    private List<LoggerMassage> _loggerMassages = new List<LoggerMassage>();
    private Stack<LoggerMassage> _loggerMassagePool = new Stack<LoggerMassage>();

    private void Start()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        Debug.Log(logString);
        if (_loggerMassagePool.Count == 0)
        {
            var loggerMassage = Instantiate(loggerMassagePrefabs, container);
            _loggerMassages.Add(loggerMassage);
            loggerMassage.SetLog(logString, stackTrace, type, ShowStackTrace);
        }
        else
        {
            var loggerMassage = _loggerMassagePool.Pop();
            loggerMassage.gameObject.SetActive(true);
            _loggerMassages.Add(loggerMassage);
            loggerMassage.SetLog(logString, stackTrace, type, ShowStackTrace);
        }
    }

    private void ShowStackTrace(string stackTrace)
    {
        this.stackTrace.text = stackTrace;
    }

    public void ClearLogs()
    {
        foreach (var loggerMassage in _loggerMassages)
        {
            loggerMassage.gameObject.SetActive(false);
            _loggerMassagePool.Push(loggerMassage);
        }

        _loggerMassages.Clear();
    }
}
