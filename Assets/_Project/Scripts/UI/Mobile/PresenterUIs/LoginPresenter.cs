using System.Collections;
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
        private bool hasNavigatedAfterLogin;
        private bool isWaitingBeforeEnter;
        private Coroutine waitBeforeEnterCoroutine;
        private float lastLoginClickTime = -10f;

        protected override void Awake()
        {
            base.Awake();
            playfabDataManager = PlayfabDataManager.Instance;
            view.OnLoginClicked += OnStartClicked;
            GetAccountCache();
        }

        private void OnEnable()
        {
            hasNavigatedAfterLogin = false;
            BindEvents();
            SyncViewState();

            if (playfabDataManager != null &&
                playfabDataManager.IsAuthenticated &&
                !playfabDataManager.IsChangingAccount)
            {
                if (!view.AcccountOrPasswordIsEmpty())
                {
                    NavigateToCharacterSelection();
                }
            }
        }

        private void OnDisable()
        {
            CancelWaitBeforeEnter();
            UnbindEvents();
        }

        private void OnDestroy()
        {
            if (view != null)
            {
                view.OnLoginClicked -= OnStartClicked;
            }
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

        private void BindEvents()
        {
            if (playfabDataManager == null)
            {
                return;
            }

            playfabDataManager.LoginSuccess -= onSuccess;
            playfabDataManager.LoginError -= onError;
            playfabDataManager.LoginStatusChanged -= onStatusChanged;
            playfabDataManager.OnLoadCharacterFormPlayfab -= OnStartGame;

            playfabDataManager.LoginSuccess += onSuccess;
            playfabDataManager.LoginError += onError;
            playfabDataManager.LoginStatusChanged += onStatusChanged;
            playfabDataManager.OnLoadCharacterFormPlayfab += OnStartGame;
        }

        private void UnbindEvents()
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

        private void SyncViewState()
        {
            if (view == null)
            {
                return;
            }
            if (view.AcccountOrPasswordIsEmpty())
            {
                hasNavigatedAfterLogin = false;
                isWaitingBeforeEnter = false;
                view.HideMessege();
                SetLoginInProgress(false);
                return;
            }
            if (playfabDataManager == null)
            {
                view.HideMessege();
                hasNavigatedAfterLogin = false;
                SetLoginInProgress(false);
                return;
            }

            if (playfabDataManager.IsAuthenticated)
            {
                SetLoginInProgress(true);
                return;
            }

            if (playfabDataManager.IsAutoLoginInProgress || playfabDataManager.IsChangingAccount)
            {
                SetLoginInProgress(true);
                return;
            }

            hasNavigatedAfterLogin = false;
            isWaitingBeforeEnter = false;
            view.HideMessege();
            SetLoginInProgress(false);
        }

        private void OnStartGame(List<ItemData> list)
        {
            if (!isActiveAndEnabled)
            {
                return;
            }

            if (!playfabDataManager.IsAuthenticated)
            {
                return;
            }

            if (playfabDataManager.IsChangingAccount)
            {
                return;
            }

            if (isWaitingBeforeEnter)
            {
                return;
            }

            NavigateToCharacterSelection();
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
            hasNavigatedAfterLogin = false;
            isWaitingBeforeEnter = false;
            SetLoginInProgress(true);
            PlayfabDataManager.Instance.Login(data);
        }

        private void onSuccess(AuthResult result)
        {
            if (!isActiveAndEnabled || view == null)
            {
                return;
            }

            SetLoginInProgress(true);
            if (result != null && result.shouldWaitBeforeEnter)
            {
                isWaitingBeforeEnter = true;
                view.ShowMessege("Dang nhap thanh cong. Dang cho client cu thoat...");
                CancelWaitBeforeEnter();
                waitBeforeEnterCoroutine = StartCoroutine(WaitBeforeEnterRoutine(result.waitBeforeEnterSeconds));
                return;
            }

            isWaitingBeforeEnter = false;
            NavigateToCharacterSelection();
        }

        private void onError(AuthError error)
        {
            if (!isActiveAndEnabled || view == null)
            {
                return;
            }

            SetLoginInProgress(false);
            view.ShowMessege(error.message);
        }

        private void onStatusChanged(string message)
        {
            if (!isActiveAndEnabled || view == null)
            {
                return;
            }

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

        private void NavigateToCharacterSelection()
        {
            if (hasNavigatedAfterLogin || navigation == null || playfabDataManager == null)
            {
                return;
            }

            if (!playfabDataManager.IsAuthenticated || playfabDataManager.IsChangingAccount)
            {
                return;
            }

            isWaitingBeforeEnter = false;
            hasNavigatedAfterLogin = true;
            navigation.OnClick();
            SetLoginInProgress(false);
        }

        private IEnumerator WaitBeforeEnterRoutine(float waitSeconds)
        {
            yield return new WaitForSeconds(waitSeconds > 0f ? waitSeconds : 3f);
            waitBeforeEnterCoroutine = null;
            NavigateToCharacterSelection();
        }

        private void CancelWaitBeforeEnter()
        {
            if (waitBeforeEnterCoroutine == null)
            {
                return;
            }

            StopCoroutine(waitBeforeEnterCoroutine);
            waitBeforeEnterCoroutine = null;
        }
    }
}
