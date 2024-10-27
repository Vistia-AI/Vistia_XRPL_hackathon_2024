using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PredictionController : Singleton<PredictionController>
{
    [SerializeField] private RectTransform parentScrollRect;
    [SerializeField] private AITab prefabBtn;
    private List<AITab> PredictionList = new List<AITab>();
    private ObjectPool<AITab> tabPool;
    private PredictionData firstElement;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AlchemistAPIHandle.Instance.AIPredictionUpdatedStream += AIPredictionUpdated;
        tabPool = new ObjectPool<AITab>(prefabBtn, 0);
    }

    private void AIPredictionUpdated(List<PredictionData> list)
    {
        this.firstElement = list[0];
        int totalCoin = list.Count;
        if (PredictionList.Count < totalCoin)
        {
            int count = totalCoin - PredictionList.Count;
            for (int i = 0; i < count; i++)
            {
                AITab tab = tabPool.Pull(Vector3.zero, transform);
                tab.transform.localScale = Vector3.one;
                PredictionList.Add(tab);
            }
        }
        else if (PredictionList.Count > totalCoin)
        {
            int count = PredictionList.Count - totalCoin;
            for (int i = PredictionList.Count - 1, j = count; i >= totalCoin && count > 0; i--, j--)
            {
                PredictionList[i].ReturnToPool();
                PredictionList.RemoveAt(i);
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            PredictionList[i].SetContent(list[i]);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(parentScrollRect);
    }

    private void OnDestroy()
    {
        AlchemistAPIHandle.Instance.AIPredictionUpdatedStream -= AIPredictionUpdated;
    }
}
