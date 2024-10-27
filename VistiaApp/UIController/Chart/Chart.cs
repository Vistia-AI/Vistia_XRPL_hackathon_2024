using Cysharp.Threading.Tasks;
using Graph;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Chart
{
    [Serializable]
    public class Chart
    {
        [Header("Graph Settings")]
        [SerializeField] private Color32 pointColor;
        [SerializeField] private Color32 lineColor;
        [SerializeField] private float thickness = 5f;

        [Header("Prefabs")]
        [SerializeField] private Point pointPrefab;
        [SerializeField] private Line linePrefab;

        [SerializeField] private GraphContainer graphContainer;
        [SerializeField] private RectTransform rectTransform;

        [SerializeField] private string symbol = "BTCUSDT";

        // Data
        private List<Tuple<decimal, long>> data = new List<Tuple<decimal, long>>();

        // Components
        private ObjectPool<Point> pointPool;
        private ObjectPool<Line> linePool;

        // Size
        private float width;
        private float height;

        // Tracking
        private bool isDrawing = false;
        private Point currentPoint;
        private Point originalPoint;

        public bool IsDrawing { get => isDrawing; set => isDrawing = value; }
        public float Width { get => width; set => width = value; }
        public float Height { get => height; set => height = value; }

        public Action onFinishDraw;
        public Action<Tuple<decimal, long>> onCurrentDataChange;

        public void Start()
        {
            width = rectTransform.rect.size.x;
            height = rectTransform.rect.size.y;

            pointPool = new ObjectPool<Point>(pointPrefab, 10);
            linePool = new ObjectPool<Line>(linePrefab, 10);
        }

        public async void Draw(List<Tuple<decimal, long>> newData, float xScale, float yScale, decimal minY)
        {
            this.data = newData;
            await SpawnGameObjectsInBatchesAsync(10, 10, xScale, yScale, minY);
            onFinishDraw?.Invoke();
        }

        public async UniTask SpawnGameObjectsInBatchesAsync(int batchSize, int delayMilliseconds, float xScale, float yScale, decimal minY)
        {
            isDrawing = true;
            if (originalPoint != null)
                await ReturnAllMaterialsAsync(batchSize, delayMilliseconds);
            int spawned = 0;
            int dataCount = data.Count;
            Point previousPoint = null;
            while (spawned < dataCount)
            {
                for (int i = 0; i < batchSize && spawned < dataCount; i++)
                {
                    Point point = pointPool.Pull();
                    if (originalPoint == null)
                    {
                        originalPoint = point;
                    }
                    point.SetColor(pointColor);
                    point.transform.SetParent(graphContainer.PointContainer);
                    point.transform.localPosition = new Vector3(spawned * xScale - width / 2f, (float)(data[spawned].Item1 - minY) * yScale - height / 2f, 0);
                    point.SetThickness(thickness);

                    if (previousPoint != null)
                    {
                        Line line = linePool.Pull();
                        line.SetColor(lineColor);
                        line.transform.SetParent(graphContainer.LineContainer);
                        line.transform.localPosition = Vector3.zero;
                        point.PreviousLine = line;
                        previousPoint.NextLine = line;
                        line.SetPoints(previousPoint, point, thickness);
                    }

                    previousPoint = point;
                    spawned++;
                }
                await UniTask.Delay(delayMilliseconds);
            }
            isDrawing = false;
        }


        public Point GetPointAtIndex(int x)
        {
            if (isDrawing) return null;
            Point result = originalPoint;
            for (int i = 0; i < x; i++)
            {
                if (result.NextLine == null)
                {
                    return result;
                }
                else if (result.NextLine != null)
                {
                    result = result.NextLine.NextPoint;
                }
            }
            return result;
        }

        public Tuple<decimal, long, Point> GetDataAtIndex(int x)
        {
            if (isDrawing) return null;
            Tuple<decimal, long> result = data[x];
            Point temp = GetPointAtIndex(x);
            onCurrentDataChange?.Invoke(data[x]);
            SelectPoint(temp);
            return new Tuple<decimal, long, Point>(result.Item1, result.Item2, temp);
        }

        public void SelectPoint(Point point)
        {
            if (isDrawing || point == null) return;
            currentPoint?.OnDeselect(thickness, pointColor, graphContainer.PointContainer);
            currentPoint = point;
            currentPoint?.OnSelect(40, Color.black, graphContainer.TopLayer);
        }

        public async void ReturnAllMaterials()
        {
            await ReturnAllMaterialsAsync(10, 10);
        }

        public async UniTask ReturnAllMaterialsAsync(int batchSize, int delayMilliseconds)
        {
            int count = 0;
            Point currentPoint = originalPoint;
            for (; ; count++)
            {
                if (count == batchSize)
                {
                    await UniTask.Delay(delayMilliseconds);
                    count = 0;
                }
                Line previousLine = currentPoint.PreviousLine;
                Line nextLine = currentPoint.NextLine;
                Point nextPoint = null;
                currentPoint.PreviousLine = null;
                currentPoint.NextLine = null;
                currentPoint.ReturnToPool();
                if (previousLine != null)
                {
                    previousLine.NextPoint = null;
                    previousLine.PreviousPoint = null;
                    previousLine.ReturnToPool();
                }
                if (nextLine != null)
                {
                    nextPoint = nextLine.NextPoint;
                }
                if (nextPoint == null) break;
                currentPoint = nextPoint;
            }
            originalPoint = null;
        }
    }
}
