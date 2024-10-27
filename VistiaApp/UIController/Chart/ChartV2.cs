using Cysharp.Threading.Tasks;
using Graph;
using System.Collections.Generic;
using System;
using UnityEngine;
using LitMotion;
namespace Chart
{
    [Serializable]
    public class ChartV2<T1, T2, TCal1, TCal2> where TCal1 : struct where TCal2 : struct
    {
        [Header("Graph Settings")]
        [SerializeField] private Color32 pointColor;
        [SerializeField] private Color32 lineColor;
        [SerializeField] private float thickness = 5f;
        [SerializeField] private float highlightThickness = 40f;
        [SerializeField] private DrawMode drawMode = DrawMode.Auto;
        [SerializeField] private int pointLimit = 500;
        [SerializeField] private float scaleX = 1f;
        //[SerializeField] private float smoothLengh = 0.1f

        [Header("Prefabs")]
        [SerializeField] private Point pointPrefab;
        [SerializeField] private Line linePrefab;

        [SerializeField] private GraphContainer graphContainer;
        [SerializeField] private RectTransform rectTransform;

        [SerializeField] private string symbol = "BTCUSDT";

        // Data
        private List<Tuple<T1, T2>> data = new List<Tuple<T1, T2>>();

        public List<Tuple<T1, T2>> Data => data;

        public Tuple<T1, T2> GetDataAtIndex(int index)
        {
            if (index < 0 || index >= data.Count) return null;

            return data[index];
        }

        // Components
        private static ObjectPool<Point> pointPool;
        private static ObjectPool<Line> linePool;

        // Size
        private float width;
        private float height;

        private float xScale;
        private float yScale;

        private decimal minY;

        // Tracking
        private bool isDrawing = false;
        private Point currentPoint;
        private Point originalPoint;

        public bool IsDrawing { get => isDrawing; set => isDrawing = value; }
        public float Width { get => width; set => width = value; }
        public float Height { get => height; set => height = value; }

        public Action onFinishDraw;

        public Func<T1, TCal1> convertT1ToCalc;
        public Func<T2, TCal2> convertT2ToCalc;

        public Action<Tuple<T1, T2>> onCurrentDataChange;

        private int offset = 1;

        private List<Point> points = new List<Point>();

        public ChartV2()
        {
            if (!IsNumericType(typeof(TCal1)) || !IsNumericType(typeof(TCal2)))
                throw new InvalidOperationException($"TCalc1 ({typeof(TCal1)}) or TCalc2 ({typeof(TCal2)}) must be a numeric type.");
            if (typeof(T1) == typeof(TCal1) && typeof(T2) == typeof(TCal2))
            {
                convertT1ToCalc = (T1 t1) => (TCal1)(object)t1;
                convertT2ToCalc = (T2 t2) => (TCal2)(object)t2;
            }
            //else throw new Exception("Invalid type conversion, you must provide a conversion function.");
        }

        public ChartV2(Func<T1, TCal1> convertT1ToCalc, Func<T2, TCal2> convertT2ToCalc)
        {
            if (!IsNumericType(typeof(TCal1)) || !IsNumericType(typeof(TCal2)))
                throw new InvalidOperationException($"TCalc1 ({typeof(TCal1)}) or TCalc2 ({typeof(TCal2)}) must be a numeric type.");
            this.convertT1ToCalc = convertT1ToCalc;
            this.convertT2ToCalc = convertT2ToCalc;
        }

        private bool IsNumericType(Type type)
        {
            return type == typeof(byte) || type == typeof(sbyte) ||
                   type == typeof(short) || type == typeof(ushort) ||
                   type == typeof(int) || type == typeof(uint) ||
                   type == typeof(long) || type == typeof(ulong) ||
                   type == typeof(float) || type == typeof(double) ||
                   type == typeof(decimal);
        }

        public Vector3 ChangeScaleX(float amount)
        {
            scaleX += amount;

            scaleX = Mathf.Max(scaleX, 1f);

            Vector3 newScale = new Vector3(scaleX, 1, 1);

            graphContainer.transform.localScale = newScale;

            int totalPoint = points.Count;
            int minRange = Mathf.CeilToInt(totalPoint / 2 - totalPoint / (2 * scaleX));
            int maxRange = Mathf.FloorToInt(totalPoint / 2 + totalPoint / (2 * scaleX));
            Debug.Log($"Total Point: {totalPoint}, Min Range: {minRange}, Max Range: {maxRange}");

            for (int i = 0; i < totalPoint; i++)
            {
                if (i <= minRange || i >= maxRange)
                {
                    points[i].gameObject.SetActive(false);
                    points[i].PreviousLine?.gameObject.SetActive(false);
                    points[i].NextLine?.gameObject.SetActive(false);
                }
                else
                {
                    points[i].gameObject.SetActive(true);
                    points[i].PreviousLine?.gameObject.SetActive(true);
                    points[i].NextLine?.gameObject.SetActive(true);
                }
            }

            return newScale;
        }


