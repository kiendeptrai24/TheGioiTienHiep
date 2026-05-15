using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Chat.TGTHChat;
using TGTH.Mobile;
using UnityEngine;
using UnityEngine.Playables;

public class ChatPagePresenter : TGTHMonoBehaviour
{
    public class ChatMessage
    {
        public string DisplayName;
        public string Text;
        public ChatMessage()
        {
        }
        public ChatMessage(string displayName, string text)
        {
            DisplayName = displayName;
            Text = text;
        }
    }
    [SerializeField] private ChatPageView view;
    [SerializeField] private ChatManager chatManager;
    [SerializeField] private ProfileManager profileManager;
    [SerializeField] private FriendPagePresenter friendPagePresenter;
    private ChatClient chatClient;
    private string nameFriend;
    private int curChatCount = 0;
    private int maxChatCount = 2;
    private int curPrivateChatCount = 0;
    private int maxPrivateChatCount = 10;
    private int timeToResetChat = 30;
    private float timer = 0;

    protected override void Awake()
    {
        base.Awake();
        view.OnSubmitChat += (text) =>
        {
            if (view.chatPrivate)
            {
                if (curPrivateChatCount >= maxPrivateChatCount)
                {
                    TopNotificationUI.Instance.ShowNotification($"Vui lòng đợi {TextColorUtil.Color("30s", Color.yellow)} sau để chat lại");
                    return;
                }
                curPrivateChatCount++;

                OnSendPrivateChat(text);
            }
            else
            {
                if (curChatCount >= maxChatCount)
                {
                    TopNotificationUI.Instance.ShowNotification($"Vui lòng đợi {TextColorUtil.Color("30s", Color.yellow)} sau để chat lại");
                    return;
                }
                curChatCount++;
                OnSendGlobalChat(text);
            }
        };
        friendPagePresenter.OnSwitchFriend += (name) =>
        {
            if (nameFriend == name) return;
            nameFriend = name;
            view.ShowNameFriend(nameFriend);
            // reset messege when you switch to new friend
            // dont save messege history
            view.ResetChatWithFriend();

        };
        chatManager.OnGetMessages += OnGetMessages;
        chatManager.OnPrivateMessage += OnPrivateMessage;

    }

    protected override void Start()
    {
        base.Start();
        chatClient = chatManager.chatClient;
    }
    private void Update()
    {
        if (Time.time > timer + timeToResetChat)
        {
            timer = Time.time;
            curChatCount = 0;
            curPrivateChatCount = 0;
        }
    }
    private void OnPrivateMessage(string senders, object messages, string channelName)
    {
        Debug.Log("OnPrivateMessage: " + senders);
        if (messages is not Hashtable data) return;

        string displayName = data["name"] as string;
        string text = data["text"] as string;

        bool isMine = senders == chatClient.UserId;
        string nameUser = isMine ? "You" : displayName;
        // text get friend's name
        if (isMine == false)
        {
            view.ShowNameFriend(displayName);
        }
        //
        string messageString =
            $"{TextColorUtil.Color(nameUser + ":", Color.chartreuse)} {TextColorUtil.Color(text, Color.white)}";

        view.ShowTextWithFriend(messageString);
    }

    private void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            if (messages[i] is not Hashtable data)
                continue;

            string displayName = data["name"] as string;
            string text = data["text"] as string;

            bool isMine = senders[i] == chatClient.UserId;
            string nameUser = isMine ? "You" : displayName;

            string messageString =
                $"<b><color=#ce4627ff>{nameUser}: </color></b>{text}";

            view.ShowText(messageString);
        }
    }

    private void OnSendGlobalChat(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        var msg = new Hashtable
        {
            { "name", profileManager.GetProfile().userName },
            { "text", text }
        };

        chatClient.PublishMessage("Global", msg);
    }
    private void OnSendPrivateChat(string text)
    {
        if (string.IsNullOrWhiteSpace(nameFriend))
        {
            Debug.Log("You must select a friend to chat");
            return;
        }

        if (string.IsNullOrWhiteSpace(text))
            return;
        var msg = new Hashtable
        {
            { "name", profileManager.GetProfile().userName },
            { "text", text }
        };
        Debug.Log("SendPrivateMessage: " + nameFriend);
        chatClient.SendPrivateMessage(nameFriend, msg);
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        view = GetComponent<ChatPageView>();
        chatManager = FindAnyObjectByType<ChatManager>();
        profileManager = FindAnyObjectByType<ProfileManager>();
        friendPagePresenter = FindAnyObjectByType<FriendPagePresenter>();
    }
}