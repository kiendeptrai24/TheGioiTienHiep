using System.Collections.Generic;
using UnityEngine;

namespace TGTH.Mobile
{
    public class LoginPresenter : TGTHMonoBehaviour
    {
        [SerializeField] private LoginPageView view;
        [SerializeField] private ActionNavigation navigation;

        private PlayfabDataManager playfabDataManager;

        protected override void Awake()
        {
            base.Awake();
            playfabDataManager = PlayfabDataManager.Instance;
            playfabDataManager.LoginSuccess += onSuccess;
            playfabDataManager.LoginError += onError;
            playfabDataManager.LoginStatusChanged += onStatusChanged;
            playfabDataManager.OnLoadCharacterFormPlayfab += OnStartGame;
            view.OnLoginClicked += OnStartClicked;
            GetAccountCache();
        }

        private void OnDestroy()
        {
            if (playfabDataManager == null)
            {
                return;
            }

            playfabDataManager.LoginSuccess -= onSuccess;
            playfabDataManager.LoginError -= onError;
            playfabDataManager.LoginStatusChanged -= onStatusChanged;
            playfabDataManager.OnLoadCharacterFormPlayfab -= OnStartGame;
        }

        private void GetAccountCache()
        {
            string email = PlayerPrefs.GetString("EMAIL");
            string password = PlayerPrefs.GetString("PASSWORD");
            if (!string.IsNullOrEmpty(email) || !string.IsNullOrEmpty(password))
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
            if (!playfabDataManager.IsAuthenticated)
            {
                return;
            }

            if (playfabDataManager.IsChangingAccount)
            {
                return;
            }

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

        private void onStatusChanged(string message)
        {
            view.ShowMessege(message);
        }
    }
}
