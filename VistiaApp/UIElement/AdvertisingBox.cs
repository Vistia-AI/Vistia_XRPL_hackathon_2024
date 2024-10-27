using System;
using UnityEngine;
using UnityEngine.UI;

public class AdvertisingBox : MonoBehaviour, IPoolable<AdvertisingBox>
{
    [SerializeField] private Image backGround;
    [SerializeField] private Button button;

    private float offset;
    private string url;
    private Vector3 lastPos;
    private RectTransform rectTransform;
    private Action<AdvertisingBox> returnAction;

    public float Offset { get => offset; set => offset = value; }

    public Image BackGround { get => backGround; set => backGround = value; }
    public string Url { get => url; set => url = value; }

    public void SavePos() => lastPos = transform.localPosition;

    public float GetSize() => rectTransform.sizeDelta.y;

    private void Awake()
    {
        button.onClick.AddListener(() => Application.OpenURL(url));
        rectTransform = GetComponent<RectTransform>();
    }
    public float Move(float offSetX)
    {
        transform.localPosition = new Vector3(lastPos.x + offSetX, lastPos.y, lastPos.z);

        return ReScale();
    }

    public float ReScale()
    {
        float t = Mathf.Clamp(Mathf.Abs(transform.localPosition.x / offset), 0f, 1f);
        float newY = Mathf.Lerp(450, 400, t);
        float newAlpha = Mathf.Lerp(255, 100, t);
        backGround.color = new Color32(255, 255, 255, (byte)newAlpha);
        rectTransform.sizeDelta = new Vector2(900, newY);
        transform.localScale = Vector2.one;
        return newY;
    }

    public void Initialize(Action<AdvertisingBox> returnAction)
    {
        this.returnAction = returnAction;
    }

    public void ReturnToPool()
    {
        this.returnAction?.Invoke(this);
    }
}
