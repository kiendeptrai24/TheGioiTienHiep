using System;
using ExitGames.Client.Photon;
using Photon.Chat.DemoChat;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Photon.Chat.TGTHChat
{
    public class ChatManager : TGTHMonoBehaviour, IChatClientListener
    {
        private Configuration config;
        [Header("Photon Chat")]
        [SerializeField] private string chatAppId = "YOUR_PHOTON_CHAT_APP_ID";
        [SerializeField] private string chatAppVersion = "1.0";
        [SerializeField] private string chatRegion = "ASIA";

        [Header("References")]
        [SerializeField] private ProfileManager profileManager;

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

        public ChatClient chatClient;
        public ClientChatState clientState;

        public event Action OnClientChatConnected;
        public event Action OnClientChatDisconnected;

        public event Action<string, string[], object[]> OnGetMessages;
        public event Action<string, object, string> OnPrivateMessage;
        public event Action<string, int, bool, object> OnFriendStatusUpdate;

        private PlayfabDataManager playfabDataManager;

        [SerializeField] private string playFabId;
        [SerializeField] private string photonToken;
        private PlayFabClientInstanceAPI clientApi;

        protected override void Awake()
        {
            base.Awake();

            clientState = ClientChatState.None;
            config = Configuration.Instance;
            if (!string.IsNullOrEmpty(config.PlayFabTitleId) && config.PlayFabTitleId != "YOUR_PLAYFAB_TITLE_ID")
            {
                PlayFabSettings.staticSettings.TitleId = config.PlayFabTitleId;
            }

            playfabDataManager = PlayfabDataManager.Instance;

            if (playfabDataManager != null)
            {
                playfabDataManager.LoginSuccess += OnLoginSuccess;
            }
            else
            {
                Debug.LogError("[PhotonChat] PlayfabDataManager.Instance is null.");
            }
            chatAppId = ChatSettings.Instance.AppId;
        }

        private void Update()
        {
            chatClient?.Service();
        }

        private void OnLoginSuccess(AuthResult result)
        {
            clientApi = result.clientApi ?? playfabDataManager?.GetClientAPI();
            playFabId = result.userId;

            if (string.IsNullOrEmpty(playFabId) && clientApi != null)
            {
                playFabId = clientApi.authenticationContext != null
                    ? clientApi.authenticationContext.PlayFabId
                    : string.Empty;
            }

            if (clientApi == null)
            {
                Debug.LogError("[PhotonChat] ClientApi is null after login success.");
                return;
            }

            if (string.IsNullOrEmpty(playFabId))
            {
                Debug.LogError("[PhotonChat] PlayFabId is null or empty.");
                return;
            }

            Debug.Log($"[PhotonChat] PlayFab login success. PlayFabId: {playFabId}");

            GetPhotonAuthenticationToken();
        }

        private void GetPhotonAuthenticationToken()
        {
            if (string.IsNullOrEmpty(chatAppId) || chatAppId == "YOUR_PHOTON_CHAT_APP_ID")
            {
                Debug.LogError("[PhotonChat] Photon Chat AppId is empty. Please set chatAppId in Inspector.");
                return;
            }

            var request = new GetPhotonAuthenticationTokenRequest
            {
                PhotonApplicationId = chatAppId
            };

            clientApi.GetPhotonAuthenticationToken(
                request,
                OnGetPhotonTokenSuccess,
                OnPlayFabError
            );
        }

        private void OnGetPhotonTokenSuccess(GetPhotonAuthenticationTokenResult result)
        {
            photonToken = result.PhotonCustomAuthenticationToken;

            if (string.IsNullOrEmpty(photonToken))
            {
                Debug.LogError("[PhotonChat] Photon token is null or empty.");
                return;
            }

            Debug.Log("[PhotonChat] Photon token received.");

            ClientConnect();
        }

        private void OnPlayFabError(PlayFabError error)
        {
            Debug.LogError($"[PhotonChat] PlayFab error: {error.GenerateErrorReport()}");
        }

        public void ClientConnect()
        {
            if (clientState == ClientChatState.Connecting || clientState == ClientChatState.Connected)
            {
                Debug.LogWarning("[PhotonChat] Chat client is already connecting or connected.");
                TopNotificationUI.Instance.ShowNotification("chat đã kết nối");
                return;
            }

            if (string.IsNullOrEmpty(playFabId))
            {
                Debug.LogError("[PhotonChat] Cannot connect. PlayFabId is null or empty.");
                TopNotificationUI.Instance.ShowNotification("Không thể kết nối. PlayFabId trống.");
                return;
            }

            if (string.IsNullOrEmpty(photonToken))
            {
                Debug.LogError("[PhotonChat] Cannot connect. Photon token is null or empty.");
                TopNotificationUI.Instance.ShowNotification("Không thể kết nối. Token trống.");
                return;
            }

            if (string.IsNullOrEmpty(chatAppId) || chatAppId == "YOUR_PHOTON_CHAT_APP_ID")
            {
                Debug.LogError("[PhotonChat] Cannot connect. Photon Chat AppId is empty.");
                TopNotificationUI.Instance.ShowNotification("Không thể kết nối. Chat AppId trống.");
                return;
            }

            chatClient = new ChatClient(this);

#if UNITY_WEBGL
            chatClient.UseBackgroundWorkerForSending = false;
#else
            chatClient.UseBackgroundWorkerForSending = true;
#endif

            AuthenticationValues authValues = new AuthenticationValues();
            authValues.AuthType = CustomAuthenticationType.Custom;

            // PlayFab + Photon Custom Authentication
            authValues.AddAuthParameter("username", playFabId);
            authValues.AddAuthParameter("token", photonToken);

            // Photon Chat bắt buộc cần UserId
            authValues.UserId = playFabId;

            chatClient.ChatRegion = chatRegion;
            chatClient.MessageLimit = 100;

            clientState = ClientChatState.Connecting;

            bool connectResult = chatClient.Connect(chatAppId, chatAppVersion, authValues);

            Debug.Log($"[PhotonChat] Connect result: {connectResult}");
        }

        public void ClientDisconnect()
        {
            if (chatClient != null)
            {
                if (clientState == ClientChatState.Connected)
                {
                    chatClient.SetOnlineStatus((int)UserStatus.Offline);
                }

                chatClient.Disconnect();
            }

            clientState = ClientChatState.Disconnected;
        }

        private void OnDestroy()
        {
            if (playfabDataManager != null)
            {
                playfabDataManager.LoginSuccess -= OnLoginSuccess;
            }

            if (chatClient != null)
            {
                chatClient.Disconnect();
                chatClient = null;
            }
        }

        #region IChatClientListener

        public void DebugReturn(DebugLevel level, string message)
        {
            Debug.LogWarning($"[PhotonChat] {level}: {message}");
        }

        public void OnConnected()
        {
            Debug.Log("[PhotonChat] Connected.");

            clientState = ClientChatState.Connected;

            string displayName = playFabId;

            if (profileManager != null && profileManager.GetProfile() != null)
            {
                displayName = profileManager.GetProfile().userName;
            }

            chatClient.SetOnlineStatus((int)UserStatus.Online, displayName);

            chatClient.Subscribe(new[] { "Global" });

            OnClientChatConnected?.Invoke();
        }

        public void OnDisconnected()
        {
            Debug.Log("[PhotonChat] Disconnected.");

            clientState = ClientChatState.Disconnected;

            OnClientChatDisconnected?.Invoke();
        }

        public void OnChatStateChange(ChatState state)
        {
            Debug.Log($"[PhotonChat] State: {state}");
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            for (int i = 0; i < channels.Length; i++)
            {
                Debug.Log($"[PhotonChat] Subscribed channel: {channels[i]}, result: {results[i]}");
            }
        }

        public void OnUnsubscribed(string[] channels)
        {
            foreach (string channel in channels)
            {
                Debug.Log($"[PhotonChat] Unsubscribed channel: {channel}");
            }
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            Debug.Log($"[PhotonChat] Status update. User: {user}, Status: {status}, GotMessage: {gotMessage}, Message: {message}");

            OnFriendStatusUpdate?.Invoke(user, status, gotMessage, message);
        }

        public void OnUserSubscribed(string channel, string user)
        {
            Debug.Log($"[PhotonChat] User subscribed. Channel: {channel}, User: {user}");
        }

        public void OnUserUnsubscribed(string channel, string user)
        {
            Debug.Log($"[PhotonChat] User unsubscribed. Channel: {channel}, User: {user}");
        }

        void IChatClientListener.OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            Debug.Log($"[PhotonChat] OnGetMessages. Channel: {channelName}");
            OnGetMessages?.Invoke(channelName, senders, messages);
        }

        void IChatClientListener.OnPrivateMessage(string sender, object message, string channelName)
        {
            Debug.Log($"[PhotonChat] Private message from: {sender}, Channel: {channelName}, Message: {message}");
            OnPrivateMessage?.Invoke(sender, message, channelName);
        }
        #endregion
    }
}
