using System;
using UnityEngine;

public class PlayerFogofWar : TGTHNetworkBehaviour
{
    [SerializeField] private Light lightprefab;
    private Light playerLight;
    private StatsData statsData;
    protected override void Awake()
    {
        base.Awake();
        statsData = GetComponent<StatsData>();
        statsData.OnStatReady += SetSpiritRange;
    }

    private void SetSpiritRange(StatsData data)
    {
        int spiritRange = statsData.SpiritRange;
        if (spiritRange <= 10) return;
        int persent = (10 - spiritRange) / 10 + 1;
        if (playerLight == null) return;
        playerLight.intensity = 200 * persent;
        playerLight.range = 20 * persent;
        playerLight.transform.localPosition = new Vector3(0, 10 * persent, 0);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) return;

        playerLight = Instantiate(lightprefab).GetComponent<Light>();
        playerLight.transform.SetParent(transform);
        playerLight.transform.localPosition = new Vector3(0, 10, 0);
        if (statsData != null && statsData.IsReady)
            SetSpiritRange(statsData);

    }
    protected override void Start()
    {
        base.Start();


    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 10);
    }
}
