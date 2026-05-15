
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatPageView : TGTHMonoBehaviour
{
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
    public bool chatPrivateSmallSave = false;
    public bool chatPrivateLargeSave = false;

    private InputManager inputs;
    public Action<string> OnSubmitChat;

    protected override void Awake()
    {
        base.Awake();
        LoadComponent();

        if (chatText != null)
        {
            chatText.richText = true;
            chatText.parseCtrlCharacters = true;
        }

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
                if (EmptyText(chatInputZoomOutField.text))
                    return;

                OnSubmitChat?.Invoke(chatInputField.text);
                chatInputField.text = "";
                chatInputField.ActivateInputField();
            }
            else
            {
                if (EmptyText(chatInputZoomOutField.text))
                    return;

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
            if (EmptyText(chatInputZoomOutField.text))
                return;

            OnSubmitChat?.Invoke(chatInputField.text);
            chatInputField.text = "";
            chatInputField.ActivateInputField();
        });

        submitLargeChatButton.onClick.AddListener(() =>
        {
            if (EmptyText(chatInputZoomOutField.text))
                return;

            OnSubmitChat?.Invoke(chatInputZoomOutField.text);
            chatInputZoomOutField.text = "";
            chatInputZoomOutField.ActivateInputField();
        });
    }
    private bool EmptyText(string text) => string.IsNullOrWhiteSpace(text);
    private void CallbackButtonNavigation()
    {
        zoomInButton.m_OnClick += () =>
        {
            showZoomOut = false;
            chatPrivate = chatPrivateSmallSave;
        };
        zoomOutButton.m_OnClick += () =>
        {
            showZoomOut = true;
            chatPrivate = chatPrivateLargeSave;
        };
        chatGeneralSmallPanelBtn.m_OnClick += () =>
        {
            chatPrivateSmallSave = false;
            chatPrivate = false;
        };
        chatPrivateSmallPanelBtn.m_OnClick += () =>
        {
            chatPrivateSmallSave = true;
            chatPrivate = true;
        };
        chatGeneralLargePanelBtn.m_OnClick += () =>
        {
            chatPrivateLargeSave = false;
            chatPrivate = false;
        };
        chatPrivateLargePanelBtn.m_OnClick += () =>
        {
            chatPrivateLargeSave = true;
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
    public void ResetChatWithFriend()
    {
        chatPrivateText.text = "";
        chatPrivateZoomOutText.text = "";
    }
    public void ShowNameFriend(string name)
    {
        var messageString = $"<b><color=#ce4627ff>Chò truyện: </color></b>{name}";
        nameFriendText.text = messageString;
    }
}