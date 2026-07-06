
using PlayFab;
using UnityEngine;
namespace TGTH.Mobile
{
    public class RegisterPresenter : TGTHMonoBehaviour
    {
        private const string CreateAccountScreenName = "CreateAccount";

        [SerializeField] private RegisterPageView view;
        private AuthFacade authFacade;
        [SerializeField] private ActionNavigation navigation;
        protected override void Awake()
        {
            base.Awake();
            var clientAPI = new PlayFabClientInstanceAPI(PlayFabSettings.staticSettings);
            IAuthService authService = null;
            if (Configuration.Instance.buildType == BuildType.LOCAL_CLIENT)
            {
                authService = new PlayFabAuthService(clientAPI);
                authFacade = new AuthFacade(authService);
            }
            else if (Configuration.Instance.buildType == BuildType.REMOTE_CLIENT)
            {
                authService = new PlayFabAuthService(clientAPI);
                authFacade = new AuthFacade(authService);
            }
            authFacade = new AuthFacade(authService);
            view.OnRegisterClicked += OnRegisterClicked;
        }
        private void OnEnable()
        {
            view.HideMessege();
        }
        private void OnRegisterClicked(RegisterData data)
        {
            authFacade.Register(data, onSuccess, onError);
        }

        private void onError(AuthError error)
        {
            TopNotificationUI.Instance.ShowNotification(error.message);
            view.ShowMessege(error.message);
        }

        private void onSuccess(AuthResult result)
        {
            PlayFabClientAPI.ForgetAllCredentials();
            TopNotificationUI.Instance.ShowNotification(result.message);
            var createAccountScreen = ScreenManagerHub.Instance.Get(CreateAccountScreenName);
            if (createAccountScreen != null)
            {
                createAccountScreen.ResetNavigation();
                return;
            }

            navigation?.OnClick();
        }
    }
}
