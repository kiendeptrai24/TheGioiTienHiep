using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Chat.TGTHChat;
using TGTH.Mobile;
using UnityEngine;

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
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        view.OnSubmitChat += (text) =>
        {
            if (view.chatPrivate)
            {
                OnSendPrivateChat(text);
            }
            else
            {
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
            $"<b><color=#ce4627ff>{nameUser}: </color></b>{text}";

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
            { "name", profileManager.GetProfileUser().userName },
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
            { "name", profileManager.GetProfileUser().userName },
            { "text", text }
        };
        Debug.Log("SendPrivateMessage: " + nameFriend);
        chatClient.SendPrivateMessage(nameFriend, msg);
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        view = GetComponent<ChatPageView>();
    }
}