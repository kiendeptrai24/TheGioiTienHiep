using Unity.Netcode;

public static class RpcTargetUtils
{
    public static ClientRpcParams Single(ulong clientId)
    {
        return new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { clientId }
            }
        };
    }

    public static ClientRpcParams Multiple(params ulong[] clientIds)
    {
        return new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = clientIds
            }
        };
    }

    public static ClientRpcParams All()
    {
        return default;
    }
}