        public void Init()
        {
            width = rectTransform.rect.size.x;
            height = rectTransform.rect.size.y;

            if (pointPool == null)
                pointPool = new ObjectPool<Point>(pointPrefab, 10);
            if (linePool == null)
                linePool = new ObjectPool<Line>(linePrefab, 10);
        }

        public async void Draw(List<Tuple<T1, T2>> newData, float xScale, float yScale, decimal minY, float drawTime = .5f)
        {
            int segmentNumber = Mathf.FloorToInt(pointLimit / newData.Count) - 1;

            isDrawing = true;
            this.xScale = xScale;
            this.yScale = yScale;
            this.minY = minY;
            this.data = newData;

            if (originalPoint != null)
            {
                await ReturnAllMaterialsAsync(10, 10);
                points.Clear();
            }


            if (drawMode == DrawMode.Auto)
            {
                if (segmentNumber < 1)
                {
                    await DrawWithNormalMode(newData, xScale, yScale, minY, drawTime);
                }
                else
                {
                    await DrawWithCatmullRomSplineList(newData, xScale, yScale, minY, segmentNumber, drawTime);
                }
            }
            else if (drawMode == DrawMode.Line)
            {
                await DrawWithNormalMode(newData, xScale, yScale, minY, drawTime);
            }
            else if (drawMode == DrawMode.CatmullRomSpline)
            {
                if (segmentNumber == 0) segmentNumber++;
                await DrawWithCatmullRomSplineList(newData, xScale, yScale, minY, segmentNumber, drawTime);
            }
        }

        public async UniTask DrawWithNormalMode(List<Tuple<T1, T2>> newData, float xScale, float yScale, decimal minY, float drawTime = .5f)
        {
            offset = 1;
            int toMiliseconds = (int)(drawTime * 1000);
            int count = data.Count;
            int batchSize = Mathf.CeilToInt(count / 50f);
            int delay = Mathf.Max(Mathf.RoundToInt(toMiliseconds * batchSize / count), 1);
            Debug.Log($"Count: {count}, Batch Size: {batchSize}, Delay: {delay}, Draw Time: {drawTime}");
            await SpawnGameObjectsInBatchesAsync(batchSize, delay, xScale, yScale, minY);
        }

        public async UniTask SpawnGameObjectsInBatchesAsync(int batchSize, int delayMilliseconds, float xScale, float yScale, decimal minY)
        {
            int spawned = 0;
            int dataCount = data.Count;
            Point previousPoint = null;
            while (spawned < dataCount)
            {
                for (int i = 0; i < batchSize && spawned < dataCount; i++)
                {
                    Point point = CreatePoint(new Vector3(spawned * xScale - width / 2f, (float)(Convert.ToDecimal(convertT1ToCalc(data[spawned].Item1)) - minY) * yScale - height / 2f, 0));

                    if (originalPoint == null)
                    {
                        originalPoint = point;
                    }

                    if (previousPoint != null)
                    {
                        CreateLine(previousPoint, point);
                    }

                    previousPoint = point;
                    spawned++;
                }
                points.Add(previousPoint);
                await UniTask.Delay(delayMilliseconds);
            }
            isDrawing = false;
            onFinishDraw?.Invoke();
        }

        public async UniTask DrawWithCatmullRomSplineList(List<Tuple<T1, T2>> newData, float xScale, float yScale, decimal minY, int segmentNumber, float drawTime = .5f)
        {
            offset = segmentNumber + 1;

            for (int i = 0; i < data.Count; i++)
            {
                Point point = CreatePoint(new Vector3(i * xScale - width / 2f, (float)(Convert.ToDecimal(convertT1ToCalc(data[i].Item1)) - minY) * yScale - height / 2f, 0));

                points.Add(point);

                if (originalPoint == null)
                {
                    originalPoint = point;
                }
            }

            Vector3 virtualStart = points[0].transform.localPosition - (points[1].transform.localPosition - points[0].transform.localPosition);  // p0' = p0 - (p1 - p0)
            Vector3 virtualEnd = points[data.Count - 1].transform.localPosition + (points[data.Count - 1].transform.localPosition - points[data.Count - 2].transform.localPosition);  // p3' = p3 + (p3 - p2)

            Point prePoint = null;

            for (int i = -1; i < data.Count - 2; i++)
            {
                Vector3 p0 = i == -1 ? virtualStart : points[i].transform.localPosition;
                Point p1 = points[i + 1];
                Vector3 p2 = points[i + 2].transform.localPosition;
                Vector3 p3 = i == data.Count - 3 ? virtualEnd : points[i + 3].transform.localPosition;
                CatmullRomCurve curve = new CatmullRomCurve(p0, p1, p2, p3, 0.5f);

                Point current = DrawCurveSegment(curve, segmentNumber + 1, prePoint);

                prePoint = current;

                await UniTask.Delay(10);
            }

            isDrawing = false;
            onFinishDraw?.Invoke();
        }

        private Point CreatePoint(Vector3 position)
        {
            Point point = pointPool.Pull();

            point.SetColor(pointColor);
            point.transform.SetParent(graphContainer.PointContainer);
            point.transform.localPosition = position;
            point.SetThickness(thickness);

            return point;
        }

