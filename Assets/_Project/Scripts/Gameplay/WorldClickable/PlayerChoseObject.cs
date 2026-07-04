using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerChoseObject : Singleton<PlayerChoseObject>
{
    [SerializeField] private EntityClickable currentEntity;
    private UIFollow uIFollow;
    private NetworkObject playerNet;
    public event Action<EntityClickable> OnEntityClicked;
    private SafeZoneManager safeZoneManager;
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
    override protected void Start()
    {
        base.Start();
        safeZoneManager = SafeZoneManager.Instance;
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
        if (safeZoneManager != null)
        {
            if (safeZoneManager.IsInside(currentEntity.transform.position) || safeZoneManager.IsInside(playerNet.transform.position))
            {
                TopNotificationUI.Instance.ShowNotification("bạn hoặc đối phương đang ở trong khu vực an toàn, không thể chiến đấu");
                return;
            }
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
