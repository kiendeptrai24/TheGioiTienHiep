
using UnityEngine;
using UnityEngine.UI;

public class ActionAccountButton : TGTHMonoBehaviour
{
    [SerializeField] private Button okeBtn;
    private ProfileManager profileManager;
    private PlayfabDataManager playfabDataManager;
    private SaveLoadManager saveManager;
    protected override void Awake()
    {
        base.Awake();
        okeBtn = GetComponent<Button>();
        playfabDataManager = PlayfabDataManager.Instance;
        saveManager = SaveLoadManager.Instance;
        okeBtn.onClick.AddListener(OnClickBtn);
        profileManager = ProfileManager.Instance;
    }

    private void OnClickBtn()
    {
        var itemData = InventoryCenterManager.Instance.playerCham;
        var popup = PopupManager.Instance.GetPopup<AccountPopup>();
        var currentProfile = profileManager.GetProfile();
        var data = new AccountDataPopup(itemData, currentProfile.userName, currentProfile.userId, currentProfile.createdAt);
        popup.ShowPopup(data,
            null, null,
            () =>
            {
                playfabDataManager.Logout();
                saveManager.SaveGame();
            }
            , () =>
            {
                playfabDataManager.ChangeAccount();
                saveManager.SaveGame();
            });
    }

}