using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatisticPopupTouchpoint : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] private StatisticPopup statisticPopup;
    [SerializeField] private float thresholdPercent = 10f;
    bool isDrag = false;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (statisticPopup.Tween != null || isDrag) return;
        isDrag = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        if (statisticPopup.Tween != null || !isDrag) return;
        statisticPopup.MainContentRect.offsetMax = new Vector2(0, eventData.position.y + statisticPopup.YValue);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (statisticPopup.Tween != null || !isDrag) return;
        float threshold = -statisticPopup.YValue * thresholdPercent / 100;
        float lowerThreshold = -threshold;
        float upperThreshold = +threshold;
        float currentYValue = statisticPopup.MainContentRect.offsetMax.y;
        if (currentYValue >= lowerThreshold && currentYValue <= upperThreshold)
        {
            float movTime = Mathf.Abs(currentYValue / statisticPopup.YValue * 2);
            statisticPopup.Tween = DOTween.To(() => currentYValue, x => statisticPopup.MainContentRect.offsetMax = new Vector2(0, x), 0, movTime)
            .SetEase(Ease.InCubic).OnComplete(() =>
            {
                statisticPopup.Tween = null;
                isDrag = false;
            });
        }
        else
        {
            float movTime;
            if (currentYValue > 0) movTime = Mathf.Abs((currentYValue - statisticPopup.YValue) / statisticPopup.YValue * 0.5f);
            else movTime = Mathf.Abs((statisticPopup.YValue - currentYValue) / statisticPopup.YValue * 0.5f);
            statisticPopup.Tween = DOTween.To(() => currentYValue, x => statisticPopup.MainContentRect.offsetMax = new Vector2(0, x), statisticPopup.YValue, movTime)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                statisticPopup.gameObject.SetActive(false);
                statisticPopup.Tween = null;
                isDrag = false;
            });
        }
    }
}
