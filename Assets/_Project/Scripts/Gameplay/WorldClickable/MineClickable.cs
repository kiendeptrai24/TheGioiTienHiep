using Unity.Netcode;

public class MineClickable : EntityClickable
{
    private SpiritStoneMine mine;
    public override void OnNetworkSpawn()
    {
        mine = GetComponent<SpiritStoneMine>();
        entityWorldType = EntityWorldType.Mine;
    }
    public bool IsObjectOwner(NetworkObject owner)
    {
        return mine.IsObjectOwner(owner);
    }
    public void UnLink(NetworkObject owner)
    {
        mine.UnLink(owner);
    }
    public override void OnEntityClickedAccept(NetworkObject network)
    {
        mine.SetOwner(network);
    }
}
