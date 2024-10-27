using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Predict
{
    public class IntervalButton : MonoBehaviour
    {
        [SerializeField] private Interval interval = Interval.One_Day;
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text text;
        [SerializeField] private bool selectOnStart = false;
        static Color32 selectedColor = new Color32(26, 100, 240, 255);
        static Color32 unselectedColor = new Color32(255, 255, 255, 0);

        /*private void Start()
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
                PredictChartController.Instance.CurrentSelectBtn = this;
            }
        }*/

        /*private void OnClick()
        {
            if (!PredictChartController.Instance.Changable) return;
            //GraphController.Instance.ChangeSetting(interval, openTime, this);
            PredictChartController.Instance.ChangeSetting(interval, this);
        }*/

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
