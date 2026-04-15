using System;
using Unity.Netcode;
using UnityEngine;

public class ShopRequester : SingletonNetwork<ShopRequester>
{
    public event Action<bool, string> OnBuyResult;
    public void RequestBuy(ulong cost, Action<bool, string> success = null)
    {
        OnBuyResult = success;
        BuyServerRpc(cost);
    }

    [ServerRpc(RequireOwnership = false)]
    private void BuyServerRpc(ulong cost, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;
        string message = "";
        var playerObject =
            NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

        if (!playerObject.TryGetComponent<ResourceStorage>(out var storage))
        {
            message = "Không tìm thấy tài khoản";
            BuyResultClientRpc(false, message, clientId);
            return;
        }

        if (!storage.HasEnough(cost))
        {
            message = "Không đủ linh thạch";
            BuyResultClientRpc(false, message, clientId);
            return;
        }
        message = "Mua thành công";
        storage.MinusCost(cost);
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
