using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ServiceUserController : TGTHNetworkBehaviour
{
    [SerializeField] private PlayerPrefabSelector prefabSelector;
    private PlayerNetManager playerNetManager;
    private bool isSpawned;
    public override void OnNetworkSpawn()
    {
        prefabSelector = PlayerPrefabSelector.Instance;
        if (IsServer)
        {
            if (prefabSelector == null)
            {
                Debug.LogError("PlayerPrefabSelector instance is not assigned.");
                return;
            }
            else
            {
                Debug.Log("PlayerPrefabSelector instance found.");
            }
        }
        if (!IsOwner) return;
        playerNetManager = PlayerNetManager.Instance;
        isSpawned = false;
        playerNetManager.OnDataLoaded += OnDataLoaded;
        if (playerNetManager.IsDataLoaded)
            OnDataLoaded();
    }

    private void OnDataLoaded()
    {
        if (IsOwner && !isSpawned)
        {
            isSpawned = true;
            StartCoroutine(RequestSpawnPlayer());
        }
    }
    override public void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        playerNetManager.OnDataLoaded -= OnDataLoaded;
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
        SpawnPlayerServerRpc(itemData.instanceId, playerNetManager.GetPos(), playerNetManager.GetRot());
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    private void SpawnPlayerServerRpc(string id, Vector3 position, Quaternion rotation, RpcParams rpcParams = default)
    {
        if (!IsServer) return;
        position.y = 0;
        if (position == Vector3.zero)
        {
            position = new Vector3(Random.Range(490f, 500f), 0, Random.Range(450f, 440f));
        }
        ulong clientId = rpcParams.Receive.SenderClientId;

        var prefab = GetPrefabById(id);
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {id}");
            return;
        }
        var networkObject = Instantiate(prefab, position, rotation).GetComponent<NetworkObject>();
        networkObject.SpawnAsPlayerObject(clientId);
    }

    private GameObject GetPrefabById(string id)
    {
        var selector = prefabSelector.GetSelectedPrefab(id);
        return selector;
    }
}