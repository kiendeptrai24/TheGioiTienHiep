using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace TGTH.Mobile
{
    public class FriendPageView : TGTHMonoBehaviour
    {
        [SerializeField] private TMP_InputField addFriendField;
        [SerializeField] private Button addFriendBtn;
        [SerializeField] private Toggle showListFriend;
        [SerializeField] private GameObject listFriend;
        public Transform contentPanel;
        public UIItemFriend uIItemFriend;
        public Action<string> OnAddFriend;
        public Action<string> OnRemoveFriend;
        protected override void Awake()
        {
            base.Awake();
            LoadComponent();

            addFriendBtn.onClick.AddListener(() =>
            {
                if (string.IsNullOrEmpty(addFriendField.text))
                    return;
                OnAddFriend?.Invoke(addFriendField.text);
                addFriendField.text = "";
                addFriendField.ActivateInputField();
            });

            showListFriend.onValueChanged.AddListener((value) =>
            {
                if (value)
                    ShowListFriend();
                else
                    HideListFriend();
            });
        }
        public void ShowListFriend()
        {
            listFriend.SetActive(true);
        }
        public void HideListFriend()
        {
            listFriend.SetActive(false);
        }
        protected override void LoadComponent()
        {
            base.LoadComponent();
        }
    }
}
