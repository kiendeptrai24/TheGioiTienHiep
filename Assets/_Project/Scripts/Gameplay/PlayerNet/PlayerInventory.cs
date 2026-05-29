

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Netcode;
using UnityEngine;

public class PlayerInventory : TGTHNetworkBehaviour
{
    [SerializeField] private List<ItemData> items;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;
        InventoryCenterManager.Instance.OnItemDataChanged += OnItemDataChanged;
        InventoryCenterManager.Instance.OnLoadDataSuccessed += OnLoadDataSuccessed;
        OnLoadDataSuccessed();
    }
    private void OnLoadDataSuccessed()
    {
        OnItemDataChanged(InventoryCenterManager.Instance.GetItemData());
    }

    private void OnItemDataChanged(List<ItemData> list)
    {
        var items = list.FindAll(x => x is PillData);
        var jsonData = JsonConvert.SerializeObject(items);
        UpdateInventoryToServerRpc(jsonData);
    }
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    public void UpdateInventoryToServerRpc(string data)
    {
        var pasteData = JsonConvert.DeserializeObject<List<ItemData>>(data);
        items = pasteData;
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        if (!IsOwner) return;
        if (InventoryCenterManager.Instance != null)
        {
            InventoryCenterManager.Instance.OnItemDataChanged -= OnItemDataChanged;
            InventoryCenterManager.Instance.OnLoadDataSuccessed -= OnLoadDataSuccessed;
        }
    }
    public List<ItemData> GetAllItems() => items;
    public List<ItemAmount> GetItemRequirments()
    {
        List<ItemAmount> itemRequirments = new();
        foreach (var item in items)
        {
            if (item.instanceId == "ID_DANDUOC_TRUCCODAN_00001")
                itemRequirments.Add(new ItemAmount(item.instanceId, item.currentstack));
        }
        return itemRequirments;
    }

    public void SetItems(List<ItemAmount> itemAmounts)
    {
        var json = JsonConvert.SerializeObject(itemAmounts);
        SetItemAmountClientRpc(json);
    }
    [ClientRpc]
    public void SetItemAmountClientRpc(string data)
    {
        if (!IsOwner) return;
        var pasteData = JsonConvert.DeserializeObject<List<ItemAmount>>(data);
        foreach (var item in pasteData)
        {
            InventoryCenterManager.Instance.SetItem(item.itemId, item.amount);
        }
    }
}