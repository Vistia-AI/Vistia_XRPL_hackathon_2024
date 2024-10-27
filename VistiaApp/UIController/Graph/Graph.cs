using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using LitMotion;

namespace Graph
{
    [Serializable]
    public class Graph
    {
        private Point originalPoint;
        [SerializeField] private Color32 pointColor;
        [SerializeField] private Color32 lineColor;
        private GraphContainer graphContainer;
        private List<Tuple<decimal, long>> data = new List<Tuple<decimal, long>>();
        [SerializeField] private float thickness = 5f;
        [SerializeField] private string symbol = "BTCUSDT";

        private ObjectPool<Point> pointPool;
        private ObjectPool<Line> linePool;
        private GraphContainer graphContainerInstance;
        private float width;
        private float height;
        private UnityEngine.UI.Slider slider;
        private Text label;
        private PricePointController pricePointController;
        private TimePointController timePointController;
        private Transform horizontalLine;
        private Transform verticalLine;
        private Point currentPoint;
        private bool isDrawing = false;
        private OpenTime openTime;

        public bool IsDrawing { get => isDrawing; set => isDrawing = value; }

        public void Init()
        {
            this.horizontalLine = GraphController.Instance.HorizontalLine;
            this.verticalLine = GraphController.Instance.VerticalLine;
            this.label = GraphController.Instance.Label;
            this.slider = GraphController.Instance.Slider;
            this.pointPool = GraphController.Instance.PointPool;
            this.linePool = GraphController.Instance.LinePool;
            if (this.graphContainer == null)
            {
                this.graphContainer = GraphController.Instance.GraphContainerPool.Pull();
                this.graphContainer.transform.SetParent(GraphController.Instance.transform);
            }
            this.width = GraphController.Instance.GraphWidth;
            this.height = GraphController.Instance.GraphHeight;
            this.pricePointController = GraphController.Instance.PricePointController;
            this.timePointController = GraphController.Instance.TimePointController;

            KLineFetcher.Instance.GetHistoricalKlinesAsync(symbol.GetSymbolWithXRP(), GraphController.Instance.Interval, GraphController.Instance.OpenTime, Draw);
        }

        public void GetGraphData(string symbol, Interval interval, OpenTime openTime)
        {
            if (this.symbol.Equals(symbol)) return;
            this.symbol = symbol;
            GetGraphData(interval, openTime);
        }

        public void GetGraphData(Interval interval, OpenTime openTime)
        {
            this.openTime = openTime;
            KLineFetcher.Instance.GetHistoricalKlinesAsync(symbol, interval, openTime, Draw);
        }

        private async void Draw(List<Tuple<decimal, long>> newData, decimal maxY, decimal minY)
        {
            if (newData == null || newData.Count == 0)
            {
                Debug.LogError("No data to draw");
                return;
            }
            Tuple<decimal, decimal> tuple = pricePointController.SetPricePoints(maxY, minY);
            timePointController.SetTimePoint(newData[0].Item2, newData[^1].Item2, openTime);
            maxY = tuple.Item2;
            minY = tuple.Item1;

            float xScale = width / (newData.Count - 1);
            float yScale = height / (float)(maxY - minY);

            data = newData;

            pricePointController.SetPricePointsPos(yScale, minY);
            timePointController.SetTimePointsPos(width);
            await SpawnGameObjectsInBatchesAsync(10, 10, xScale, yScale, minY);
            SelectOriginalPoint(1f);
        }

        public async UniTask SpawnGameObjectsInBatchesAsync(int batchSize, int delayMilliseconds, float xScale, float yScale, decimal minY)
        {
            slider.interactable = false;
            isDrawing = true;
            if (originalPoint != null)
                await ReturnAllMaterialsAsync(batchSize, delayMilliseconds);
            else
                slider.value = 0;
            slider.maxValue = data.Count - 1;
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
                    point.transform.localPosition = new Vector3(spawned * xScale, (float)(data[spawned].Item1 - minY) * yScale, 0);
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

        public void SelectNextPoint(Point point)
        {
            if (isDrawing || point == null) return;
            currentPoint?.OnDeselect(thickness, pointColor, graphContainer.PointContainer);
            currentPoint = point;
            currentPoint?.OnSelect(40, Color.black, graphContainer.TopLayer);
            if (point != null)
            {
                decimal price = data[(int)slider.value].Item1;
                long time = data[(int)slider.value].Item2;
                pricePointController.SetCurrentPricePos(price);
                label.text += $"{DateTimeOffset.FromUnixTimeMilliseconds(time)}";
                horizontalLine.position = new Vector3(horizontalLine.position.x, point.transform.position.y, 0f);
                verticalLine.position = new Vector3(point.transform.position.x, verticalLine.position.y, 0f);
            }
        }

        public void SelectOriginalPoint(float movTime)
        {
            if (isDrawing) return;
            currentPoint?.OnDeselect(thickness, pointColor, graphContainer.PointContainer);
            currentPoint = originalPoint;
            decimal price = data[0].Item1;
            long time = data[0].Item2;
            pricePointController.SetCurrentPricePos(price, movTime);
            label.text += $"{DateTimeOffset.FromUnixTimeMilliseconds(time)}";
            //horizontalLine.DOMoveY(currentPoint.transform.position.y, movTime);
            /*verticalLine.DOMoveX(currentPoint.transform.position.x, movTime).onComplete = () =>
            {
                currentPoint?.OnSelect(40, Color.black, graphContainer.TopLayer);
            };*/

            Action action = () =>
            {
                currentPoint?.OnSelect(40, Color.black, graphContainer.TopLayer);
                slider.interactable = true;
            };
            horizontalLine.ChangePositionYTo(currentPoint.transform.position.y, movTime, null, Ease.InOutCubic);
            verticalLine.ChangePositionXTo(currentPoint.transform.position.x, movTime, action, Ease.InOutCubic);

            /*LMotion.Create(horizontalLine.transform.position.y, currentPoint.transform.position.y, movTime)
                .WithEase(LitMotion.Ease.InOutCubic)
                .BindWithState(horizontalLine, (y, target) => horizontalLine.transform.position = new Vector3(target.position.x, y, 0));*/
            /*LMotion.Create(verticalLine.transform.position.x, currentPoint.transform.position.x, movTime)
                .WithEase(LitMotion.Ease.InOutCubic)
                .WithOnComplete(() => currentPoint?.OnSelect(40, Color.black, graphContainer.TopLayer))
                .BindWithState(verticalLine, (x, target) => verticalLine.transform.position = new Vector3(x, target.position.y, 0));*/


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
            label.text = string.Empty;

            //slider.DOValue(0f, .5f);
            //await UniTask.Delay(500);

            /*await LMotion.Create(slider.value, 0f, .5f)
                .WithEase(LitMotion.Ease.InOutCubic)
                .BindWithState(slider, (value, target) => slider.value = value).ToUniTask();*/

            await slider.ChangeValueAsync(0f, .5f, null, Ease.InOutCubic);
        }
    }
}
