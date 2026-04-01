using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TopNotificationUI : Singleton<TopNotificationUI>
{
    [SerializeField] private UIItemNotification uiItemNoti;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rect;

    [SerializeField] private float displayTime = 2f;
    [SerializeField] private float moveDistance = 100f;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private float maxCount = 3;
    [SerializeField] private float currentCount = 3;
    private Coroutine currentRoutine;
    private string currentMessage = "";

    protected override void Awake()
    {
        canvasGroup.alpha = 0;
        currentCount = 0;
    }

    public void Show(string message)
    {
        // Gộp message
        if (currentCount >= maxCount)
        {
            currentMessage = "";
            currentCount = 0;
        }
        if (!string.IsNullOrEmpty(currentMessage))
            currentMessage += "\n" + message;
        else
            currentMessage = message;

        text.text = currentMessage;
        currentCount++;
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine());
    }
    private IEnumerator ShowRoutine()
    {
        canvasGroup.alpha = 1;
        rect.anchoredPosition = Vector2.zero;

        float timer = 0f;

        // ⏱️ Đếm thời gian nhưng pause khi hold
        while (timer < displayTime)
        {
            if (!uiItemNoti.IsHolding())
            {
                timer += Time.deltaTime;
            }

            yield return null;
        }

        // 🚀 bắt đầu fade + move
        float time = 0;
        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * moveDistance;

        while (time < 1f)
        {
            // ⏸️ nếu giữ → pause animation luôn
            if (uiItemNoti.IsHolding())
            {
                yield return null;
                continue;
            }

            time += Time.deltaTime * fadeSpeed;

            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, time);
            canvasGroup.alpha = 1 - time;

            yield return null;
        }

        canvasGroup.alpha = 0;
        currentMessage = "";
        currentCount = 0;
    }
}