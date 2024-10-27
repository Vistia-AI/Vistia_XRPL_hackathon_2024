using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ChatBox : MonoBehaviour, IPoolable<ChatBox>
{
    [SerializeField] private TMPro.TMP_Text m_contentText;
    [SerializeField] private TMPro.TMP_Text m_timeText;

    private Action<ChatBox> m_returnAction;
    private Coroutine m_waitingCo;

    private void Start()
    {
        this.transform.localScale = Vector3.one;

    }
    public void Initialize(Action<ChatBox> returnAction)
    {

        m_returnAction = returnAction;
    }

    public void ReturnToPool()
    {
        m_returnAction?.Invoke(this);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SetContent(string content, string time)
    {
        if (m_waitingCo != null)
        {
            StopCoroutine(m_waitingCo);
            m_waitingCo = null;
        }
        this.m_contentText.text = ApplyBoldFormatting(content);
        this.m_timeText.text = time;
    }

    public void SetWaitingStatus(ScrollRect scrollRect)
    {
        m_contentText.text = "Generating";
        m_waitingCo = StartCoroutine(WaitingCo());
        scrollRect.verticalNormalizedPosition = 0;
    }

    private IEnumerator WaitingCo()
    {
        while (true)
        {
            m_contentText.text = "Generating";
            yield return new WaitForSeconds(0.1f);
            m_contentText.text = "Generating.";
            yield return new WaitForSeconds(0.1f);
            m_contentText.text = "Generating..";
            yield return new WaitForSeconds(0.1f);
            m_contentText.text = "Generating...";
            yield return new WaitForSeconds(0.1f);
        }
    }

    string ApplyBoldFormatting(string content)
    {
        StringBuilder formattedContent = new StringBuilder();
        bool isBold = false;

        int length = content.Length;
        for (int i = 0; i < length; i++)
        {
            // Check for the bold symbols (**)
            if (i < length - 1 && content[i] == '*' && content[i + 1] == '*')
            {
                isBold = !isBold; // Toggle bold mode
                i++; // Skip the next '*'
                if (isBold)
                {
                    formattedContent.Append("<b>"); // Start bold
                }
                else
                {
                    formattedContent.Append("</b>"); // End bold
                }
                continue;
            }

            // Append the character as it is
            formattedContent.Append(content[i]);
        }

        return formattedContent.ToString();
    }

    public void SetContentWithAnimation(string content, string time, ScrollRect scrollRect, Action onComplete, int typingSpeed = 200)
    {
        if (m_waitingCo != null)
        {
            StopCoroutine(m_waitingCo);
            m_waitingCo = null;
        }

        m_timeText.text = time;
        TypeText(content, typingSpeed, scrollRect, onComplete).Forget();
    }

    private async UniTask TypeText(string data, int charPerSecond, ScrollRect scrollRect, Action onComplete)
    {
        StringBuilder stringBuilder = new StringBuilder();
        int length = data.Length;
        bool isBold = false; // Track whether we're in bold mode

        // Calculate delay between each character based on charPerSecond
        float delayBetweenChars = 1f / charPerSecond;

        for (int i = 0; i < length; i++)
        {
            // Check for bold symbols (**)
            if (i < length - 1 && data[i] == '*' && data[i + 1] == '*')
            {
                isBold = !isBold; // Toggle bold mode
                i++; // Skip the next '*'
                if (isBold)
                {
                    stringBuilder.Append("<b>"); // Start bold
                }
                else
                {
                    stringBuilder.Append("</b>"); // End bold
                }
                continue;
            }

            stringBuilder.Append(data[i]); // Append one character at a time
            m_contentText.text = stringBuilder.ToString(); // Update the UI with the current string

            scrollRect.verticalNormalizedPosition = 0; // Scroll to the bottom

            // Wait before processing the next character
            await UniTask.Delay((int)(delayBetweenChars * 1000)); // Convert delay to milliseconds
        }

        onComplete?.Invoke();
    }
}
