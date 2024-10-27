using UnityEngine;
using UnityEngine.UI;

public class HelloUI : PopupBase
{
    [SerializeField] private Button nextBtn;
    private void Awake()
    {
        nextBtn.onClick.AddListener(HomeOpen);
    }

    private void HomeOpen()
    {
        this.gameObject.SetActive(false);
    }
}
