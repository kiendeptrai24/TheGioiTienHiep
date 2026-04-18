namespace PlayFab.Multiplayer.InteropWrapper
{
    public enum PFLobbyServerConnectionStatus : uint
    {
        NotConnected = Interop.PFLobbyServerConnectionStatus.NotConnected,
        Connected = Interop.PFLobbyServerConnectionStatus.Connected
    }
}
