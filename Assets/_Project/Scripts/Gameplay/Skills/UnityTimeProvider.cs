public sealed class UnityTimeProvider : ITimeProvider
{
    public float Now => UnityEngine.Time.time;
}