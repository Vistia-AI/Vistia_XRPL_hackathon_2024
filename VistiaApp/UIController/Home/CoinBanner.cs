using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;

public class CoinBanner : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    [Space(5f), Header("Coin Banner"), Space(5f)]
    [SerializeField] private List<AdvertisingBox> coinBanners = new List<AdvertisingBox>();
    private ObjectPool<AdvertisingBox> coinBannerPool;
    [SerializeField] private AdvertisingBox coinBannerPrefab;
    [SerializeField] private Transform container;

    [Space(5f), Header("Dots"), Space(5f)]
    [SerializeField] private List<Image> dots = new List<Image>();
    [SerializeField] private Image dotPrefab;
    [SerializeField] private Transform dotContainer;
    [SerializeField] private Sprite focused;
    [SerializeField] private Sprite unfocused;

    [Space(5f), Header("Items"), Space(5f)]
    [SerializeField] private List<Sprite> bannerDatas = new List<Sprite>();
    [SerializeField] private List<string> Urls = new List<string>();

    [Space(5f), Header("Options"), Space(5f)]
    [SerializeField] private float movingTime = 2f;
    [SerializeField] private float timeDelay = 15f;
    [SerializeField] private float timeCount = 0f;
    [SerializeField] private float offset = 800;
    [SerializeField] private float bannerWidth = 900;
    [SerializeField] private bool enableAutoSlide = true;
    [SerializeField] private Canvas canvas;
    private float despawnThreshold;
    private float spawnThreshold;
    private int currentItemIndex = 0;
    private AdvertisingBox middleCoinBannerBox;
    private Sequence sequence;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        float screenWidth = Screen.width / canvas.scaleFactor;
        int elementCount = Mathf.CeilToInt((screenWidth + offset - bannerWidth) / offset) + 2;
        if (elementCount % 2 == 0) elementCount++;
        float symmetryOffset = elementCount * offset;
        float padding = symmetryOffset + bannerWidth;
        float extendPadding = 0.5f * bannerWidth + offset;
        coinBannerPool = new ObjectPool<AdvertisingBox>(coinBannerPrefab, elementCount + 1);
        despawnThreshold = (padding + extendPadding) * 0.5f;
        spawnThreshold = (screenWidth - bannerWidth) * 0.5f;

        float firstPoint = -(elementCount - 1) * offset * 0.5f;
        int middleIndex = elementCount / 2;
        int dataCount = middleIndex - 1;

        for (int i = 0; i < elementCount; i++)
        {
            AdvertisingBox temp = coinBannerPool.Pull(new Vector3(firstPoint, 0, 0), container);
            firstPoint += offset;
            temp.Offset = offset;
            temp.SavePos();
            coinBanners.Add(temp);
            temp.ReScale();

            temp.BackGround.sprite = bannerDatas[dataCount];
            temp.Url = Urls[dataCount];
            dataCount++;
            if (dataCount >= bannerDatas.Count)
                dataCount = 0;

            if (i == middleIndex)
            {
                middleCoinBannerBox = temp;
                middleCoinBannerBox.BackGround.color = Color.white;
            }
        }

        for (int i = 0; i < bannerDatas.Count; i++)
        {
            Image dot = Instantiate(dotPrefab, dotContainer);
            dots.Add(dot);
            if (i == 0)
                dot.sprite = focused;
        }
    }

    public void OnBeginDrag(PointerEventData data)
    {
        StopSequence();
    }

    public void OnDrag(PointerEventData data)
    {
        float difference = data.pressPosition.x - data.position.x;
        CoinBannerUpdateOnMove(difference);
    }

    private void CoinBannerUpdateOnMove(float difference)
    {
        float maxY = 0;
        AdvertisingBox currentMiddleBox = null;
        for (int i = 0; i < coinBanners.Count; i++)
        {
            float currentY = coinBanners[i].Move(-difference);
            if (currentY > maxY)
            {
                maxY = currentY;
                currentMiddleBox = coinBanners[i];
            }
        }

        if (currentMiddleBox != middleCoinBannerBox)
        {
            dots[currentItemIndex].sprite = unfocused;

            if (currentMiddleBox.transform.localPosition.x > middleCoinBannerBox.transform.localPosition.x)
                currentItemIndex++;
            else if (currentMiddleBox.transform.localPosition.x < middleCoinBannerBox.transform.localPosition.x)
                currentItemIndex--;

            if (currentItemIndex < 0)
            {
                currentItemIndex = bannerDatas.Count - 1;
            }
            else if (currentItemIndex >= bannerDatas.Count)
            {
                currentItemIndex = 0;
            }

            dots[currentItemIndex].sprite = focused;

            //middleCoinBannerBox.BackGround.color = new Color32(255, 255, 255, 100);
            middleCoinBannerBox = currentMiddleBox;
            //middleCoinBannerBox.BackGround.color = Color.white;
        }

        if (coinBanners[0].transform.localPosition.x < -despawnThreshold)
        {
            AdvertisingBox temp = coinBanners[0];
            coinBanners.Remove(coinBanners[0]);
            temp.ReturnToPool();
        }
        else if (coinBanners[^1].transform.localPosition.x > despawnThreshold)
        {
            AdvertisingBox temp = coinBanners[^1];
            coinBanners.Remove(temp);
            temp.ReturnToPool();
        }

        int centerBannerIndex = coinBanners.IndexOf(middleCoinBannerBox);

        if (coinBanners[0].transform.localPosition.x > -spawnThreshold)
        {
            AdvertisingBox temp = coinBannerPool.Pull(coinBanners[0].transform.localPosition - new Vector3(offset, 0, 0) + new Vector3(difference, 0, 0), container);
            temp.SavePos();
            temp.Offset = offset;

            int indexOffset = centerBannerIndex + 1;
            int dataIndex = currentItemIndex - indexOffset;
            if (dataIndex < 0)
                dataIndex = bannerDatas.Count + dataIndex;
            temp.BackGround.sprite = bannerDatas[indexOffset];

            coinBanners.Insert(0, temp);
        }
        else if (coinBanners[^1].transform.localPosition.x < spawnThreshold)
        {
            AdvertisingBox temp = coinBannerPool.Pull(coinBanners[^1].transform.localPosition + new Vector3(offset, 0, 0) + new Vector3(difference, 0, 0), container);
            temp.SavePos();
            temp.Offset = offset;

            int indexOffset = coinBanners.Count - centerBannerIndex;
            int dataIndex = currentItemIndex + indexOffset;
            if (dataIndex >= bannerDatas.Count)
                dataIndex = dataIndex - bannerDatas.Count;
            temp.BackGround.sprite = bannerDatas[dataIndex];
            coinBanners.Add(temp);
        }
    }

    private void ReturnToCenter()
    {
        Vector3 distance = -middleCoinBannerBox.transform.localPosition;
        float realMoveTime = movingTime * Mathf.Abs(distance.x) / offset;
        sequence = DOTween.Sequence();
        foreach (AdvertisingBox banner in coinBanners)
        {
            sequence.Join(banner.transform.DOLocalMove(banner.transform.localPosition + distance, realMoveTime));
        }
        sequence
        .OnUpdate(() =>
        {
            foreach (AdvertisingBox banner in coinBanners)
            {
                banner.ReScale();
            }
        })
        .OnComplete(() =>
        {
            SaveBannerPos();
        });
    }

    private void SlideToNextItem()
    {
        StartCoroutine(SlideToNextItemCo(offset));
    }

    private IEnumerator SlideToNextItemCo(float maxDistance)
    {
        float currentDistance = 0f;
        float t = 0;
        while (true)
        {
            t += Time.deltaTime / movingTime;
            currentDistance = Mathf.Lerp(0f, maxDistance, LinearToOutQuad(t));
            CoinBannerUpdateOnMove(currentDistance);
            yield return null;
            if (t >= 1f) break;
        }
        SaveBannerPos();
    }

    private float LinearToOutQuad(float t)
    {
        // Ensure t is clamped between 0 and 1
        t = Mathf.Clamp01(t);

        // OutQuad easing function: 1 - (1 - t)^2
        return 1 - (1 - t) * (1 - t);
    }

    private void StopSequence()
    {
        enableAutoSlide = false;
        StopAllCoroutines();
        sequence?.Kill();
        SaveBannerPos();
    }

    private void SaveBannerPos()
    {
        foreach (AdvertisingBox banner in coinBanners)
        {
            banner.SavePos();
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        SaveBannerPos();
        ReturnToCenter();
        enableAutoSlide = true;
        timeCount = 0;
    }

    private void Update()
    {
        if (!enableAutoSlide) return;
        timeCount += Time.deltaTime;
        if (timeCount >= timeDelay)
        {
            timeCount = 0;
            SlideToNextItem();

        }
    }

    private void OnDestroy()
    {
        //AlchemistAPIHandle.Instance.OnCoinDatasUpdated -= OnC
    }
}
