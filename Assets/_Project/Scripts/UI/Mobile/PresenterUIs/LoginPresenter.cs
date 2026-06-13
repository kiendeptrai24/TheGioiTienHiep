
using System;
using System.Collections.Generic;
using UnityEngine;
namespace TGTH.Mobile
{
    public class LoginPresenter : TGTHMonoBehaviour
    {
        [SerializeField] private LoginPageView view;

        private PlayfabDataManager playfabDataManager;
        [SerializeField] private ActionNavigation navigation;

        protected override void Awake()
        {
            base.Awake();
            playfabDataManager = PlayfabDataManager.Instance;
            playfabDataManager.LoginSuccess += onSuccess;
            playfabDataManager.LoginError += onError;
            playfabDataManager.OnLoadCharacterFormPlayfab += OnStartGame;
            view.OnLoginClicked += OnStartClicked;
            GetAccountCache();
        }

        private void GetAccountCache()
        {
            string email = PlayerPrefs.GetString("EMAIL");
            string password = PlayerPrefs.GetString("PASSWORD");
            if (string.IsNullOrEmpty(email) == false || string.IsNullOrEmpty(password) == false)
            {
                view.ShowAccount(email, password);
            }
        }

        private void OnEnable()
        {
            view.HideMessege();
        }
        private void OnStartGame(List<ItemData> list)
        {
            navigation.OnClick();
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