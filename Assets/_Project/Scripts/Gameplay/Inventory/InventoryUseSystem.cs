

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
            UseItemServerRpc(playerClientId);
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
            SendMessegeToClientRpc(false, "Không đủ điểm kỹ năng",
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { playerObj.OwnerClientId } }
            });
        }
        else
        {
            client.PlayerObject.GetComponent<PlayerProfile>().SetSkillPoint(-1);
            SendMessegeToClientRpc(true, "Đã sử dụng",
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new[] { playerObj.OwnerClientId } }
            });

        }

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
    private bool TryAddItemToPages(InventoryItem inventoryItem)
    {
        if (inventoryItem == null)
            return false;

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