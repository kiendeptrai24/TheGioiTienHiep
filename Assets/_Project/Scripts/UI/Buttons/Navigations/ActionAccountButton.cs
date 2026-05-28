
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.UI;

public class ActionAccountButton : TGTHMonoBehaviour
{
    [SerializeField] private Button okeBtn;
    private ProfileManager profileManager;
    private PlayfabDataManager playfabDataManager;
    protected override void Awake()
    {
        base.Awake();
        okeBtn = GetComponent<Button>();
        playfabDataManager = PlayfabDataManager.Instance;
        okeBtn.onClick.AddListener(OnClickBtn);
        profileManager = ProfileManager.Instance;
    }

    private void OnClickBtn()
    {
        var itemData = InventoryCenterManager.Instance.playerCham;
        var popup = PopupManager.Instance.GetPopup<AccountPopup>();
        Debug.Log(itemData.itemName);
        var currentProfile = profileManager.GetProfile();
        var data = new AccountDataPopup(itemData, currentProfile.userName, currentProfile.userId);
        popup.ShowPopup(data,
            null, null,
            () =>
            {
                playfabDataManager.Logout();
                Debug.Log("Logout");
            }
            , () =>
            {
                playfabDataManager.ChangeAccount();
                Debug.Log("Go to account management screen");
            });
    }

}