using System;
using Unity.Netcode;
using UnityEngine;

public class ShopRequester : SingletonNetwork<ShopRequester>
{
    public event Action<bool, string> OnBuyResult;
    public void RequestBuy(string instanceId, Action<bool, string> success = null)
    {
        OnBuyResult = success;
        BuyServerRpc(instanceId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void BuyServerRpc(string instanceId, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        string message = "";
        var playerObject =
            NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        if(string.IsNullOrEmpty(instanceId))
        {
            message = "Sản phẩm không tồn tại";
            BuyResultClientRpc(false, message, clientId);
            return;
        }
        var itemData = GameDataCenterManager.Instance.GetShopItemById(instanceId);
        if (itemData == null)
        {
            message = "Sản phẩm không tồn tại";
            BuyResultClientRpc(false, message, clientId);
            return;
        }
        if (!playerObject.TryGetComponent<ResourceStorage>(out var storage))
        {
            message = "Không tìm thấy tài khoản";
            BuyResultClientRpc(false, message, clientId);
            return;
        }

        if (!storage.HasEnough(itemData.itemPrice))
        {
            message = "Không đủ linh thạch";
            BuyResultClientRpc(false, message, clientId);
            return;
        }
        message = "Mua thành công";
        storage.MinusCost(itemData.itemPrice);
        BuyResultClientRpc(true, message, clientId);
    }

    [ClientRpc]
    private void BuyResultClientRpc(bool success, string message, ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId)
            return;
        OnBuyResult?.Invoke(success, message);
    }
}
