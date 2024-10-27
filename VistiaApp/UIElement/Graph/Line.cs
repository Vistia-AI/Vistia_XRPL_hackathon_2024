using System;
using UnityEngine;
using UnityEngine.UI;

namespace Graph
{
    public class Line : MonoBehaviour, IPoolable<Line>
    {
        [SerializeField] private Image image;

        private Action<Line> returnAction;
        private Point previousPoint;
        private Point nextPoint;

        public Point NextPoint { get => nextPoint; set => nextPoint = value; }
        public Point PreviousPoint { get => previousPoint; set => previousPoint = value; }

        private void Start()
        {
            transform.localScale = Vector3.one;
        }

        public void SetColor(Color32 color)
        {
            image.color = color;
        }

        public void SetPoints(Point point1, Point point2, float thickess)
        {
            previousPoint = point1;
            nextPoint = point2;

            Vector3 direction = point2.transform.localPosition - point1.transform.localPosition;
            float distance = direction.magnitude;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            image.rectTransform.sizeDelta = new Vector2(distance, thickess);
            image.rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
            image.rectTransform.localPosition = point1.transform.localPosition + direction / 2;
        }

        public void SetPoints(float thickess)
        {
            Vector3 direction = nextPoint.transform.localPosition - previousPoint.transform.localPosition;
            float distance = direction.magnitude;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            image.rectTransform.sizeDelta = new Vector2(distance, thickess);
            image.rectTransform.localRotation = Quaternion.Euler(0, 0, angle);
            image.rectTransform.localPosition = previousPoint.transform.localPosition + direction / 2;
        }

        public void Initialize(Action<Line> returnAction)
        {
            this.returnAction = returnAction;
        }

        public void ReturnToPool()
        {
            this.returnAction(this);
        }
    }
}
