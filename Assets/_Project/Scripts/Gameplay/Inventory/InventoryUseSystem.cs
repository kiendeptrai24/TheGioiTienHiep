

using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class InventoryUseSystem : SingletonNetwork<InventoryUseSystem>, IUsable
{
    [SerializeField] private InventoryPageManager inventoryPageManager;
    [SerializeField] private TechniquePageManager techniqueManager;
    [SerializeField] private SkillPageManager skillPageManager;
    public List<ItemData> itemUsed = new List<ItemData>();
    private UIItemSlotBase uiItem;
    public void UseItem(ulong playerClientId, UIItemSlotBase uiItem, int quantity = 1)
    {
        if (TryAddItemToPages(uiItem.inventoryItem))
        {
            this.uiItem = uiItem;
            if (uiItem.inventoryItem.data is PillData)
            {
                string instanceId = uiItem.inventoryItem.data.instanceId;
                if (InventoryCenterManager.Instance.RemoveData(uiItem.inventoryItem.data) == false)
                    return;
                UseItemPillServerRpc(playerClientId, instanceId);
            }
            else
            {

                UseItemServerRpc(playerClientId);
            }
        }
        else
        {
            Debug.Log("dont have use item");
        }
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void UseItemServerRpc(ulong playerClientId)
    {
        if (!IsServer) return;
        if (!NetworkManager.ConnectedClients.TryGetValue(playerClientId, out var client))
            return;
        var playerObj = client.PlayerObject;
        if (playerObj == null) return;
        var playerProfile = playerObj.GetComponent<PlayerProfile>();
        if (playerProfile == null) return;
        var skillPoint = playerProfile.GetSkillPoint();
        if (skillPoint <= 0)
        {
            SendMessegeToClientRpc(false, "Không đủ điểm kỹ năng", RpcTargetUtils.Single(playerClientId));
        }
        else
        {
            client.PlayerObject.GetComponent<PlayerProfile>().SetSkillPoint(-1);
            SendMessegeToClientRpc(true, "Đã sử dụng", RpcTargetUtils.Single(playerClientId));

        }

    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void UseItemPillServerRpc(ulong playerClientId, string instanceId)
    {
        if (!NetworkManager.ConnectedClients.TryGetValue(playerClientId, out var client))
            return;
        var playerObj = client.PlayerObject;
        if (playerObj == null) return;
        var playerViral = playerObj.GetComponent<PlayerVitals>();
        if (playerViral == null) return;

        var item = GameDataCenterManager.Instance.GetItemById(instanceId) as PillData;
        if (item == null) return;
        playerViral.Increase(VitalType.Health, Mathf.RoundToInt(item.health));
        playerViral.Increase(VitalType.Mana, Mathf.RoundToInt(item.mana));
        playerViral.Increase(VitalType.Spirit, Mathf.RoundToInt(item.spirit));

        SendUsePillDataToClientRpc(true, "Đã sử dụng", RpcTargetUtils.Single(playerClientId));
    }
    [ClientRpc]
    public void SendMessegeToClientRpc(bool success, string message, ClientRpcParams clientRpcParams)
    {
        TopNotificationUI.Instance.ShowNotification(message);
        if (success)
        {
            InventoryCenterManager.Instance.UseData(uiItem.inventoryItem.data);
        }
    }
    [ClientRpc]
    public void SendUsePillDataToClientRpc(bool success, string message, ClientRpcParams clientRpcParams)
    {
        TopNotificationUI.Instance.ShowNotification(message);
        if (success)
        {
            ;
        }
    }
    private bool TryAddItemToPages(InventoryItem inventoryItem)
    {
        if (inventoryItem == null)
            return false;
        if (inventoryItem.data is PillData)
            return true;

        if (techniqueManager.AddItemData(inventoryItem.data))
            return true;

        if (skillPageManager.AddItemData(inventoryItem.data))
            return true;

        return false;
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        inventoryPageManager = GetComponent<InventoryPageManager>();

    }
}