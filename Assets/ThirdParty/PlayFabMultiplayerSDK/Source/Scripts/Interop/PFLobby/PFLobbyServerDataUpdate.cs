namespace PlayFab.Multiplayer.Interop
{
    public unsafe partial struct PFLobbyServerDataUpdate
    {
        [NativeTypeName("const PFEntityKey *")]
        public PFEntityKey* newServer;

        [NativeTypeName("uint32_t")]
        public uint serverPropertyCount;

        [NativeTypeName("const char *const *")]
        public sbyte** serverPropertyKeys;

        [NativeTypeName("const char *const *")]
        public sbyte** serverPropertyValues;
    }
}
