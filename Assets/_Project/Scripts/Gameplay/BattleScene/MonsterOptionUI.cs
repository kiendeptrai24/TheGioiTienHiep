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
    private EntityClickable currentEntity;

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
        PlayerNetManager.Instance.OnPlayerExiststed += OnPlayerExists;
    }

    private void OnPlayerExists(NetworkObject @object)
    {
        player = @object;
    }

    public void Show(EntityClickable entity)
    {
        currentEntity = entity;
        root.SetActive(true);
    }

    public void Hide()
    {
        currentEntity = null;
        root.SetActive(false);
    }

    public void OnLeaveClicked()
    {
        Hide();
    }

    public void OnAttackClicked()
    {
        currentEntity?.OnEntityClickedAccept(player);
        Hide();
    }
}
