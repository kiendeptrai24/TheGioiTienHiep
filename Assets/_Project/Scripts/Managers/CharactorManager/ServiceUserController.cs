using System.Collections;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

public class ServiceUserController : TGTHNetworkBehaviour
{
    [SerializeField] private PlayerPrefabSelector prefabSelector;
    private PlayerNetManager playerNetManager;

    public override void OnNetworkSpawn()
    {
        prefabSelector = PlayerPrefabSelector.Instance;
        if (!IsOwner) return;
        playerNetManager = PlayerNetManager.Instance;
        playerNetManager.OnDataLoaded += OnDataLoaded;
        if (playerNetManager.IsDataLoaded)
            OnDataLoaded();
    }

    private void OnDataLoaded()
    {
        if (IsOwner)
        {
            StartCoroutine(RequestSpawnPlayer());
        }
    }

    private IEnumerator RequestSpawnPlayer()
    {
        yield return new WaitUntil(() => playerNetManager.IsDataLoaded);

        var itemData = prefabSelector.GetItemData();

        if (itemData == null)
        {
            Debug.LogError("No prefab selected!");
            yield break;
        }
        SpawnPlayerServerRpc(itemData.itemId, playerNetManager.GetPos(), playerNetManager.GetRot());
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void SpawnPlayerServerRpc(string id, Vector3 position, Quaternion rotation, RpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        var prefab = GetPrefabById(id);
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {id}");
            return;
        }
        var networkObject = Instantiate(prefab, position, rotation).GetComponent<NetworkObject>();
        Debug.Log(networkObject.gameObject.name);
        networkObject.SpawnAsPlayerObject(clientId);
    }

    private GameObject GetPrefabById(string id)
    {
        var selector = prefabSelector.GetSelectedPrefab(id);
        return selector;
    }
}