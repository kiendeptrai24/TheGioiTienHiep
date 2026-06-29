using System.Collections.Generic;
using UnityEngine;

namespace TGTH.Mobile
{
    public class LoginPresenter : TGTHMonoBehaviour
    {
        private const float LoginClickCooldownSeconds = 0.75f;

        [SerializeField] private LoginPageView view;
        [SerializeField] private ActionNavigation navigation;

        private PlayfabDataManager playfabDataManager;
        private bool isLoginInProgress;
        private float lastLoginClickTime = -10f;

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
            if (view != null)
            {
                view.OnLoginClicked -= OnStartClicked;
            }

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
            SetLoginInProgress(false);
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
            if (view == null || playfabDataManager == null)
            {
                return;
            }

            if (isLoginInProgress)
            {
                view.ShowMessege("Dang trong qua trinh dang nhap. Vui long cho.");
                return;
            }

            if (Time.unscaledTime - lastLoginClickTime < LoginClickCooldownSeconds)
            {
                view.ShowMessege("Ban thao tac qua nhanh. Vui long thu lai sau mot chut.");
                return;
            }

            if (playfabDataManager.IsAuthenticated)
            {
                view.ShowMessege("Tai khoan da dang nhap.");
                return;
            }

            if (playfabDataManager.IsChangingAccount)
            {
                view.ShowMessege("Dang chuyen tai khoan. Vui long cho.");
                return;
            }

            if (data == null)
            {
                view.ShowMessege("Du lieu dang nhap khong hop le.");
                return;
            }

            lastLoginClickTime = Time.unscaledTime;
            SetLoginInProgress(true);
            PlayfabDataManager.Instance.Login(data);
        }

        private void onSuccess(AuthResult result)
        {
            SetLoginInProgress(false);
            view.ShowMessege(result.message);
        }

        private void onError(AuthError error)
        {
            SetLoginInProgress(false);
            view.ShowMessege(error.message);
        }

        private void onStatusChanged(string message)
        {
            view.ShowMessege(message);
        }

        private void SetLoginInProgress(bool inProgress)
        {
            isLoginInProgress = inProgress;
            if (view != null)
            {
                view.SetLoginInProgress(inProgress);
            }
        }
    }
}
