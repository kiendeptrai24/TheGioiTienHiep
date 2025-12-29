

using UnityEngine;
using UnityEngine.UI;

public class UseItemPopupBtn : TGTHMonoBehaviour
{
    private Button useItemPopupbtn;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
        useItemPopupbtn.onClick.AddListener(OnClick);
    }
    public void OnClick()
    {
        var popup = PopupManager.Instance.GetPopup<UseItemPopup>();
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        useItemPopupbtn = GetComponent<Button>();
    }
}