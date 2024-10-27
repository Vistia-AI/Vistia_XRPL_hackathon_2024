using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class TimeTag : MonoBehaviour, IPoolable<TimeTag>
{
    [SerializeField] private TMP_Text text;
    private Action<TimeTag> action;

    public void SetText(string text)
    {
        this.text.text = text;
    }
    public void Initialize(Action<TimeTag> returnAction)
    {
        this.action = returnAction;
    }

    public void ReturnToPool()
    {
        action?.Invoke(this);
    }
}

public enum TimeTagType
{
    [Description("dd MMM")]
    date_month,
    [Description("HH'h'")]
    hour,
    [Description("MMM")]
    month,
}
