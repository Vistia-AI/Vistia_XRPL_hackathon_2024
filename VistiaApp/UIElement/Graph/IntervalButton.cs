using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Graph
{
    public class IntervalButton : MonoBehaviour
    {
        [SerializeField] private Interval interval = Interval.One_Day;
        [SerializeField] private OpenTime openTime = OpenTime.One_Year_Ago;
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text text;
        [SerializeField] private bool selectOnStart = false;
        static Color32 selectedColor = new Color32(26, 100, 240, 255);
        static Color32 unselectedColor = new Color32(255, 255, 255, 0);

        private void Start()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }
            button?.onClick.AddListener(OnClick);
            if (selectOnStart)
            {
                OnSelect();
                //GraphController.Instance.CurrentSelectBtn = this;
                ChartController.Instance.CurrentSelectBtn = this;
            }
        }

        private void OnClick()
        {
            if (!ChartController.Instance.Changable) return;
            //GraphController.Instance.ChangeSetting(interval, openTime, this);
            ChartController.Instance.ChangeSetting(interval, openTime, this);
        }

        public void OnSelect()
        {
            button.targetGraphic.DOColor(selectedColor, 0.5f);
            text.DOColor(Color.white, 0.5f);
        }

        public void OnDeselect()
        {
            button.targetGraphic.DOColor(unselectedColor, 0.5f);
            text.DOColor(Color.black, 0.5f);
        }
    }
}
