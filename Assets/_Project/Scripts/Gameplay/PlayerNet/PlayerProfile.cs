
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
    private NetworkVariable<int> potentialPoint = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    private NetworkVariable<int> skillPoint = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    private ProfileUser profileUser;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        var user = ProfileManager.Instance.GetProfile();
        if (IsOwner)
        {
            var inventoryCenterManager = InventoryCenterManager.Instance;
            profileUser = user;

            resourceStorage = GetComponent<ResourceStorage>();
            resourceStorage.OnCoinsChanged += OnCoinsChanged;
            potentialPoint.OnValueChanged += OnPotentialPointChanged;
            skillPoint.OnValueChanged += OnSkillPointChanged;
            inventoryCenterManager.OnItemPlayerChanged += OnItemPlayerChanged;
            OnItemPlayerChanged(inventoryCenterManager.playerCham);
        }

        OnLoadPlayerIdServerRpc(user.userId, user.potentialPoint, user.skillPoint);
    }
    #region Event CallBack

    private void OnSkillPointChanged(int previousValue, int newValue)
    {
        if (profileUser == null) return;
        profileUser.skillPoint = newValue;
    }

    private void OnPotentialPointChanged(int previousValue, int newValue)
    {
        if (profileUser == null) return;
        profileUser.potentialPoint = newValue;
    }

    private void OnCoinsChanged(ulong obj)
    {
        if (profileUser == null) return;
        profileUser.playerResource.linhThach = (int)resourceStorage.Coins.Value;
    }

    private void OnItemPlayerChanged(ItemData data)
    {
        if (data == null) return;
        GetComponent<StatsData>().SetUpItem(data);
    }
    #endregion
    
    #region Get Data

    public PlayerResource GetPlayerResource()
    {
        if (profileUser == null) return null;
        return profileUser.playerResource;
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
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void OnLoadPlayerIdServerRpc(string playerId, int potentialPoint = 0, int skillPoint = 0)
    {
        if (!IsServer) return;
        this.playerId.Value = new FixedString64Bytes(playerId);
        this.potentialPoint.Value = potentialPoint;
        this.skillPoint.Value = skillPoint;
    }

}