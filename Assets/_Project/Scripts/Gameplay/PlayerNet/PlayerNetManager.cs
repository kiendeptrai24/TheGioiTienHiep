

using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetManager : Singleton<PlayerNetManager>
{
    [SerializeField] private NetworkObject playerObject;
    public event Action<NetworkObject> OnPlayerExiststed;
    public void SetPlayerObject(NetworkObject _playerObject)
    {
        playerObject = _playerObject;
        SetPlayerProfile();
        var itemData = InventoryCenterManager.Instance.listItemDatasChampion;
        ItemPrefabDatabase.Instance.OnListItemDatasChampionChanged(itemData);
        OnPlayerExiststed?.Invoke(playerObject);

    }
    public void SetPlayerProfile()
    {
        var storage = playerObject.GetComponent<ResourceStorage>();
        ProfileManager.Instance.BindResource(storage);
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }
}