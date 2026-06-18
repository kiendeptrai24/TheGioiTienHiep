
using System;
using ExitGames.Client.Photon;
using Photon.Chat.DemoChat;
using UnityEngine;
namespace Photon.Chat.TGTHChat
{
    public class ChatManager : TGTHMonoBehaviour, IChatClientListener
    {
        public enum ClientChatState
        {
            None,
            Connecting,
            Connected,
            Disconnected
        }
        public enum UserStatus
        {
            Offline = 0,
            Invisible = 1,
            Online = 2,
            Away = 3,
            DoNotDisturb = 4,
            LookingForGroup = 5,
            Playing = 6
        }
        [SerializeField] private ProfileManager profileManager;
        public ChatClient chatClient;
        public ClientChatState clientState;
        public event Action OnClientChatConnected;
        public event Action OnClientChatDisconnected;

        public event Action<string, string[], object[]> OnGetMessages;
        public event Action<string, object, string> OnPrivateMessage;
        public event Action<string, int, bool, object> OnFriendStatusUpdate;

        protected override void Awake()
        {
            base.Awake();
        }
        protected override void Start()
        {
            base.Start();
            clientState = ClientChatState.None;
            ClientConnect();
        }

        public void ClientConnect()
        {
            chatClient = new ChatClient(this);

#if UNITY_WEBGL
            chatClient.UseBackgroundWorkerForSending = false;
#else
            chatClient.UseBackgroundWorkerForSending = true;
#endif

            var auth = new AuthenticationValues(profileManager.GetProfile().userId);

            clientState = ClientChatState.Connecting;
            chatClient.ChatRegion = "Asia";
            chatClient.MessageLimit = 100;
            bool ok = chatClient.Connect(ChatSettings.Instance.AppId, "1.0", auth);
        }

        private void Update()
        {
            chatClient?.Service();
        }
        public void ClientDisconnect()
        {
            if (this.chatClient != null)
            {
                this.chatClient.Disconnect();
            }
            clientState = ClientChatState.Disconnected;
        }
        public void OnDestroy()
        {
            if (this.chatClient != null)
            {
                this.chatClient.Disconnect();
            }
        }
        #region IChatClientListener implementation

        public void DebugReturn(DebugLevel level, string message)
        {
            Debug.LogWarning($"[PhotonChat] {level}: {message}");
        }

        public void OnDisconnected()
        {
            clientState = ClientChatState.Disconnected;
            chatClient.SetOnlineStatus((int)UserStatus.Online, profileManager.GetProfile().userName);
            OnClientChatDisconnected?.Invoke();
        }

        public void OnConnected()
        {
            Debug.Log("[PhotonChat] Connected");

            clientState = ClientChatState.Connected;
            chatClient.SetOnlineStatus((int)UserStatus.Online, profileManager.GetProfile().userName);

            chatClient.Subscribe(new[] { "Global" });
            OnClientChatConnected?.Invoke();
        }

        public void OnChatStateChange(ChatState state)
        {
            Debug.Log($"[PhotonChat] State: {state}");
        }
        public void OnSubscribed(string[] channels, bool[] results)
        {

        }

        public void OnUnsubscribed(string[] channels)
        {

        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            Console.WriteLine("Status change for: {0} to: {1}", user, status);
            OnFriendStatusUpdate?.Invoke(user, status, gotMessage, message);
        }

        public void OnUserSubscribed(string channel, string user)
        {

        }

        public void OnUserUnsubscribed(string channel, string user)
        {

        }

        void IChatClientListener.OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            Debug.Log("OnGetMessages: " + channelName);
            OnGetMessages?.Invoke(channelName, senders, messages);
        }

        void IChatClientListener.OnPrivateMessage(string sender, object message, string channelName)
        {
            OnPrivateMessage?.Invoke(sender, message, channelName);
        }
        #endregion
    }

}