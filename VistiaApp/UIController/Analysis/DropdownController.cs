using System.Collections;
using UnityEngine;

namespace Analysis
{
    public class DropdownController : MonoBehaviour
    {
        [SerializeField] private AdvancedDropdown heatMap;
        [SerializeField] private AdvancedDropdown timeType;

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();

            heatMap.SelectOption((int)AlchemistAPIHandle.Instance.HeatMapType);
            timeType.SelectOption((int)AlchemistAPIHandle.Instance.TimeType);

            heatMap.onChangedValue += OnHeatMapSelected;
            timeType.onChangedValue += OnTimeTypeSelected;
        }

        private void OnHeatMapSelected(int index)
        {
            AlchemistAPIHandle.Instance.HeatMapType = (HeatMapType)index;
            AlchemistAPIHandle.Instance.Call_Al_API();
        }

        private void OnTimeTypeSelected(int index)
        {
            AlchemistAPIHandle.Instance.TimeType = (TimeType)index;
            AlchemistAPIHandle.Instance.Call_Al_API();
        }

        private void OnDestroy()
        {
            heatMap.onChangedValue -= OnHeatMapSelected;
            timeType.onChangedValue -= OnTimeTypeSelected;
        }

        /*private void OnConnectionStatusChanged(bool isConnected)
        {
            if (isConnected)
            {
                AlchemistAPIHandle.Instance.Call_Al_API();
            }
        }*/
    }
}
