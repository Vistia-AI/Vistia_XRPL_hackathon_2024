using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TableLineContent : MonoBehaviour, IPoolable<TableLineContent>
{
    [SerializeField] private TMP_Text number;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text time;
    [SerializeField] private Button button;
    private Action<TableLineContent> returnAction;
    private StockData stockData;
    private Signal signal;
    private void Start()
    {
        this.transform.localScale = Vector3.one;
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        UIManager.Instance.ShowPopup(PopupType.Statistics, (popUp) =>
        {
            StatisticPopup statisticPopup = popUp as StatisticPopup;
            statisticPopup.SetData(stockData, signal);
        });
    }

    public void Initialize(Action<TableLineContent> returnAction)
    {
        this.returnAction = returnAction;
    }

    public void ReturnToPool()
    {
        returnAction?.Invoke(this);
    }

    public void SetContent(int index, StockData data, Signal signal)
    {
        this.stockData = data;
        this.signal = signal;
        this.number.text = index.ToString();
        this._name.text = data.Symbol.ToString();
        TimeZoneInfo tzinfo = TimeZoneInfo.Local;
        DateTime temp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(data.DateCreated)).DateTime;
        this.time.text = TimeZoneInfo.ConvertTimeFromUtc(temp, tzinfo).ToString();
    }
}
