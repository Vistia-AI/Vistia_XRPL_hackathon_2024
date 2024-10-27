using Cysharp.Threading.Tasks;
using UnityEngine;

public class HomeUI : UIBase
{
    [SerializeField] private SearchPopup searchController;
    private bool isSearchOpen;
    public override async UniTask OnShow(UIType previousUI, params object[] message)
    {
        await base.OnShow(previousUI, message);
        if (previousUI == UIType.Chart)
        {
            searchController.gameObject.SetActive(isSearchOpen);
        }
        isSearchOpen = false;
    }

    public override async UniTask OnHide(UIType nextUI)
    {
        isSearchOpen = searchController.gameObject.activeSelf;
        if (isSearchOpen)
        {
            await searchController.OnCloseAsync();
            await UniTask.Delay(500);
        }
    }
}