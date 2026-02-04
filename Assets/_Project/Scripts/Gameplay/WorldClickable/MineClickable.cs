using Unity.Netcode;

public class MineClickable : EntityClickable
{
    private SpiritStoneMine mine;
    public override void OnNetworkSpawn()
    {
        mine = GetComponent<SpiritStoneMine>();
    }
    public override void OnEntityClickedAccept(NetworkObject network)
    {
        mine.SetOwner(network);
    }
}
