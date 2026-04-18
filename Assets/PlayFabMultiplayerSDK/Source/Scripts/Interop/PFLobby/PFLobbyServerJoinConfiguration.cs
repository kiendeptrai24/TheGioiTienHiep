namespace PlayFab.Multiplayer.Interop
{
    public unsafe partial struct PFLobbyServerJoinConfiguration
    {
        [NativeTypeName("uint32_t")]
        public uint serverPropertyCount;

        [NativeTypeName("const char *const *")]
        public sbyte** serverPropertyKeys;

        [NativeTypeName("const char *const *")]
        public sbyte** serverPropertyValues;
    }
}