        private Line CreateLine(Point point1, Point point2)
        {
            Line line = linePool.Pull();

            line.SetColor(lineColor);
            line.transform.SetParent(graphContainer.LineContainer);
            line.transform.localPosition = Vector3.zero;
            line.SetPoints(point1, point2, thickness);
            point2.PreviousLine = line;
            point1.NextLine = line;

            return line;
        }

        public Point GetPointAtIndex(int x)
        {
            if (isDrawing) return null;
            x = x + x * (offset - 1);
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

        public Tuple<T1, T2, Point> GetDataAndHighLightPointAtIndex(int x)
        {
            if (isDrawing) return null;
            Tuple<T1, T2> result = data[x];
            Point temp = GetPointAtIndex(x);
            onCurrentDataChange?.Invoke(data[x]);
            SelectPoint(temp);
            return new Tuple<T1, T2, Point>(result.Item1, result.Item2, temp);
        }

        public void SelectPoint(Point point)
        {
            if (isDrawing || point == null) return;
            currentPoint?.OnDeselect(thickness, pointColor, graphContainer.PointContainer);
            currentPoint = point;
            currentPoint?.OnSelect(highlightThickness, pointColor, graphContainer.TopLayer);
            currentPoint?.transform.SetAsFirstSibling();
        }

        MotionHandle customPoint;

        public Point DrawCustomPoint(int index, TCal1 data, Color color, float size)
        {
            if (isDrawing) return null;
            Point point;

            point = CreatePoint(new Vector3(index * xScale - width / 2f, (float)(Convert.ToDecimal(data) - minY) * yScale - height / 2f, 0));
            point.OnSelect(size, color, graphContainer.TopLayer);


            return point;
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

        float t = 0.5f;

        Point DrawCurveSegment(CatmullRomCurve curve, int detail, Point prePoint)
        {
            Point prev = curve.p1;

            if (prePoint != null)
            {
                CreateLine(prePoint, prev);
            }
            //Vector2 prev = curve.p1.transform.position;

            for (int i = 1; i < detail; i++)
            {
                float t = i / (detail - 1f);
                Vector2 pt = curve.GetPoint(t);
                Point current = CreatePoint(pt);
                CreateLine(prev, current);
                prev = current;
            }

            return prev;
        }

        public struct CatmullRomCurve
        {
            public Vector2 p0, p2, p3;
            public Point p1;
            public float alpha;

            public CatmullRomCurve(Vector2 p0, Point p1, Vector2 p2, Vector2 p3, float alpha)
            {
                (this.p0, this.p1, this.p2, this.p3) = (p0, p1, p2, p3);
                this.alpha = alpha;
            }

            // Evaluates a point at the given t-value from 0 to 1
            public Vector2 GetPoint(float t)
            {
                // calculate knots
                const float k0 = 0;
                float k1 = GetKnotInterval(p0, p1);
                float k2 = GetKnotInterval(p1, p2) + k1;
                float k3 = GetKnotInterval(p2, p3) + k2;

                // evaluate the point
                float u = Mathf.LerpUnclamped(k1, k2, t);
                Vector2 A1 = Remap(k0, k1, p0, p1, u);
                Vector2 A2 = Remap(k1, k2, p1, p2, u);
                Vector2 A3 = Remap(k2, k3, p2, p3, u);
                Vector2 B1 = Remap(k0, k2, A1, A2, u);
                Vector2 B2 = Remap(k1, k3, A2, A3, u);
                return Remap(k1, k2, B1, B2, u);
            }

            Vector2 Remap(float a, float b, Point c, Point d, float u)
            {
                return Vector2.LerpUnclamped(c.transform.localPosition, d.transform.localPosition, (u - a) / (b - a));
            }

            Vector2 Remap(float a, float b, Vector2 c, Vector2 d, float u)
            {
                return Vector2.LerpUnclamped(c, d, (u - a) / (b - a));
            }

            Vector2 Remap(float a, float b, Vector3 c, Point d, float u)
            {
                return Vector2.LerpUnclamped(c, d.transform.localPosition, (u - a) / (b - a));
            }

            Vector2 Remap(float a, float b, Point c, Vector3 d, float u)
            {
                return Vector2.LerpUnclamped(c.transform.localPosition, d, (u - a) / (b - a));
            }

            float GetKnotInterval(Vector3 a, Point b)
            {
                return Mathf.Pow(Vector2.SqrMagnitude(a - b.transform.localPosition), 0.5f * alpha);
            }

            float GetKnotInterval(Point a, Vector3 b)
            {
                return Mathf.Pow(Vector2.SqrMagnitude(a.transform.localPosition - b), 0.5f * alpha);
            }

            float GetKnotInterval(Point a, Point b)
            {
                return Mathf.Pow(Vector2.SqrMagnitude(a.transform.localPosition - b.transform.localPosition), 0.5f * alpha);
            }

            float GetKnotInterval(Vector3 a, Vector3 b)
            {
                return Mathf.Pow(Vector2.SqrMagnitude(a - b), 0.5f * alpha);
            }
        }

        public enum DrawMode
        {
            Line,
            CatmullRomSpline,
            Auto
        }
    }
}

