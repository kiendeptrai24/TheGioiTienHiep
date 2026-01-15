
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatPageView : TGTHMonoBehaviour
{
    [SerializeField] private TMP_InputField chatInputField;
    [SerializeField] private TextMeshProUGUI chatText;
    [SerializeField] private Button submitButton;
    public Action<string> OnSubmitChat;
    public int TestLength = 2048;
    private byte[] testBytes = new byte[2048];
    protected override void Awake()
    {
        base.Awake();
        // Ensure TMP renders <color> tags and \n, \t correctly
        if (chatText != null)
        {
            chatText.richText = true;
            chatText.parseCtrlCharacters = true;
        }
        OnSubmitChat?.Invoke(chatInputField.text);
        submitButton.onClick.AddListener(() =>
        {
            OnSubmitChat?.Invoke(chatInputField.text);
            chatInputField.text = "";
        });
    }
    private string PostHelpToCurrentChannel(string inputLine)
    {
        return inputLine += HelpText;
    }
    public void ShowText(string message)
    {
        chatText.text = message;
        
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