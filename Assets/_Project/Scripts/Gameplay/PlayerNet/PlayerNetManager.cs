

using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetManager : Singleton<PlayerNetManager>, ISaveable
{
    [SerializeField] private NetworkObject playerObject;
    private PlayerController player;
    public event Action<NetworkObject> OnPlayerExiststed;
    [SerializeField] private Vector3 position = new Vector3(500, 0, 440);
    private Quaternion rotation = Quaternion.identity;
    public Vector3 GetPos() => position;
    public Quaternion GetRot() => rotation;
    public event Action OnDataLoaded;
    public bool IsDataLoaded = false;

    public void LoadData(GameData _data)
    {
        position = _data.position;
        rotation = _data.rotation;
        IsDataLoaded = true;
        OnDataLoaded?.Invoke();
    }
    public void SaveGame(ref GameData _data)
    {
        _data.position = position;
        _data.rotation = rotation;
    }

    public void SetPlayerObject(NetworkObject _playerObject)
    {
        playerObject = _playerObject;
        player = playerObject.GetComponent<PlayerController>();
        SetPlayerProfile();
        var itemData = InventoryCenterManager.Instance.listItemDatasChampion;
        ItemPrefabDatabase.Instance.OnListItemDatasChampionChanged(itemData);
        OnPlayerExiststed?.Invoke(playerObject);

    }
    private void Update()
    {
        if (player == null) return;

        if (player.moveable.IsMoving())
        {
            position = player.gameObject.transform.position;
            rotation = player.gameObject.transform.rotation;
        }
    }
    public NetworkObject GetPlayerObj() => playerObject;

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