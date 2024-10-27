using System;
using TMPro;
using UnityEngine;

public class PriceTag : MonoBehaviour, IPoolable<PriceTag>
{
    [SerializeField] private TMP_Text text;
    private Action<PriceTag> action;

    public void SetText(string text)
    {
        this.text.text = text;
    }
    public void Initialize(Action<PriceTag> returnAction)
    {
        this.action = returnAction;
    }

    public void ReturnToPool()
    {
        action?.Invoke(this);
    }
}
