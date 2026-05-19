using Unity.Netcode;
using UnityEngine;

public static class TimeUtils
{
    public static double GetServerTime()
    {
        if (NetworkManager.Singleton == null)
            return 0;
        return NetworkManager.Singleton.ServerTime.Time;
    }
    public static long DateTimeOffset() => System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public static double GetLocalTime()
    {
        if (NetworkManager.Singleton == null)
            return 0;
        return NetworkManager.Singleton.LocalTime.Time;
    }

    public static float GetUnityTime()
    {
        return Time.time;
    }
}