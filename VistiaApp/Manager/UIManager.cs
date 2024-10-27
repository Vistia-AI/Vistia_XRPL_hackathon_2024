using UnityEngine;
using System.Collections.Generic;
using System;
using LitMotion;
using Cysharp.Threading.Tasks;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private List<NavButtonEffect> navButtons;
    [SerializeField] private List<UIBase> pages;
    [SerializeField] private List<PopupBase> popups;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform pagesContainer;

    private UIType _currentUIType = UIType.Home;

    private NavButtonEffect _currentNavButton;
    private UIBase _currentPage;
    private PopupBase _currentPopup;

    private void Start()
    {
        if (!PlayerPrefs.HasKey("isFirstTime"))
        {
            ShowPopup(PopupType.Hello, (popup) =>
            {
                PlayerPrefs.SetInt("isFirstTime", 1);
                PlayerPrefs.Save();
            });
        }
        Init();
    }

    public void Init()
    {
        foreach (var navButton in navButtons)
        {
            navButton.callback = OnNavButtonSelect;
        }
        navButtons[(int)_currentUIType].Select();
    }

    private async void OnNavButtonSelect(NavButtonEffect navButton)
    {
        await ChangePageAsync(navButton.UiType);
        _currentNavButton?.DeSelect();
        _currentNavButton = navButton;
    }

    public async UniTask ChangePageAsync(UIType uiType, params object[] message)
    {
        int index = (int)uiType;
        UIBase ui = pages[index];
        if (_currentPage != null && _currentPage != ui)
        {
            await _currentPage.OnHide(uiType);

            bool isLeft = (int)_currentUIType < index;

            GameObject container = new GameObject("Container");

            container.transform.SetParent(pagesContainer.transform);
            container.transform.localPosition = Vector3.zero;

            _currentPage.transform.SetParent(container.transform);

            ui.transform.SetParent(container.transform);
            ui.transform.localPosition = new Vector3(Screen.width * (isLeft ? 1 : -1), 0, 0);
            ui.gameObject.SetActive(true);

            Action onComplete = () =>
            {
                _currentPage.transform.SetParent(pagesContainer.transform);
                ui.transform.SetParent(pagesContainer.transform);
                Destroy(container);
                _currentPage.gameObject.SetActive(false);
            };

            await container.transform.ChangeLocalPositionXTo(Screen.width * (isLeft ? -1 : 1) / canvas.transform.localScale.x, .3f, onComplete, Ease.InOutCubic);

            await ui.OnShow(_currentUIType, message);
        }
        _currentPage = ui;
        _currentUIType = uiType;
        _currentPage.gameObject.SetActive(true);
    }

    public void ShowPopup(PopupType popupType, Action<PopupBase> action)
    {
        PopupBase popup = popups[(int)popupType];
        _currentPopup = popup;
        popup.gameObject.SetActive(true);
        action?.Invoke(popup);
    }
}
