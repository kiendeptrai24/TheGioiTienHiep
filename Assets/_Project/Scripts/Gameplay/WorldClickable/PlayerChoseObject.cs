using System;
using FeatureToggles;
using Unity.Netcode;
using UnityEngine;

public class PlayerChoseObject : Singleton<PlayerChoseObject>
{
    private EntityClickable currentEntity;
    private NetworkObject playerNet;
    public event Action<EntityClickable> OnEntityClicked;
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
        currentEntity.OnEntityClickedAccept(playerNet);
    }
}
