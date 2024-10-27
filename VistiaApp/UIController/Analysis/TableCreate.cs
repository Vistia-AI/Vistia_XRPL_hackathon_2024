using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Analysis
{
    public class TableCreate : MonoBehaviour
    {
        [SerializeField] private TableLineContent linePrefab;
        [SerializeField] private Transform container;
        [SerializeField] private RectTransform contentRT;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private BannerController bannerController;
        [SerializeField] private GameObject noData;
        private List<TableLineContent> activeList = new List<TableLineContent>();
        private ObjectPool<TableLineContent> pool;
        private void Start()
        {
            pool = new(linePrefab, 2);
        }
        public void CreateTable(List<StockData> listData, Signal signal)
        {

            int totalLine = listData.Count;
            if (totalLine == 0) noData.SetActive(true);
            else
            {
                string temp = listData[0].Symbol;
                bannerController.SetBanner(temp);
                noData.SetActive(false);
            }
            if (activeList.Count < totalLine)
            {
                int count = totalLine - activeList.Count;
                for (int i = 0; i < count; i++)
                {
                    TableLineContent line = pool.Pull(Vector3.zero, container);
                    activeList.Add(line);
                }
            }
            else if (activeList.Count > totalLine)
            {
                int count = activeList.Count - totalLine;
                for (int i = activeList.Count - 1, j = count; i >= totalLine && count > 0; i--, j--)
                {
                    activeList[i].ReturnToPool();
                    activeList.RemoveAt(i);
                }
            }
            for (int i = 0; i < listData.Count; i++)
            {
                activeList[i].SetContent(i + 1, listData[i], signal);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentRT);
            // scrollRect.DOVerticalNormalizedPos(0, 0.5f);
        }
    }
}
