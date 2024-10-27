using DG.Tweening;
using LitMotion;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupTouchpoint : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] private SearchPopup searchController;
    [SerializeField] private float thresholdPercent = 10f;
    bool isDrag = false;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (searchController.MotionHandle.IsActive() || isDrag) return;
        isDrag = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (searchController.MotionHandle.IsActive() || !isDrag) return;
        searchController.MainContentRect.offsetMax = new Vector2(0, eventData.position.y + searchController.YValue);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (searchController.MotionHandle.IsActive() || !isDrag) return;
        float threshold = -searchController.YValue * thresholdPercent / 100;
        float lowerThreshold = -threshold;
        float upperThreshold = +threshold;
        float currentYValue = searchController.MainContentRect.offsetMax.y;
        if (currentYValue >= lowerThreshold && currentYValue <= upperThreshold)
        {
            float movTime = Mathf.Abs(currentYValue / searchController.YValue * 2);
            searchController.Tween = DOTween.To(() => currentYValue, x => searchController.MainContentRect.offsetMax = new Vector2(0, x), 0, movTime)
            .SetEase(DG.Tweening.Ease.InCubic).OnComplete(() =>
            {
                searchController.Tween = null;
                isDrag = false;
            });
        }
        else
        {
            float movTime;
            if (currentYValue > 0) movTime = Mathf.Abs((currentYValue - searchController.YValue) / searchController.YValue * 0.5f);
            else movTime = Mathf.Abs((searchController.YValue - currentYValue) / searchController.YValue * 0.5f);
            searchController.Tween = DOTween.To(() => currentYValue, x => searchController.MainContentRect.offsetMax = new Vector2(0, x), searchController.YValue, movTime)
            .SetEase(DG.Tweening.Ease.InCubic)
            .OnComplete(() =>
            {
                searchController.gameObject.SetActive(false);
                searchController.Tween = null;
                isDrag = false;
            });
        }
    }
}
