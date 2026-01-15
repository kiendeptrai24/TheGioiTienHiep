using Photon.Chat;
using Photon.Chat.TGTHChat;
using UnityEngine;

public class ChatPagePresenter : TGTHMonoBehaviour
{
    [SerializeField] private ChatPageView view;
    [SerializeField] private ChatManager chatManager;
    private ChatClient chatClient;

    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        view.OnSubmitChat += OnSubmitChat;
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

    }

    private void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        ChatChannel channel = null;
        bool found = this.chatClient.TryGetChannel(channelName, out channel);
        if (!found)
        {
            Debug.Log("ShowChannel failed to find channel: " + channelName);
            return;
        }
        Debug.Log("ShowChannel: " + channelName);
        foreach (var sender in senders)
        {
            Debug.Log("Sender: " + sender + "\n");
        }
        foreach (var message in messages)
        {
            Debug.Log("Message: " + message + "\n");
        }

        string messageString = $"<b><color=#ce4627ff>{senders[0]}: </color></b>" + messages[0];

        view.ShowText(messageString);
    }

    private void OnSubmitChat(string text)
    {
        Debug.Log("Submit chat: " + text);
        this.chatClient.PublishMessage("Global", text);
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        view = GetComponent<ChatPageView>();
    }
}