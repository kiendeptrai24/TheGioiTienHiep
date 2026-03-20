
using System;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
namespace TGTH.Mobile
{
    public class RegisterPresenter : TGTHMonoBehaviour
    {
        [SerializeField] private RegisterPageView view;
        private AuthManager authManager;

        protected override void Awake()
        {
            base.Awake();
            IAuthService authService = new PlayFabAuthService();
            authManager = new AuthManager(authService);
            view.OnRegisterClicked += OnRegisterClicked;
        }

        private void OnRegisterClicked(RegisterData data)
        {
            authManager.Register(data, onSuccess, onError);
        }

        private void onError(AuthError error)
        {
            view.ShowError(error.message);
        }

        private void onSuccess(AuthResult result)
        {
            Debug.Log(result.message);
        }
    }
}