

using UnityEngine;

public class FPSLimiter : TGTHMonoBehaviour
{
    [Header("FPS Target")]
    [Range(15, 240)]
    public int targetFPS = 60;

    protected override void Awake()
    {
        base.Awake();
        SetFPS();
    }

    public void SetFPS()
    {
        Application.targetFrameRate = targetFPS;
    }
}