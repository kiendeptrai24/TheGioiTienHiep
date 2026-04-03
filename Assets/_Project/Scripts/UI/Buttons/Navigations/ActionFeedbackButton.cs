
using UnityEngine;
using UnityEngine.UI;
using System;
using PlayFab.ClientModels;
using System.Collections.Generic;
using PlayFab;

[Serializable]
public class FeedbackPayload
{
    public string userId;
    public string title;
    public string message;
    public string gameVersion;
    public string platform;
}
public class ActionFeedbackButton : TGTHMonoBehaviour
{
    [SerializeField] private Button okeBtn;
    public float feedbackCooldown = 60f; // Cooldown time in seconds
    private float lastFeedbackTime = -Mathf.Infinity; // Time when the last feedback
    private bool isCooldownActive => Time.time - lastFeedbackTime < feedbackCooldown;
    private PlayfabDataManager playfabDataManager;
    protected override void Awake()
    {
        base.Awake();
        playfabDataManager = PlayfabDataManager.Instance;
        okeBtn = GetComponent<Button>();
        okeBtn.onClick.AddListener(OnClickBtn);
    }

    private void OnClickBtn()
    {
        var itemData = InventoryCenterManager.Instance.playerCham;
        var popup = PopupManager.Instance.GetPopup<FeedbackPopup>();
        var data = new BaseSetupData();
        popup.ShowPopup(data, result =>
        {
            if (result != null)
            {
                if (isCooldownActive)
                {
                    TopNotificationUI.Instance.ShowNotification($"Vui lòng đợi {feedbackCooldown - (Time.time - lastFeedbackTime):F1} giây nữa nếu muốn gửi phản hồi tiếp theo.");
                }
                else
                {

                    SendFeedback(result.userId, result.title, result.message);
                    lastFeedbackTime = Time.time;
                }
            }
        });
    }
    public void SendFeedback(string userId, string title, string message)
    {
        var request = new WriteClientPlayerEventRequest
        {
            EventName = "player_feedback",
            Body = new Dictionary<string, object>
            {
                { "userId", userId },
                { "title", title },
                { "message", message },
                { "gameVersion", Application.version },
                { "platform", Application.platform.ToString() }
            }
        };

        playfabDataManager.GetClientAPI().WritePlayerEvent(
            request,
            result => Debug.Log("Đã gửi feedback lên PlayFab"),
            error => Debug.LogError("Gửi feedback lỗi: " + error.GenerateErrorReport())
        );
    }
}