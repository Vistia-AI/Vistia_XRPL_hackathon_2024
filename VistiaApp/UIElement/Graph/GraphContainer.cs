using System;
using UnityEngine;

namespace Graph
{
    public class GraphContainer : MonoBehaviour, IPoolable<GraphContainer>
    {
        [SerializeField] private Transform pointContainer;
        [SerializeField] private Transform lineContainer;
        [SerializeField] private Transform topLayer;
        [SerializeField] private RectTransform rectTransform;

        private Action<GraphContainer> returnAction;

        public Transform PointContainer => pointContainer;
        public Transform LineContainer => lineContainer;

        public Transform TopLayer { get => topLayer; set => topLayer = value; }

        public void Initialize(Action<GraphContainer> returnAction)
        {
            this.returnAction = returnAction;
        }

        public void ReturnToPool()
        {
            this.returnAction(this);
        }

        private void Start()
        {
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.anchorMin = new Vector2(0, 0);
            //rectTransform.pivot = new Vector2(0, 0);
            transform.localScale = Vector3.one;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
        }
    }
}
