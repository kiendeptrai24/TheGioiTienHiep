namespace PlayFab.Multiplayer.Interop
{
    [NativeTypeName("struct PFLobbyServerPostUpdateAsServerCompletedStateChange : PFLobbyStateChange")]
    public unsafe partial struct PFLobbyServerPostUpdateAsServerCompletedStateChange
    {
        public PFLobbyStateChange __AnonymousBase_1;

        public int result;

        [NativeTypeName("PFLobbyHandle")]
        public PFLobby* lobby;

        public void* asyncContext;
    }
}
