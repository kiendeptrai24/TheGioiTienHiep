using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MonsterOptionUI : MonoBehaviour
{
    public static MonsterOptionUI Instance;
    [SerializeField] private Button leaveButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button infoButton;
    [SerializeField] private GameObject root;
    [SerializeField] private NetworkObject player;
    private MonsterClickable currentMonster;

    private void Awake()
    {
        Instance = this;
        leaveButton.onClick.AddListener(() =>
        {
            OnLeaveClicked();
        });
        attackButton.onClick.AddListener(() =>
        {
            OnAttackClicked();
        });
        PlayerNetManager.Instance.OnPlayerExists += OnPlayerExists;
    }

    private void OnPlayerExists(NetworkObject @object)
    {
        player = @object;
    }

    public void Show(MonsterClickable monster)
    {
        currentMonster = monster;
        root.SetActive(true);
    }

    public void Hide()
    {
        currentMonster = null;
        root.SetActive(false);
    }

    public void OnLeaveClicked()
    {
        Hide();
    }

    // Button: Đánh
    public void OnAttackClicked()
    {
        BattleSimulatorRequest.Instance.RequestBattleSimulatorServerRpc(player.OwnerClientId, currentMonster.NetworkObjectId);
        Hide();
    }
}
