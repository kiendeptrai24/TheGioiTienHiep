using System;
using Photon.Chat.TGTHChat;
using UnityEngine;

namespace TGTH.Mobile
{
    public class ProfilePagePresenter : TGTHMonoBehaviour
    {
        [SerializeField] private ProfilePageView view;
        [SerializeField] private ProfileManager profileManager;
        protected override void Awake()
        {
            base.Awake();
            view.OnUserIdChanged += OnUserIdChanged;
            view.OnUserNameChanged += OnUserNameChanged;
            profileManager.OnProfileChanged += OnProfileChanged;
            view.ShowUser(profileManager.GetProfile());
        }

        private void OnProfileChanged(ProfileUser user)
        {
            view.ShowUser(user);
        }

        private void OnUserNameChanged(string text)
        {
            var userId = profileManager.GetProfile().userId;
            profileManager.GetProfile().userName = text;
            profileManager.SetProfileUser(userId, text);
            view.ShowUserName(text);
        }

        private void OnUserIdChanged(string text)
        {
            view.ShowUserId(text);
        }

        protected override void LoadComponent()
        {
            base.LoadComponent();
            view = GetComponent<ProfilePageView>();
        }
    }
}
