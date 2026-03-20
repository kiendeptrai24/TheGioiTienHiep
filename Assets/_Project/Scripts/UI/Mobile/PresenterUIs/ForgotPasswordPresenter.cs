
using UnityEngine;
namespace TGTH.Mobile
{
    public class ForgotPasswordPresenter : TGTHMonoBehaviour
    {
        [SerializeField] private ForgotPageView view;
        private InventoryCenterManager inventoryCenterManager;
        private AuthManager authManager;

        protected override void Awake()
        {
            base.Awake();
            IAuthService authService = new PlayFabAuthService();
            authManager = new AuthManager(authService);
            inventoryCenterManager = InventoryCenterManager.Instance;
            view.OnStartClicked += OnStartClicked;
        }

        private void OnStartClicked(ForgotPasswordData data)
        {
            authManager.ForgotPassword(data, onSuccess, onError);
        }

        private void onSuccess(string obj)
        {
            view.ShowError(obj);
        }

        private void onError(AuthError error)
        {
            view.ShowError(error.message);
        }
    }
}