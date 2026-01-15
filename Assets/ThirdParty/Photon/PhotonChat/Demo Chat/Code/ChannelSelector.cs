using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace Photon.Chat.DemoChat
{
    public class ChannelSelector : MonoBehaviour, IPointerClickHandler
    {
        public string Channel;

        public void SetChannel(string channel)
        {
            this.Channel = channel;
            Text t = GetComponentInChildren<Text>();
            t.text = this.Channel;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ChatNewGui handler = FindObjectOfType<ChatNewGui>();
            handler.ShowChannel(this.Channel);
        }
    }
}