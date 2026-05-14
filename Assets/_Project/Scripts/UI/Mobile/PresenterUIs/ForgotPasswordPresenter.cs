
using UnityEngine;
namespace TGTH.Mobile
{
    public class ForgotPasswordPresenter : TGTHMonoBehaviour
    {
        [SerializeField] private ForgotPageView view;
        private AuthFacade authFacade;
        private PlayfabDataManager playfabDataManager;
        protected override void Awake()
        {
            base.Awake();
            playfabDataManager = PlayfabDataManager.Instance;
            authFacade = playfabDataManager.GetAuthManager();
            view.OnStartClicked += OnStartClicked;
        }
        private void OnEnable()
        {
            view.HideMessege();
        }
        private void OnStartClicked(ForgotPasswordData data)
        {
            authFacade.ForgotPassword(data, onSuccess, onError);
        }

        private void onSuccess(string obj)
        {
            view.ShowMessege(obj);
        }

        private void onError(AuthError error)
        {
            view.ShowMessege(error.message);
        }
    }
}