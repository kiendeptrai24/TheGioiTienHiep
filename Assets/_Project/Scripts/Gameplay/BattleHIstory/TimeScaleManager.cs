using UnityEngine;

public class TimeScaleManager
{
    public static void SetUnityTimeScale(float timeScale) => Time.timeScale = timeScale;
    public static float GetUnityTimeScale() => Time.timeScale;
}
