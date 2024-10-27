using UnityEngine;

public class FitInParent : MonoBehaviour
{
    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        RectTransform parentRect = transform.parent.GetComponent<RectTransform>();
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = parentRect.rect.size;
    }
}
