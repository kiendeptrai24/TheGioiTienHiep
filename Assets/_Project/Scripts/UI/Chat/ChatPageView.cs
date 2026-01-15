
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatPageView : TGTHMonoBehaviour
{
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private TextMeshProUGUI chatText;
    [SerializeField] private TMP_InputField chatInputZoomOutField;
    [SerializeField] private TextMeshProUGUI chatZoomOutText;
    [SerializeField] private NavigationButton zoomInButton;
    [SerializeField] private NavigationButton zoomOutButton;
    [SerializeField] private bool showZoomOut;
    [SerializeField] private Button submitSmallChatButton;
    [SerializeField] private Button submitLargeChatButton;
    private InputManager inputs;
    public Action<string> OnSubmitChat;
    public int TestLength = 2048;
    private byte[] testBytes = new byte[2048];
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        if (chatText != null)
        {
            chatText.richText = true;
            chatText.parseCtrlCharacters = true;
        }
        OnSubmitChat?.Invoke(chatInputField.text);
        zoomInButton.m_OnClick += () =>
        {
            showZoomOut = false;
        };
        zoomOutButton.m_OnClick += () =>
        {
            showZoomOut = true;
        };
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