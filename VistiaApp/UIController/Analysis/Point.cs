using LitMotion;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Analysis
{
    public class Point : MonoBehaviour, IPoolable<Point>
    {
        static Color32 down = new Color32(170, 20, 20, 255);
        static Color32 up = new Color32(13, 135, 123, 255);
        [SerializeField] private Image iconImage;
        [SerializeField] private Image lineImage;
        [SerializeField] private Button button;
        private RectTransform lineRect;
        private BannerController bannerController;
        private Action<Point> returnAction;

        private string symbol;
        private float value = 0;
        private float changeRate = 0;
        //private Tween tween;
        private MotionHandle motionHandle;
        private MotionHandle colorHandle;
        public void Awake()
        {
            lineRect = lineImage.GetComponent<RectTransform>();
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            bannerController.SetBanner(symbol);
        }

        public void Init(string name, float value, float changeRate, BannerController banner)
        {
            string symbol = name.GetSymbolWithoutXRP();
            this.value = value;
            this.changeRate = changeRate;
            this.bannerController = banner;
            AlchemistAPIHandle.Instance.GetCurrencyLogo(symbol, OnGetSprite);
        }

        private void OnGetSprite(string symbol, Sprite sprite)
        {
            iconImage.sprite = sprite;
            this.symbol = symbol;
        }

        public void Initialize(Action<Point> returnAction)
        {
            this.returnAction = returnAction;
        }

        public void ReturnToPool()
        {
            /*lineImage.DOFade(0, 0);
            tween = iconImage.DOFade(0, 0.2f).OnComplete(() =>
            {
                this.returnAction?.Invoke(this);
                iconImage.DOFade(1, 0);
                lineImage.DOFade(1, 0);
            });*/

            lineImage.ChangeAlpha(0, 0);
            iconImage.ChangeAlpha(0, 0.2f, () =>
            {
                this.returnAction?.Invoke(this);
                iconImage.ChangeAlpha(1, 0);
                lineImage.ChangeAlpha(1, 0);
            });
        }

        public void SetCoordinate(float x, float m_y)
        {
            //tween?.Kill();
            if (motionHandle.IsActive()) motionHandle.Cancel();

            //lineImage.DOFade(0, 0);
            lineImage.ChangeAlpha(0, 0);

            if (changeRate < 0)
            {
                //lineImage.DOColor(down, 0.5f);
                lineImage.ChangeColor(down, 0.5f);
            }
            else
            {
                //lineImage.DOColor(up, 0.5f);
                lineImage.ChangeColor(up, 0.5f);
            }

            /*tween = transform.DOLocalMove(new Vector2(x, value / 100 * m_y - m_y / 2), 0.5f).OnComplete(() =>
            {
                lineImage.DOFade(1, 0.2f);
                lineRect.sizeDelta = Vector2.zero;
                if (changeRate < 0)
                {
                    lineRect.pivot = new Vector2(0.5f, 0f);
                    lineImage.transform.DOLocalMoveY(0, 0f);
                    lineRect.DOSizeDelta(new Vector2(4f, Mathf.Abs(changeRate) / 100 * m_y), 0.2f);

                }
                else
                {
                    lineRect.pivot = new Vector2(0.5f, 1f);
                    lineImage.transform.DOLocalMoveY(0, 0f);
                    lineRect.DOSizeDelta(new Vector2(4f, Mathf.Abs(changeRate) / 100 * m_y), 0.2f);
                }
            });*/

            Action onComplete = () =>
            {
                lineImage.ChangeAlpha(1, .2f);
                lineRect.sizeDelta = Vector2.zero;
                if (changeRate < 0)
                    lineRect.pivot = new Vector2(0.5f, 0f);
                else
                    lineRect.pivot = new Vector2(0.5f, 1f);
                lineImage.transform.localPosition = new Vector3(0, 0);
                lineRect.ChangeSizeDelta(new Vector2(4f, (Mathf.Abs(changeRate) / 100 * m_y) / this.transform.localScale.x), 0.2f);
            };

            motionHandle = transform.ChangeLocalPosition(new Vector3(x, value / 100 * m_y - m_y / 2), 0.5f, onComplete, Ease.InOutCubic);
        }

        private void OnDisable()
        {
            //tween?.Kill();
            if (motionHandle.IsActive()) motionHandle.Cancel();
        }
    }
}
