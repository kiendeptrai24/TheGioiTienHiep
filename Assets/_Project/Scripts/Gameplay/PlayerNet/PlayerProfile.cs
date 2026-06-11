
using System;
using NUnit.Framework;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
public class PlayerProfile : TGTHNetworkBehaviour
{
    private ResourceStorage resourceStorage;
    [SerializeField]
    private NetworkVariable<FixedString64Bytes> playerId = new(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    private NetworkVariable<FixedString64Bytes> playerName = new(
        "",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    [SerializeField]
    private NetworkVariable<int> potentialPoint = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    [SerializeField]
    private NetworkVariable<int> skillPoint = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    private PlayerInventory inventory;
    public event Action OnProfileChanged;
    [SerializeField]
    private ProfileUser profileUser;
    private PlayerResource playerResource;

    public Action<string> OnPlayerNameChange;

    protected override void Awake()
    {
        base.Awake();
        resourceStorage = GetComponent<ResourceStorage>();
        inventory = GetComponent<PlayerInventory>();
        resourceStorage.OnSpiritStoneChanged += OnSpiritStoneChanged;
    }
    void OnDisable()
    {
        resourceStorage.OnSpiritStoneChanged -= OnSpiritStoneChanged;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerName.OnValueChanged += OnPlayerNameChanged;
        var playerProfile = GetComponent<PlayerProfile>();
        var user = ProfileManager.Instance.GetProfile();
        playerResource = new PlayerResource();

        if (IsOwner)
        {
            // callback networkvariable
            profileUser = user;
            playerProfile.OnProfileChanged += OnPlayerProfileChanged;
            potentialPoint.OnValueChanged += OnPotentialPointChanged;
            skillPoint.OnValueChanged += OnSkillPointChanged;
            OnSkillPointChanged(0, profileUser.skillPoint);

            // load coins to resourcestorage
            LoadCoinsServerRpc(profileUser.coins);
            LoadPlayerIdServerRpc(user.userId, user.potentialPoint, user.skillPoint);
        }
    }

    private void OnPlayerNameChanged(FixedString64Bytes previousValue, FixedString64Bytes newValue)
    {
        OnPlayerNameChange?.Invoke(newValue.ToString());
    }

    private void OnPlayerProfileChanged()
    {
        playerName.Value = profileUser.userName;
    }
    #region Event CallBack

    #region Callback Network Variable

    private void OnSkillPointChanged(int previousValue, int newValue)
    {
        if (profileUser == null) return;
        profileUser.skillPoint = newValue;
        OnProfileChanged?.Invoke();
    }

    private void OnPotentialPointChanged(int previousValue, int newValue)
    {
        if (profileUser == null) return;
        profileUser.potentialPoint = newValue;
        OnProfileChanged?.Invoke();
    }
    #endregion

    #region Callback Resoure Storage

    private void OnSpiritStoneChanged(ulong obj)
    {
        playerResource.linhThach = (int)resourceStorage.SpiritStone.Value;

        if (!IsOwner) return;

        if (profileUser == null) return;
        profileUser.coins = resourceStorage.SpiritStone.Value;
        profileUser.playerResource.linhThach = (int)resourceStorage.SpiritStone.Value;
        OnProfileChanged?.Invoke();
    }
    #endregion

    #endregion

    #region Get Data

    public PlayerResource GetPlayerResource()
    {
        if (playerResource == null)
            playerResource = new PlayerResource();
        playerResource.itemAmounts = inventory.GetItemRequirments();
        return playerResource;
    }
    public FixedString64Bytes GetPlayerId() => playerId.Value;
    public int GetPotentialPoint() => potentialPoint.Value;
    public int GetSkillPoint() => skillPoint.Value;
    #endregion

    #region Set Data
    public void SetPotentialPoint(int value)
    {
        if (!IsServer) return;
        if (value == 0) return;
        potentialPoint.Value += value;
    }

    public void SetSkillPoint(int value)
    {
        if (!IsServer) return;
        if (value == 0) return;
        skillPoint.Value += value;
    }
    #endregion

    #region Server Rpc

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void LoadCoinsServerRpc(ulong coins)
    {
        resourceStorage.InitSpiritStone(coins);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void LoadPlayerIdServerRpc(string playerId, int potentialPoint = 0, int skillPoint = 0)
    {
        this.playerId.Value = new FixedString64Bytes(playerId);
        this.potentialPoint.Value = potentialPoint;
        this.skillPoint.Value = skillPoint;
    }
    #endregion
}