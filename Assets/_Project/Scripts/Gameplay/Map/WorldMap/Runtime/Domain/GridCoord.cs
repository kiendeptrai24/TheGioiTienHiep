// Assets/_Game/WorldMap/Runtime/Domain/GridCoord.cs
namespace WorldMap.Domain
{
    public readonly struct GridCoord
    {
        public readonly int x;
        public readonly int z;

        public GridCoord(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public override string ToString() => $"({x},{z})";
    }
}
