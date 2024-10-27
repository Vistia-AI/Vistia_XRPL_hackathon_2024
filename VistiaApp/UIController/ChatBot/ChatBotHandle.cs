using Cysharp.Threading.Tasks;
using LitMotion;
using System;
using System.Collections.Generic;
using TMPro;
using UMI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatBotHandle : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private ChatBox m_chatBoxPrefab;
    [SerializeField] private ChatBox m_bigChatBoxPrefab;
    [SerializeField] private ChatBox m_userChatBoxPrefab;
    [SerializeField] private TimeIndicator m_timeIndicatorPrefab;

    [SerializeField] private int m_bigDataThreshold = 500;

    [Header("Parent")]
    [SerializeField] private Transform m_chatBoxParent;

    [Header("Inputfield")]
    [SerializeField] private TMP_InputField m_chatInputField;
    [SerializeField] private RectTransform m_chatInputFieldRect;
    [SerializeField] private MobileInputField m_mobileInputField;

    [Header("ScrollRect")]
    [SerializeField] private ScrollRect m_chatScrollRect;
    [SerializeField] private RectTransform m_scrollRectRT;
    [SerializeField] private RectTransform m_contentRect;

    [Header("Duration")]
    [SerializeField] float duration = .5f;

    [Header("Canvas")]
    [SerializeField] private Canvas m_canvas;

    [Header("Images")]
    [SerializeField] private Image m_chatLoadingImage;

    public TMP_Text m_logText;

    private ObjectPool<ChatBox> m_chatBoxPool;
    private ObjectPool<ChatBox> m_bigChatBoxPool;
    private ObjectPool<ChatBox> m_userChatBoxPool;
    private ObjectPool<TimeIndicator> m_timeIndicatorPool;
    private ChatBotHistory m_chatBotHistory;

    private VistiaFile m_vistiaFile;

    private int m_currentHistoryIndex;

    private DateTime m_lastChatTime;

    private bool m_isReloadAvailable = false;

    private int m_startIndex = -1;
    private float keyBoardAnchor;

    private void OnEnable()
    {
        MobileInput.OnKeyboardAction += OnKeyboardAction;
    }

    private void OnDisable()
    {
        MobileInput.OnKeyboardAction -= OnKeyboardAction;
    }

    private void OnKeyboardAction(bool isShow, int height)
    {
        // raise when the keyboard is displayed or hidden, and when the keyboard height is changed
        m_logText.text = $"Keyboard is {(isShow ? "shown" : "hidden")}, height: {height}, Screen Height: {Screen.height}";
        float keyBoardHeight = height / m_canvas.transform.localScale.y;

        MoveInputField(keyBoardHeight);
    }

    private void MoveInputField(float height)
    {
        if (height == 0)
        {
            m_chatInputFieldRect.offsetMax = new Vector2(-40f, 30 + m_chatInputFieldRect.sizeDelta.y);
            m_chatInputFieldRect.offsetMin = new Vector2(40f, 30);
            return;
        }
        float padding = Screen.height / m_canvas.transform.localScale.x * 0.1f;
        height -= padding;
        m_chatInputFieldRect.offsetMax = new Vector2(-40f, 30 + height + m_chatInputFieldRect.sizeDelta.y);
        m_chatInputFieldRect.offsetMin = new Vector2(40f, 30 + height);
    }

    private void Awake()
    {
        m_chatBoxPool = new ObjectPool<ChatBox>(m_chatBoxPrefab, 0);
        m_bigChatBoxPool = new ObjectPool<ChatBox>(m_bigChatBoxPrefab, 0);
        m_userChatBoxPool = new ObjectPool<ChatBox>(m_userChatBoxPrefab, 0);
        m_timeIndicatorPool = new ObjectPool<TimeIndicator>(m_timeIndicatorPrefab, 0);
    }

    private void Start()
    {
        m_chatInputField.onSubmit.AddListener(OnChatInputFieldSubmit);
        m_chatInputField.interactable = false;

        m_chatScrollRect.onValueChanged.AddListener(OnScrollRectValueChange);

        m_anchorMaxMotion = LMotion.Create(0, 0, 0).RunWithoutBinding();
        m_anchorMinMotion = LMotion.Create(0, 0, 0).RunWithoutBinding();

        LoadOrCreateFile();
    }

    private void LoadOrCreateFile()
    {
        m_vistiaFile = new VistiaFile();

        if (m_vistiaFile.IsFileExists("Data", "ChatHistory"))
        {
            m_chatBotHistory = m_vistiaFile.Load<ChatBotHistory>("Data", "ChatHistory", "defaultPassword").Item1;
        }
        else
        {
            m_chatBotHistory = new ChatBotHistory();
            m_vistiaFile.Save("Data", "ChatHistory", m_chatBotHistory, null);
        }

        m_isReloadAvailable = true;
        m_currentHistoryIndex = m_chatBotHistory.Count();
        m_chatInputField.interactable = true;

        Debug.Log(m_chatBotHistory.ToString());

        LoadHistoryChat(--m_currentHistoryIndex).Forget();
    }

    private async UniTask LoadHistoryChat(int index)
    {
        m_isReloadAvailable = false;
        if (index < 0 || index >= m_chatBotHistory.Count())
        {
            m_currentHistoryIndex = 0;
            m_isReloadAvailable = true;
            Debug.LogWarning("Index out of range!");
            return;
        }

        m_chatInputField.interactable = false;
        List<Tuple<string, string, bool>> data = m_chatBotHistory.GetDataByIndex(index);

        string time = m_chatBotHistory.GetDateTimeByIndex(index);
        DateTime temp = DateTime.Parse(time);
        bool isSameDay = false;

        if (m_lastChatTime == DateTime.MinValue)
            m_lastChatTime = temp;

        if (temp.Day == m_lastChatTime.Day)
        {
            TimeIndicator tIndicator = m_chatBoxParent.GetComponentInChildren<TimeIndicator>(false);
            if (tIndicator != null)
            {
                tIndicator.gameObject.SetActive(false);
                tIndicator.ReturnToPool();
            }
            isSameDay = true;
        }

        Debug.Log("Temp: " + temp.ToString("MMM dd, yyyy") + " Last: " + m_lastChatTime.ToString("MMM dd, yyyy") + " Same: " + isSameDay);

        float delayTime = duration / data.Count;
        // Get from back to front
        for (int i = data.Count - 1; i >= 0; i--)
        {
            if (m_startIndex > 0 && isSameDay && i >= m_startIndex) continue;
            Debug.Log("Index: " + i + " Start: " + m_startIndex);
            Tuple<string, string, bool> message = data[i];
            ChatBox chatBox;

            if (message.Item3)
            {
                chatBox = m_userChatBoxPool.Pull();
                chatBox.transform.SetParent(m_chatBoxParent);
                chatBox.SetContent(message.Item1, message.Item2);
            }
            else
            {
                string s = message.Item1;
                if (s.Length > m_bigDataThreshold)
                {
                    chatBox = m_bigChatBoxPool.Pull();
                    chatBox.transform.SetParent(m_chatBoxParent);
                    chatBox.SetContent(s, message.Item2);
                }
                else
                {
                    //chatBox = m_chatBoxPool.Pull();
                    chatBox = m_chatBoxPool.Pull();
                    chatBox.transform.SetParent(m_chatBoxParent);
                    chatBox.SetContent(s, message.Item2);
                }
            }
            if (isSameDay) chatBox.transform.SetSiblingIndex(2);
            chatBox.transform.SetAsFirstSibling();

            await UniTask.Delay((int)(delayTime * 1000));
        }


        TimeIndicator timeIndicator = m_timeIndicatorPool.Pull();
        timeIndicator.transform.SetParent(m_chatBoxParent);
        timeIndicator.SetTime(temp);

        timeIndicator.transform.SetAsFirstSibling();

        await LMotion.Create(m_chatScrollRect.verticalNormalizedPosition, 1, duration)
            .WithEase(Ease.InOutCubic)
            .WithOnComplete(() =>
            {
                m_chatInputField.interactable = true;
                m_isReloadAvailable = true;
            })
            .BindWithState(m_chatScrollRect, (value, target) => target.verticalNormalizedPosition = value)
            .ToUniTask();
    }

    private void SaveChatContent(string message, string time, bool isUser)
    {
        int appendIndex = m_chatBotHistory.Add(m_lastChatTime.ToString("MMM dd, yyyy"), message, time, isUser);

        if (m_startIndex < 0)
        {
            m_startIndex = appendIndex;
        }

        m_vistiaFile.Save("Data", "ChatHistory", m_chatBotHistory, null);
    }

    private void OnScrollRectValueChange(Vector2 value)
    {
        float threshold = Screen.height / m_canvas.transform.localScale.x * 0.05f;

        float completePercentage = m_contentRect.localPosition.y / threshold;
        if (!m_chatInputField.interactable) completePercentage = 0;

        m_chatLoadingImage.transform.localEulerAngles = new Vector3(0, 0, completePercentage * 360);

        m_chatLoadingImage.ChangeAlpha(Mathf.Max(0, -completePercentage), 0);

        if (!m_isReloadAvailable) return;

        if (m_contentRect.localPosition.y <= -threshold) // Top
        {
            m_chatScrollRect.OnEndDrag(new PointerEventData(EventSystem.current));
            LoadHistoryChat(--m_currentHistoryIndex).Forget();
        }
    }

    private ChatBox m_tempChatBox;

    private void OnChatInputFieldSubmit(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        OnDeselect();

        m_chatInputField.text = string.Empty;

        DateTime currentTime = System.DateTime.Now;

        if (m_lastChatTime == null || currentTime.Day != m_lastChatTime.Day)
        {
            TimeIndicator timeIndicator = m_timeIndicatorPool.Pull();
            timeIndicator.transform.SetParent(m_chatBoxParent);
            timeIndicator.SetTime(currentTime);

            m_lastChatTime = currentTime;
        }

        string currentTimeText = currentTime.ToString("HH:mm tt");

        ChatBox chatBox = m_userChatBoxPool.Pull();
        chatBox.transform.SetParent(m_chatBoxParent);
        chatBox.SetContent(text, currentTimeText);

        m_tempChatBox = m_chatBoxPool.Pull();
        m_tempChatBox.transform.SetParent(m_chatBoxParent);
        m_tempChatBox.SetWaitingStatus(m_chatScrollRect);

        SaveChatContent(text, currentTimeText, true);

        LMotion.Create(m_chatScrollRect.verticalNormalizedPosition, 0, duration)
            .WithEase(Ease.InOutCubic)
            .BindWithState(m_chatScrollRect, (value, target) => target.verticalNormalizedPosition = value);

        SentChat(text);
    }

    private void SentChat(string data)
    {
        m_chatInputField.interactable = false;
        AlchemistAPIHandle.Instance.ChatBot(data, OnChatReceived, OnChatErrorReceived);
    }

    private void OnChatReceived(string data)
    {
        int length = data.Length;
        bool isBigData = length > m_bigDataThreshold;
        int charPerSec = Mathf.Max(Mathf.CeilToInt(length / 5f), 100);
        string currentTimeText = System.DateTime.Now.ToString("HH:mm tt");

        if (isBigData)
        {
            m_tempChatBox?.ReturnToPool();
            m_tempChatBox = null;
            ChatBox chatBox = m_bigChatBoxPool.Pull();
            chatBox.transform.SetParent(m_chatBoxParent);
            //chatBox.SetContent(data, currentTimeText);
            chatBox.SetContentWithAnimation(data, currentTimeText, m_chatScrollRect, () => m_chatInputField.interactable = true, charPerSec);
        }
        else
        {
            //m_tempChatBox.SetContent(data, currentTimeText);
            m_tempChatBox.SetContentWithAnimation(data, currentTimeText, m_chatScrollRect, () => m_chatInputField.interactable = true, charPerSec);
            m_tempChatBox = null;
        }

        SaveChatContent(data, currentTimeText, false);
    }

    private void OnChatErrorReceived(string error)
    {
        string currentTimeText = System.DateTime.Now.ToString("HH:mm tt");

        if (m_tempChatBox != null)
        {
            m_tempChatBox.SetContent(error, currentTimeText);
            m_tempChatBox.SetContentWithAnimation(error, currentTimeText, m_chatScrollRect, () => m_chatInputField.interactable = true);
            m_tempChatBox = null;
        }
        else
        {
            ChatBox chatBox = m_chatBoxPool.Pull();
            chatBox.transform.SetParent(m_chatBoxParent);
            chatBox.SetContent($"<color = red>{error}</color>", currentTimeText);
            //chatBox.SetContentWithAnimation(error, currentTimeText, m_chatScrollRect, () => m_chatInputField.interactable = true);
            m_chatInputField.interactable = true;
        }
    }

    private void ClearChat()
    {

    }

    MotionHandle m_anchorMaxMotion;
    MotionHandle m_anchorMinMotion;

    public void OnSelect()
    {
        m_isReloadAvailable = false;
        /*if (m_anchorMaxMotion.IsActive()) m_anchorMaxMotion.Cancel();
        if (m_anchorMinMotion.IsActive()) m_anchorMinMotion.Cancel();

        m_anchorMaxMotion = m_chatInputFieldRect.ChangeAnchorMax(new Vector2(1, 1 - 0.013f), duration, null, Ease.InOutCubic);
        m_anchorMinMotion = m_chatInputFieldRect.ChangeAnchorMin(new Vector2(0, 1 - 0.074f), duration, null, Ease.InOutCubic);*/
    }

    public void OnDeselect()
    {
        OnKeyboardAction(false, 0);
        //m_mobileInputField.SetFocus(false);
        m_isReloadAvailable = true;
        /*if (m_anchorMaxMotion.IsActive()) m_anchorMaxMotion.Cancel();
        if (m_anchorMinMotion.IsActive()) m_anchorMinMotion.Cancel();

        m_anchorMaxMotion = m_chatInputFieldRect.ChangeAnchorMax(new Vector2(1, 0.074f), duration, null, Ease.InOutCubic);
        m_anchorMinMotion = m_chatInputFieldRect.ChangeAnchorMin(new Vector2(0, 0.013f), duration, () => m_isReloadAvailable = true, Ease.InOutCubic);*/
    }
}
