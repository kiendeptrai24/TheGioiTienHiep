using UnityEngine;

public class BattleUIController : MonoBehaviour
{
    public static BattleUIController Instance { get; private set; }
    [SerializeField] private GameObject worldHUD;
    [SerializeField] private GameObject battleHUD;

    private void Awake() => Instance = this;

    public void EnterBattle(int sessionId)
    {
        if (worldHUD) worldHUD.SetActive(false);
        if (battleHUD) battleHUD.SetActive(true);
    }

    public void ExitBattle()
    {
        if (battleHUD) battleHUD.SetActive(false);
        if (worldHUD) worldHUD.SetActive(true);
    }
}
