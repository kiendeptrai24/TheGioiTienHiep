
using PlayFab;
using UnityEngine;
namespace TGTH.Mobile
{
    public class RegisterPresenter : TGTHMonoBehaviour
    {
        [SerializeField] private RegisterPageView view;
        private AuthManager authManager;
        [SerializeField] private ActionNavigation navigation;
        protected override void Awake()
        {
            base.Awake();
            var clientAPI = new PlayFabClientInstanceAPI(PlayFabSettings.staticSettings);
            IAuthService authService = new PlayFabAuthService(clientAPI);
            authManager = new AuthManager(authService);
            view.OnRegisterClicked += OnRegisterClicked;
        }
        private void OnEnable()
        {
            view.HideMessege();
        }
        private void OnRegisterClicked(RegisterData data)
        {
            authManager.Register(data, onSuccess, onError);
        }

        private void onError(AuthError error)
        {
            view.ShowMessege(error.message);
        }

        private void onSuccess(AuthResult result)
        {
            Debug.Log(result.message);
            navigation.OnClick();
        }
    }
}