using LitMotion;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Graph
{
    public class Point : MonoBehaviour, IPoolable<Point>
    {
        [SerializeField] private Image border;
        [SerializeField] private Image center;
        private Line previousLine;
        private Line nextLine;
        private Action<Point> returnAction;

        public Line PreviousLine { get => previousLine; set => previousLine = value; }
        public Line NextLine { get => nextLine; set => nextLine = value; }

        private void Start()
        {
            transform.localScale = Vector3.one;
        }

        public void SetThickness(float value)
        {
            //border.rectTransform.sizeDelta = new Vector2(value, value);
            if (m_size.IsActive()) m_size.Complete();
            m_size = LMotion.Create(border.rectTransform.sizeDelta, new Vector2(value, value), .5f)
                .BindWithState(border.rectTransform, (size, target) => border.rectTransform.sizeDelta = size);
        }

        public void SetColor(Color32 color, bool centerOnly = false)
        {
            if (centerOnly)
            {
                center.color = color;
                border.color = Color.black;
                return;
            }
            border.color = color;
            center.color = color;
        }

        private MotionHandle m_size;
        private MotionHandle m_color;
        private MotionHandle m_center_color;

        public void OnSelect(float size, Color32 color, Transform layer)
        {
            transform.SetParent(layer);
            if (m_size.IsActive()) m_size.Complete();
            if (m_color.IsActive()) m_color.Complete();
            if (m_center_color.IsActive()) m_center_color.Complete();
            //DOTween.To(() => image.rectTransform.sizeDelta, x => image.rectTransform.sizeDelta = x, new Vector2(size, size), 0.5f);
            //DOTween.To(() => image.color, x => image.color = x, color, 1f);
            m_size = LMotion.Create(border.rectTransform.sizeDelta, new Vector2(size, size), .5f)
                .BindWithState(border.rectTransform, (size, target) => border.rectTransform.sizeDelta = size);
            m_color = LMotion.Create(border.color, Color.black, 1f)
                .BindWithState(border, (c, target) => border.color = c);
            m_center_color = LMotion.Create(center.color, color, 1f)
                .BindWithState(center, (c, target) => center.color = c);
        }

        public void OnDeselect(float size, Color32 color, Transform layer)
        {
            transform.SetParent(layer);
            if (m_size.IsActive()) m_size.Complete();
            if (m_color.IsActive()) m_color.Complete();
            if (m_center_color.IsActive()) m_center_color.Complete();
            //DOTween.To(() => image.rectTransform.sizeDelta, x => image.rectTransform.sizeDelta = x, new Vector2(size, size), 0.5f);
            //DOTween.To(() => image.color, x => image.color = x, color, 1f);
            m_size = LMotion.Create(border.rectTransform.sizeDelta, new Vector2(size, size), .5f)
                .BindWithState(border.rectTransform, (size, target) => border.rectTransform.sizeDelta = size);
            m_color = LMotion.Create(border.color, color, 1f)
                .BindWithState(border, (c, target) => border.color = c);
            m_center_color = LMotion.Create(center.color, color, 1f)
                .BindWithState(center, (c, target) => center.color = c);
        }

        public void RecalculateScale(Vector3 scale)
        {
            transform.localScale = new Vector3(
                1f / scale.x,
                1f / scale.y,
                1f / scale.z
            );
        }


        public void Initialize(Action<Point> returnAction)
        {
            this.returnAction = returnAction;
        }

        public void ReturnToPool()
        {
            this.returnAction(this);
        }
    }
}
