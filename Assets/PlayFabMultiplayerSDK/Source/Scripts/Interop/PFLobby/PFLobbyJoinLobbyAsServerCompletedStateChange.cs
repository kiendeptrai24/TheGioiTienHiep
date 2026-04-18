namespace PlayFab.Multiplayer.Interop
{
    [NativeTypeName("struct PFLobbyJoinLobbyAsServerCompletedStateChange : PFLobbyStateChange")]
    public unsafe partial struct PFLobbyJoinLobbyAsServerCompletedStateChange
    {
        public PFLobbyStateChange __AnonymousBase_1;

        public int result;

        public PFEntityKey newServer;

        public void* asyncContext;

        [NativeTypeName("PFLobbyHandle")]
        public PFLobby* lobby;

    }
}
