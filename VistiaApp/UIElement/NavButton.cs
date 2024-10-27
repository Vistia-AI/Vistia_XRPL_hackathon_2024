using UnityEngine;
using UnityEngine.UI;

public class NavButton : MonoBehaviour
{
    [SerializeField] private UIType uiType;
    [SerializeField] private Button button;

    private void Start()
    {
        button.onClick.AddListener(OnClick);
    }

    private async void OnClick()
    {
        await UIManager.Instance.ChangePageAsync(uiType);
    }
}
