using UnityEngine;
using UnityEngine.UI;

namespace Graph
{
    public class GraphController : Singleton<GraphController>
    {

        [Header("Prefabs")]
        [SerializeField] private GraphContainer graphContainerPrefabs;
        [SerializeField] private Point pointPrefab;
        [SerializeField] private Line linePrefab;

        [Header("UI")]
        [SerializeField] private Slider slider;
        [SerializeField] private Text label;
        [SerializeField] private PricePointController pricePointController;
        [SerializeField] private TimePointController timePointController;
        [SerializeField] private RectTransform parentRect;
        [SerializeField] private RectTransform rectTransform;
        public Slider Slider { get => slider; set => slider = value; }
        public Text Label { get => label; set => label = value; }
        public PricePointController PricePointController { get => pricePointController; set => pricePointController = value; }
        public TimePointController TimePointController { get => timePointController; set => timePointController = value; }

        [Header("Axis")]
        [SerializeField] private Transform horizontalLine;
        [SerializeField] private Transform verticalLine;
        public Transform HorizontalLine { get => horizontalLine; set => horizontalLine = value; }
        public Transform VerticalLine { get => verticalLine; set => verticalLine = value; }


        [SerializeField]
        private Graph graph;

        #region Preferences
        private float graphHeight;
        private float graphWidth;
        private Interval interval = Interval.One_Day;
        private OpenTime openTime = OpenTime.One_Year_Ago;
        public float GraphHeight { get => graphHeight; set => graphHeight = value; }
        public float GraphWidth { get => graphWidth; set => graphWidth = value; }
        public Interval Interval { get => interval; set => interval = value; }
        public OpenTime OpenTime { get => openTime; set => openTime = value; }
        #endregion

        #region Pooling
        private ObjectPool<GraphContainer> graphContainerPool;
        private ObjectPool<Point> pointPool;
        private ObjectPool<Line> linePool;

        public ObjectPool<GraphContainer> GraphContainerPool { get => graphContainerPool; set => graphContainerPool = value; }
        public ObjectPool<Point> PointPool { get => pointPool; set => pointPool = value; }
        public ObjectPool<Line> LinePool { get => linePool; set => linePool = value; }

        #endregion

        private IntervalButton currentSelectBtn;
        public IntervalButton CurrentSelectBtn { get => currentSelectBtn; set => currentSelectBtn = value; }

        private void Start()
        {

            slider.onValueChanged.AddListener((float value) =>
            {
                label.text = string.Empty;
                graph.SelectNextPoint(graph.GetPointAtIndex((int)value));
            });

            graphWidth = rectTransform.rect.size.x;
            graphHeight = rectTransform.rect.size.y;

            pointPool = new ObjectPool<Point>(pointPrefab, 10);
            linePool = new ObjectPool<Line>(linePrefab, 10);
            graphContainerPool = new ObjectPool<GraphContainer>(graphContainerPrefabs, 1);

            Init();
        }

        private void Init()
        {
            graph.Init();
        }

        [ContextMenu("Test")]
        public void Test()
        {
            ChangeSymbol("BTCUSDT");
        }

        public void ChangeSetting(Interval interval, OpenTime openTime, IntervalButton btn)
        {
            if (graph.IsDrawing) return;
            currentSelectBtn?.OnDeselect();
            currentSelectBtn = btn;
            currentSelectBtn?.OnSelect();
            if (this.interval != interval || this.openTime != openTime)
            {
                this.interval = interval;
                this.openTime = openTime;
                graph.GetGraphData(interval, openTime);
            }
        }

        public void ChangeSymbol(string symbol)
        {
            if (graph.IsDrawing) return;
            graph.GetGraphData(symbol, interval, openTime);
        }

        [ContextMenu("Reset Graph")]
        public void ResetGraph()
        {
            graph.ReturnAllMaterials();
        }
    }
}
