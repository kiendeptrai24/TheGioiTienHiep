using Unity.VisualScripting;
using UnityEngine;

public class PlayerFogofWar : TGTHNetworkBehaviour
{
    [SerializeField] private Light lightprefab;
    private Light playerLight;
    protected override void Awake()
    {
        base.Awake();
        playerLight = Instantiate(lightprefab).GetComponent<Light>();
        playerLight.transform.SetParent(transform);
        playerLight.transform.localPosition = new Vector3(0, 10, 0);
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        StatsData statsData = GetComponent<StatsData>();
        int spiritRange = statsData.SpiritRange;
        if (spiritRange <= 10) return;
        int persent = (10 - spiritRange) / 10 + 1;
        playerLight.intensity = 200 * persent;
        playerLight.range = 20 * persent;
        playerLight.transform.localPosition = new Vector3(0, 10 * persent, 0);
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
