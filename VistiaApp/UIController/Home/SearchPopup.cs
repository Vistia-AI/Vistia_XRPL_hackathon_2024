using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using LitMotion;
using Cysharp.Threading.Tasks;
public class SearchPopup : PopupBase
{
    [SerializeField] private SearchButton prefabBtn;
    [SerializeField] private Transform container;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private RectTransform mainContentRect;
    [SerializeField] private Button closeButton;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform scrollRectRT;
    private float threshold;  // Threshold for triggering loading more content

    private List<SearchButton> searchButtons = new List<SearchButton>();

    private ObjectPool<SearchButton> buttonPool;
    private float yValue;
    private Tween tween;
    private MotionHandle motionHandle;

    public RectTransform MainContentRect { get => mainContentRect; set => mainContentRect = value; }
    public float YValue { get => yValue; set => yValue = value; }
    public Tween Tween { get => tween; set => tween = value; }
    public MotionHandle MotionHandle { get => motionHandle; set => motionHandle = value; }

    private bool isAvaialble = false;
    private bool isContinue = false;

    private int skip = 0;
    private int limit = 20;
    private string searchKey = "/";
    private int lastDataCount = 0;
    private bool isFirstTime = true;

    private void Awake()
    {
        buttonPool = new ObjectPool<SearchButton>(prefabBtn, 0);
        inputField.onEndEdit.AddListener(OnEndEdit);
        yValue = -mainContentRect.rect.height;
        mainContentRect.offsetMax = new Vector2(0, yValue);
        closeButton.onClick.AddListener(OnClose);
        scrollRect.onValueChanged.AddListener((v) => CheckScrollPosition());
    }

    private void OnEnable()
    {

        // use dotween
        /* tween = DOTween.To(() => yValue, x => mainContentRect.offsetMax = new Vector2(0, x), 0, 0.5f)
             .SetEase(DG.Tweening.Ease.InCubic)
             .OnComplete(() =>
             {
                 tween = null;
                 if (searchButtons.Count == 0) OnEndEdit(string.Empty);
                 isAvaialble = true;
             });*/

        // use LitMotion
        motionHandle = LMotion.Create(yValue, 0, 0.5f)
            .WithEase(LitMotion.Ease.InOutCubic)
            .WithOnComplete(() =>
            {
                if (searchButtons.Count == 0) OnEndEdit(string.Empty);
                isAvaialble = true;
            })
            .BindWithState(mainContentRect, (x, target) => mainContentRect.offsetMax = new Vector2(0, x));
    }

    private void OnClose()
    {
        isAvaialble = false;
        /*if (tween != null) return;
        tween = DOTween.To(() => 0, x => mainContentRect.offsetMax = new Vector2(0, x), yValue, 0.5f)
            .SetEase(DG.Tweening.Ease.InCubic).OnComplete(() =>
            {
                gameObject.SetActive(false);
                tween = null;
            });*/

        if (motionHandle.IsActive()) return;
        motionHandle = LMotion.Create(Vector2.zero, new Vector2(0, yValue), .5f)
            .WithOnComplete(() => gameObject.SetActive(false))
        .BindWithState(mainContentRect, (offset, target) => mainContentRect.offsetMax = offset);
    }


    private void CheckScrollPosition()
    {
        if (!isAvaialble) return;
        float size = content.sizeDelta.y - scrollRectRT.sizeDelta.y;
        threshold = content.sizeDelta.y / searchButtons.Count;

        if (size < 0 && content.localPosition.y >= threshold) // Bottom
        {
            scrollRect.OnEndDrag(new PointerEventData(EventSystem.current));
            OnEndEdit(searchKey);
        }
        else if (size > 0 && content.localPosition.y >= size + threshold) // Bottom
        {
            scrollRect.OnEndDrag(new PointerEventData(EventSystem.current));
            OnEndEdit(searchKey);
        }
        else if (content.localPosition.y <= -threshold) // Top
        {
            scrollRect.OnEndDrag(new PointerEventData(EventSystem.current));
            OnEndEdit(searchKey);
        }
    }

    public async UniTask OnCloseAsync()
    {
        /*if (tween != null) return;
        tween = DOTween.To(() => 0, x => mainContentRect.offsetMax = new Vector2(0, x), yValue, 0.5f)
            .SetEase(DG.Tweening.Ease.InCubic).OnComplete(() =>
            {
                gameObject.SetActive(false);
                tween = null;
            });*/

        if (motionHandle.IsActive()) return;
        motionHandle = LMotion.Create(Vector2.zero, new Vector2(0, yValue), .5f)
            .WithOnComplete(() => gameObject.SetActive(false))
        .BindWithState(mainContentRect, (offset, target) => mainContentRect.offsetMax = offset);
        await UniTask.Delay(500);
    }

    private void OnEndEdit(string text)
    {
        if (string.IsNullOrEmpty(text) && !isFirstTime) return;
        isFirstTime = false;
        if (searchKey == text)
        {
            if (lastDataCount < limit) return;
            else
            {
                isContinue = true;
                skip += limit;
            }
        }
        else
        {
            searchKey = text;
            skip = 0;
        }
        isAvaialble = false;
        inputField.interactable = false;
        AlchemistAPIHandle.Instance.SearchCurrency(text, skip, limit, SpawnCointab);
    }

    private void SpawnCointab(List<Currency> dataList, long code)
    {
        inputField.interactable = true;
        int totalRecord = dataList.Count;
        if (!isContinue)
        {
            if (searchButtons.Count < totalRecord)
            {
                int count = totalRecord - searchButtons.Count;
                for (int i = 0; i < count; i++)
                {
                    SearchButton point = buttonPool.Pull(Vector3.zero, container);
                    searchButtons.Add(point);
                }
            }
            else if (searchButtons.Count > totalRecord)
            {
                int count = searchButtons.Count - totalRecord;
                for (int i = searchButtons.Count - 1, j = count; i >= totalRecord && count > 0; i--, j--)
                {
                    searchButtons[i].ReturnToPool();
                    searchButtons.RemoveAt(i);
                }
            }

            for (int i = 0; i < totalRecord; i++)
            {
                searchButtons[i].SetButton(dataList[i]);
                searchButtons[i].transform.localScale = Vector3.one;
            }
        }
        else
        {
            int currentCount = searchButtons.Count;
            for (int i = 0; i < totalRecord; i++)
            {
                SearchButton point = buttonPool.Pull(Vector3.zero, container);
                searchButtons.Add(point);
            }

            for (int i = currentCount; i < searchButtons.Count; i++)
            {
                searchButtons[i].SetButton(dataList[i - currentCount]);
                searchButtons[i].transform.localScale = Vector3.one;
            }
        }
        lastDataCount = totalRecord;
        isContinue = false;
        Invoke(nameof(MakeAvaiable), 0.5f);
    }

    private void MakeAvaiable()
    {
        isAvaialble = true;
    }
}
