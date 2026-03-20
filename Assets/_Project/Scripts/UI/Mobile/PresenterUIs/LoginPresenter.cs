
using System;
using System.Collections.Generic;

using UnityEngine;
namespace TGTH.Mobile
{
    public class LoginPresenter : TGTHMonoBehaviour
    {
        [SerializeField] private LoginPageView view;

        private PlayfabDataManager playfabDataManager;
        protected override void Awake()
        {
            base.Awake();
            playfabDataManager = PlayfabDataManager.Instance;
            playfabDataManager.LoginSuccess += onSuccess;
            playfabDataManager.LoginError += onError;
            view.OnLoginClicked += OnStartClicked;
        }

        private void OnStartClicked(LoginData data)
        {
            PlayfabDataManager.Instance.Login(data);
        }

        private void onSuccess(AuthResult result)
        {
            view.ShowMessege(result.message);

        }
        private void onError(AuthError error)
        {
            view.ShowMessege(error.message);
        }

    }
}