public struct XorShift32
{
    private uint _state;
    public XorShift32(uint seed) => _state = seed == 0 ? 2463534242u : seed;

    public uint NextU()
    {
        uint x = _state;
        x ^= x << 13;
        x ^= x >> 17;
        x ^= x << 5;
        _state = x;
        return x;
    }

    public float Next01() => (NextU() & 0x00FFFFFF) / 16777216f; // [0..1)
}
