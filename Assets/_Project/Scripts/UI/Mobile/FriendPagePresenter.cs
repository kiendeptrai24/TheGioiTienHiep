using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace TGTH.Mobile
{
    public class FriendPagePresenter : TGTHMonoBehaviour
    {
        [SerializeField] private FriendPageView view;
        [SerializeField] private ProfileManager profileManager;
        public event Action<string> OnSwitchFriend;
        protected override void Awake()
        {
            base.Awake();
            view.OnAddFriend += OnAddFriend;
        }
        protected override void Start()
        {
            base.Start();
            Init();
        }
        private void Init()
        {
            var listFriend = profileManager.GetProfileUser().GetListFriend();
            for (int i = 0; i < listFriend.Count; i++)
            {
                UIItemFriend uiItemFriend = Instantiate(view.uIItemFriend, view.contentPanel);
                uiItemFriend.OnItemClicked += OnUIItemFriendClicked;
                uiItemFriend.SetName(listFriend[i]);
            }
        }
        private void OnUIItemFriendClicked(UIItemFriend uiItemFriend)
        {
            Debug.Log("Switch friend " + uiItemFriend.nameTxt.text);
            OnSwitchFriend?.Invoke(uiItemFriend.nameTxt.text);
        }
        private void OnAddFriend(string name)
        {
            var nameFriend = name;
            profileManager.AddFriend(nameFriend);

            UIItemFriend uiItemFriend = Instantiate(view.uIItemFriend, view.contentPanel);
            uiItemFriend.OnItemClicked += OnUIItemFriendClicked;
            uiItemFriend.SetName(name);
        }
        private void OnRemoveFriend(string name)
        {
            var nameFriend = name;
            profileManager.RemoveFriend(nameFriend);
        }
    }
}
