
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatPageView : TGTHMonoBehaviour
{
    [SerializeField] private TMP_InputField addFriendField;
    [SerializeField] Button addFriendBtn;
    public Action<string> OnAddFriend;

    [Header("Field")]
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private TMP_InputField chatInputZoomOutField;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI chatText;
    [SerializeField] private TextMeshProUGUI chatZoomOutText;
    [Header("Text Friend")]
    [SerializeField] private TextMeshProUGUI nameFriendText;
    [SerializeField] private TextMeshProUGUI nameFriendLargeText;

    [Header("Text Private")]
    [SerializeField] private TextMeshProUGUI chatPrivateText;
    [SerializeField] private TextMeshProUGUI chatPrivateZoomOutText;

    [Header("Button Navigation")]
    [Space]
    [SerializeField] private NavigationButton zoomInButton;
    [SerializeField] private NavigationButton zoomOutButton;
    [Space]
    [SerializeField] private NavigationButton chatGeneralSmallPanelBtn;
    [SerializeField] private NavigationButton chatPrivateSmallPanelBtn;
    [Space]
    [SerializeField] private NavigationButton chatGeneralLargePanelBtn;
    [SerializeField] private NavigationButton chatPrivateLargePanelBtn;


    [Header("Button Submit")]
    [SerializeField] private Button submitSmallChatButton;
    [SerializeField] private Button submitLargeChatButton;

    [Header("Zoom Out")]
    [SerializeField] private bool showZoomOut;
    public bool chatPrivate = false;

    private InputManager inputs;
    public Action<string> OnSubmitChat;

    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        addFriendBtn.onClick.AddListener(() =>
        {
            OnAddFriend?.Invoke(addFriendField.text);
            addFriendField.text = "";
            addFriendField.ActivateInputField();
        });

        if (chatText != null)
        {
            chatText.richText = true;
            chatText.parseCtrlCharacters = true;
        }
        OnSubmitChat?.Invoke(chatInputField.text);

        CallbackButtonNavigation();
        CallBackButtonSubmit();
        CallbackPressEnter();
    }

    private void CallbackPressEnter()
    {
        inputs.OnEnterClick += () =>
        {
            if (showZoomOut == false)
            {
                OnSubmitChat?.Invoke(chatInputField.text);
                chatInputField.text = "";
                chatInputField.ActivateInputField();
            }
            else
            {
                OnSubmitChat?.Invoke(chatInputZoomOutField.text);
                chatInputZoomOutField.text = "";
                chatInputZoomOutField.ActivateInputField();
            }
        };
    }

    private void CallBackButtonSubmit()
    {
        submitSmallChatButton.onClick.AddListener(() =>
        {
            OnSubmitChat?.Invoke(chatInputField.text);
            chatInputField.text = "";
            chatInputField.ActivateInputField();
        });

        submitLargeChatButton.onClick.AddListener(() =>
        {
            OnSubmitChat?.Invoke(chatInputZoomOutField.text);
            chatInputZoomOutField.text = "";
            chatInputZoomOutField.ActivateInputField();
        });
    }

    private void CallbackButtonNavigation()
    {
        zoomInButton.m_OnClick += () =>
        {
            showZoomOut = false;
        };
        zoomOutButton.m_OnClick += () =>
        {
            showZoomOut = true;
        };
        chatGeneralSmallPanelBtn.m_OnClick += () =>
        {
            chatPrivate = false;
        };
        chatPrivateSmallPanelBtn.m_OnClick += () =>
        {
            chatPrivate = true;
        };
        chatGeneralLargePanelBtn.m_OnClick += () =>
        {
            chatPrivate = false;
        };
        chatPrivateLargePanelBtn.m_OnClick += () =>
        {
            chatPrivate = true;
        };
    }

    protected override void LoadComponent()
    {
        base.LoadComponent();
        inputs = FindAnyObjectByType<InputManager>();
    }
    public void ShowText(string message)
    {
        chatZoomOutText.text += message + "\n";
        chatText.text += message + "\n";
    }
    public void ShowTextWithFriend(string message)
    {
        chatPrivateText.text += message + "\n";
        chatPrivateZoomOutText.text += message + "\n";
    }
    public void ShowNameFriend(string name)
    {
        var messageString = $"<b><color=#ce4627ff>Chò truyện: </color></b>{name}";
        nameFriendText.text = messageString;
    }
    private static string HelpText = "\n    -- HELP --\n" +
                                    "To subscribe to channel(s):\n" +
                                    "\t<color=#E07B00>\\subscribe</color> <color=green><list of channelnames></color>\n" +
                                    "\tor\n" +
                                    "\t<color=#E07B00>\\s</color> <color=green><list of channelnames></color>\n" +
                                    "\n" +
                                    "To leave channel(s):\n" +
                                    "\t<color=#E07B00>\\unsubscribe</color> <color=green><list of channelnames></color>\n" +
                                    "\tor\n" +
                                    "\t<color=#E07B00>\\u</color> <color=green><list of channelnames></color>\n" +
                                    "\n" +
                                    "To switch the active channel\n" +
                                    "\t<color=#E07B00>\\join</color> <color=green><channelname></color>\n" +
                                    "\tor\n" +
                                    "\t<color=#E07B00>\\j</color> <color=green><channelname></color>\n" +
                                    "\n" +
                                    "To send a private message:\n" +
                                    "\t\\<color=#E07B00>msg</color> <color=green><username></color> <color=green><message></color>\n" +
                                    "\n" +
                                    "To add friend(s):\n" +
                                    "\t\\<color=#E07B00>friend</color> <color=green><username></color> [<color=green><username></color>]\n" +
                                    "\n" +
                                    "To remove friend(s):\n" +
                                    "\t\\<color=#E07B00>unfriend</color> <color=green><username></color> [<color=green><username></color>]\n" +
                                    "\n" +
                                    "To change status:\n" +
                                    "\t\\<color=#E07B00>state</color> <color=green><stateIndex></color> <color=green><message></color>\n" +
                                    "<color=green>0</color> = Offline " +
                                    "<color=green>1</color> = Invisible " +
                                    "<color=green>2</color> = Online " +
                                    "<color=green>3</color> = Away \n" +
                                    "<color=green>4</color> = Do not disturb " +
                                    "<color=green>5</color> = Looking For Group " +
                                    "<color=green>6</color> = Playing" +
                                    "\n\n" +
                                    "To clear the current chat tab (private chats get closed):\n" +
                                    "\t<color=#E07B00>\\clear</color>";
}