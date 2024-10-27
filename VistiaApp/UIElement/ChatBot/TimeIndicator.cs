using System;
using UnityEngine;

public class TimeIndicator : MonoBehaviour, IPoolable<TimeIndicator>
{
    [SerializeField] private TMPro.TMP_Text m_timeText;

    private Action<TimeIndicator> m_returnAction;
    private void Start()
    {
        this.transform.localScale = Vector3.one;

    }
    public void SetTime(DateTime time)
    {
        m_timeText.text = time.ToString("MMM dd, yyyy");
    }

    public void SetCurrentTime()
    {
        // MMM DD, YYYY
        m_timeText.text = System.DateTime.Now.ToString("MMM dd, yyyy");
    }

    public void Initialize(Action<TimeIndicator> returnAction)
    {

        m_returnAction = returnAction;
    }

    public void ReturnToPool()
    {
        m_returnAction?.Invoke(this);
    }
}
