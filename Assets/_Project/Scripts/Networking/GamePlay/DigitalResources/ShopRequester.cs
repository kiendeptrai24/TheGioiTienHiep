using System;
using Unity.Netcode;
using UnityEngine;

public class ShopRequester : SingletonNetwork<ShopRequester>
{
    public event Action<bool, string> OnBuyResult;
    public void RequestBuy(int cost, Action<bool, string> success = null)
    {
        OnBuyResult = success;
        BuyServerRpc(cost);
    }

    [ServerRpc(RequireOwnership = false)]
    private void BuyServerRpc(int cost, ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        var playerObject =
            NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

        if (!playerObject.TryGetComponent<ResourceStorage>(out var storage))
            return;

        if (!storage.HasEnough(cost))
        {
            BuyResultClientRpc(false, clientId);
            return;
        }

        storage.Remove(cost);

        BuyResultClientRpc(true, clientId);
    }

    [ClientRpc]
    private void BuyResultClientRpc(bool success, ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId != targetClientId)
            return;
        Debug.Log("BuyResultClientRpc: " + success);
        OnBuyResult?.Invoke(success, success ? "Mua thành công" : "Không đủ linh thạch");
    }
}
