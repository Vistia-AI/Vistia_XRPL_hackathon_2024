using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountUI : UIBase
{
    [SerializeField] private TMP_Text userNameTxt;
    [SerializeField] private TMP_Text mailTxt;
    [SerializeField] private Image avatarImg;
    [SerializeField] private Button profileButton;
    [SerializeField] private Button changePassButton;
    [SerializeField] private Button aiChatBotBtn;
    [SerializeField] private Button communityBtn;

    [SerializeField] private Button FAQButton;
    [SerializeField] private Button helpCenterButton;
    [SerializeField] private Button LogoutButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        profileButton.onClick.AddListener(ProfileOpen);
        changePassButton.onClick.AddListener(ChangePass);
        FAQButton.onClick.AddListener(FAQOpen);
        helpCenterButton.onClick.AddListener(HelpCenterOpen);
        LogoutButton.onClick.AddListener(LogOut);
        aiChatBotBtn.onClick.AddListener(AIChatBotOpen);
        communityBtn.onClick.AddListener(CommunityOpen);

    }

    private void CommunityOpen()
    {
        Application.OpenURL("https://t.me/+iVHND1-90TAwNzA1");
    }

    private async void AIChatBotOpen()
    {
        await UIManager.Instance.ChangePageAsync(UIType.ChatBot);

    }

    public void InitProfile(string userName, string mail, Sprite avatar)
    {
        this.userNameTxt.text = "Hi, " + userName;
        this.mailTxt.text = mail;
        this.avatarImg.sprite = avatar;
    }
    private void LogOut()
    {
        throw new NotImplementedException();
    }

    private void HelpCenterOpen()
    {
        Application.OpenURL("https://t.me/+iVHND1-90TAwNzA1");
    }

    private void FAQOpen()
    {
        Application.OpenURL("https://t.me/+iVHND1-90TAwNzA1");
    }

    private void ChangePass()
    {
        throw new NotImplementedException();
    }

    private void ProfileOpen()
    {
        throw new NotImplementedException();
    }
}
