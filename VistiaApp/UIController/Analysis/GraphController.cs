using UIRangeSliderNamespace;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace Analysis
{
    public class GraphController : MonoBehaviour
    {
        [SerializeField] private Transform pointContainer;
        [SerializeField] private UIRangeSlider rangeSlider;
        [SerializeField] private Point pointPrefab;
        [SerializeField] private int maxSliderValue;
        [SerializeField] private Analysis.BannerController banner;
        [SerializeField] private Image averageLine;
        [SerializeField] private float averageLineWidth;
        [SerializeField] private TMP_Text averageTxt;
        [SerializeField] private float padding = 20;

        private List<ChartData> pointsData = new List<ChartData>();
        private ObjectPool<Point> pointPool;
        private List<Point> activePoint = new List<Point>();

        float m_width = 0;
        float m_height = 0;
        int m_widthValue;
        decimal averageRsi = 0;

        private void Start()
        {
            pointPool = new ObjectPool<Point>(pointPrefab, pointsData.Count);
            // padding = pointPrefab.gameObject.GetComponent<RectTransform>().rect.size.x / 2f;
            padding = 0f;
            RectTransform rt = pointContainer.gameObject.GetComponent<RectTransform>();

            /*m_width = rt.rect.size.x / canvas.scaleFactor;
            m_height = rt.rect.size.y / canvas.scaleFactor;*/

            m_width = rt.rect.size.x;
            m_height = rt.rect.size.y;
            // averageLine.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(m_width, averageLineWidth);
            averageLine.gameObject.SetActive(true);

            m_widthValue = maxSliderValue;

            rangeSlider.maxLimit = maxSliderValue;
            rangeSlider.minLimit = 1;
            rangeSlider.onValuesChanged.AddListener(OnSliderValueChanged);

            AlchemistAPIHandle.Instance.ChartDatasUpdatedStream += OnChartDataUpdate;
        }

        private void OnChartDataUpdate(List<ChartData> data)
        {
            if (data == null || data.Count == 0)
            {
                Debug.LogError("Data is null or empty");
                return;
            }
            rangeSlider.maxLimit = data.Count;
            this.pointsData = data;
            decimal totalRsi = 0;
            for (int i = 0; i < data.Count; i++)
            {
                totalRsi += data[i].Rsi;
            }
            averageRsi = totalRsi / data.Count;

            //averageLine.transform.DOLocalMoveY((float)averageRsi / 100 * m_height - m_height / 2, 1f);
            averageLine.transform.ChangeLocalPositionYTo((float)averageRsi / 100 * m_height - m_height / 2, 1f);

            averageTxt.text = "Avg: " + averageRsi.ToString("F2");
            OnSliderValueChanged(rangeSlider.valueMin, rangeSlider.valueMax);
        }

        private void OnSliderValueChanged(float min, float max)
        {
            if (max <= 0) return;
            int roundMin = Mathf.FloorToInt(min);
            int totalPoint = Mathf.CeilToInt(max) - roundMin + 1;
            int startPointIndex = (int)min - 1;
            float spaceBetweenPoint = (m_width - padding * 2) / (totalPoint + 1);

            totalPoint = Mathf.Clamp(totalPoint, 0, pointsData.Count - startPointIndex);

            if (activePoint.Count < totalPoint)
            {
                int count = totalPoint - activePoint.Count;
                for (int i = 0; i < count; i++)
                {
                    Point point = pointPool.Pull(Vector3.zero, pointContainer);
                    activePoint.Add(point);
                }
            }
            else if (activePoint.Count > totalPoint)
            {
                int count = activePoint.Count - totalPoint;
                for (int i = activePoint.Count - 1, j = count; i >= totalPoint && count > 0; i--, j--)
                {
                    activePoint[i].ReturnToPool();
                    activePoint.RemoveAt(i);
                }
            }
            // - (1 - (min - roundMin)) * spaceBetweenPoint;
            float startX = -m_width / 2 + padding;

            foreach (Point point in activePoint)
            {
                startX += spaceBetweenPoint;
                ChartData chartData = pointsData[startPointIndex];
                point.Init(chartData.Symbol, (float)chartData.Rsi, (float)chartData.Percentage_change, banner);
                float value = (float)chartData.Rsi;
                //value = Mathf.Clamp(value, 0, m_heightValue);
                point.SetCoordinate(startX, m_height);
                point.gameObject.SetActive(true);
                startPointIndex++;
            }
        }

        private void OnDestroy()
        {
            AlchemistAPIHandle.Instance.ChartDatasUpdatedStream += OnChartDataUpdate;
        }
    }
}
