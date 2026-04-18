namespace PlayFab.Multiplayer.Interop
{
    [NativeTypeName("struct PFLobbyServerLeaveLobbyAsServerCompletedStateChange : PFLobbyStateChange")]
    public unsafe partial struct PFLobbyServerLeaveLobbyAsServerCompletedStateChange
    {
        public PFLobbyStateChange __AnonymousBase_1;

        [NativeTypeName("PFLobbyHandle")]
        public PFLobby* lobby;

        public void* asyncContext;
    }
}
