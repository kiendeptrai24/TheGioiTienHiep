using UnityEngine;
using UnityEngine.UI;

public class UIPanelManager : TGTHMonoBehaviour
{
    [SerializeField] private Button menuBtn;
    [SerializeField] private GameObject menuPanel;
    protected override void Awake()
    {
        base.Awake();
        menuBtn.onClick.AddListener(ShowMenuPanel);
    }

    public void HideMenuPanel()
    {
        menuPanel.SetActive(false);
        menuBtn.gameObject.SetActive(true);
    }

    public void ShowMenuPanel()
    {
        menuPanel.SetActive(true);
        menuBtn.gameObject.SetActive(false);
    }

}
