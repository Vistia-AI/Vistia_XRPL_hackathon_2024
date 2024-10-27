using System.Collections.Generic;
using System;
using UnityEngine;

public class TimePointController : MonoBehaviour
{
    [SerializeField] private TimeTag timeTagPrefab;
    [SerializeField] private Transform timeTagContainer;

    private ObjectPool<TimeTag> timeTagPool;

    private List<TimeTag> activeTimeTag = new List<TimeTag>();

    private void Start()
    {
        timeTagPool = new ObjectPool<TimeTag>(timeTagPrefab, 5);
    }

    public void SetTimePoint(long startTime, long endTime, OpenTime openTime, int interval = 5, bool isMillisecond = true)
    {
        List<string> timeline;
        if (isMillisecond)
        {
            timeline = GenerateTimeline(DateTimeOffset.FromUnixTimeMilliseconds(startTime), DateTimeOffset.FromUnixTimeMilliseconds(endTime), openTime, interval);
        }
        else
        {
            timeline = GenerateTimeline(DateTimeOffset.FromUnixTimeSeconds(startTime), DateTimeOffset.FromUnixTimeSeconds(endTime), openTime, interval);
        }
        int totalIncrement = timeline.Count;
        int currentCount = activeTimeTag.Count;

        for (int i = currentCount - 1; i >= 0; i--)
        {
            TimeTag temp = activeTimeTag[i];
            temp.ReturnToPool();
            activeTimeTag.RemoveAt(i);
        }

        for (int i = 0; i < totalIncrement; i++)
        {
            TimeTag temp = timeTagPool.Pull();
            temp.SetText(timeline[i]);
            activeTimeTag.Add(temp);
        }
    }

    public void SetTimePointsPos(float maxX)
    {
        for (int i = 0; i < activeTimeTag.Count; i++)
        {
            activeTimeTag[i].transform.SetParent(timeTagContainer);
            activeTimeTag[i].transform.localScale = Vector3.one;
            activeTimeTag[i].transform.localPosition = new Vector3(maxX / (activeTimeTag.Count + 1) * (i + 1), 0, 0);
        }
    }

    private List<string> GenerateTimeline(DateTimeOffset timeA, DateTimeOffset timeB, OpenTime openTime, int n)
    {
        Debug.Log(timeA.UtcDateTime);
        Debug.Log(timeB.UtcDateTime);
        TimeSpan totalDuration = timeB - timeA;
        int totalDays = totalDuration.Days;
        if (openTime == OpenTime.One_Day_Ago)
            n = Mathf.Min(n, 4);
        TimeSpan step = totalDuration / (n + 1);
        string format = string.Empty;
        /*switch (openTime)
        {
            case OpenTime.Five_Years_Ago:
                format = "yyyy";
                break;
            case OpenTime.One_Year_Ago:
                if (timeA.Year != timeB.Year)
                    format = "MMM yy";
                else
                    format = "MMM";
                break;
            case OpenTime.One_Month_Ago or OpenTime.One_Week_Ago:
                if (timeA.Month != timeB.Month)
                    format = "dd MMM";
                else
                    format = "dd";
                break;
            case OpenTime.One_Day_Ago:
                format = "hh:mm tt";
                break;
        }*/
        if (totalDays <= 1)
        {
            format = "hh:mm tt";
        }
        else if (totalDays <= 30)
        {
            if (timeA.Month != timeB.Month)
                format = "dd MMM";
            else
                format = "dd";
        }
        else if (totalDays <= 365)
        {
            if (timeA.Year != timeB.Year)
                format = "MMM yy";
            else
                format = "MMM";
        }
        else
        {
            format = "MMM yy";
        }

        List<string> labels = new List<string>();

        // Generate timeline labels
        for (int i = 1; i <= n; i++)
        {
            DateTimeOffset currentLabelTime = timeA.Add(step * i);
            labels.Add(currentLabelTime.ToString(format));
        }

        return labels;
    }
}
