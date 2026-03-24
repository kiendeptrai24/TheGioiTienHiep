using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerChoseObject : Singleton<PlayerChoseObject>
{
    private EntityClickable currentEntity;
    private NetworkObject playerNet;
    public event Action<EntityClickable> OnEntityClicked;
    public EntityClickable GetCurrentEntity()
    {
        return currentEntity;
    }
    protected override void Awake()
    {
        base.Awake();
        PlayerNetManager.Instance.OnPlayerExiststed += OnPlayerExists;
    }

    private void OnPlayerExists(NetworkObject @object)
    {
        playerNet = @object;
    }

    public void SetupEntity(EntityClickable entity)
    {
        currentEntity = entity;
        transform.position = entity.transform.position;
        OnEntityClicked?.Invoke(entity);
    }
    public void RequestBattleSimulator()
    {
        if (currentEntity == null || playerNet == null)
        {
            Debug.Log("object null");
            return;
        }
        if (CheckIsOwner()) return;

        currentEntity.OnEntityClickedAccept(playerNet);
    }
    public void UnLink()
    {
        if (currentEntity == null) return;
        var mine = currentEntity.GetComponent<MineClickable>();
        if (mine == null) return;
        mine.UnLink(playerNet);
    }
    public bool CheckIsOwner()
    {
        var mine = currentEntity.GetComponent<MineClickable>();
        if (mine == null) return false;

        return mine.IsObjectOwner(playerNet);
    }
}
