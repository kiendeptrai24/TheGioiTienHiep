
using Unity.Netcode;

public interface IRosterLinker
{
    public void Link(NetworkObject owner);
    public void UnLink(NetworkObject owner);
}