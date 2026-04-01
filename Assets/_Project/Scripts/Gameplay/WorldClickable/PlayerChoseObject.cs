using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerChoseObject : Singleton<PlayerChoseObject>
{
    private EntityClickable currentEntity;
    private UIFollow uIFollow;
    private NetworkObject playerNet;
    public event Action<EntityClickable> OnEntityClicked;
    public EntityClickable GetCurrentEntity()
    {
        return currentEntity;
    }
    public NetworkObject GetPlayerNet() => playerNet;
    protected override void Awake()
    {
        base.Awake();
        uIFollow = GetComponent<UIFollow>();
        PlayerNetManager.Instance.OnPlayerExiststed += OnPlayerExists;
    }

    private void OnPlayerExists(NetworkObject @object)
    {
        playerNet = @object;
    }

    public void SetupEntity(EntityClickable entity)
    {
        if (entity == playerNet) return;
        currentEntity = entity;
        uIFollow.SetTarget(currentEntity.transform);
        OnEntityClicked?.Invoke(entity);
    }
    public void RequestBattleSimulator()
    {
        if (currentEntity == null || playerNet == null)
        {
            Debug.Log("object null");
            return;
        }
        currentEntity.OnEntityClickedAccept(playerNet);
    }
    public void UnLink()
    {
        if (currentEntity == null) return;
        var mine = currentEntity.GetComponent<MineClickable>();
        if (mine == null) return;
        mine.UnLink(playerNet);
    }

}
