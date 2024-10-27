using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NavButtonEffect : MonoBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite unfocused;
    [SerializeField] private Sprite focused;
    [SerializeField] private TMP_Text text;
    [SerializeField] private UIType uiType;
    private Tween _tween;

    public Action<NavButtonEffect> callback;

    public UIType UiType { get => uiType; set => uiType = value; }

    private void Start()
    {
        btn.onClick.AddListener(Select);
    }

    public void Select()
    {
        btn.interactable = false;
        ResetTween();
        callback?.Invoke(this);
        _tween = icon.transform.DOLocalMoveY(25, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            icon.sprite = focused;
            text.DOFade(1, 0.2f);
        });
    }

    public void DeSelect()
    {
        btn.interactable = true;
        ResetTween();
        _tween = text.DOFade(0, 0.2f).OnComplete(() =>
        {
            icon.transform.DOLocalMoveY(0, 0.2f).SetEase(Ease.OutBack);
            icon.sprite = unfocused;
        });
    }

    private void ResetTween() => _tween?.Kill();
}
