using System;
using UnityEngine;

public abstract class ActiveStateNotifier : TGTHMonoBehaviour
{
    public event Action OnActive;
    public event Action OnUnActive;

    // Cho lớp con hoặc base trigger event một cách an toàn
    protected void RaiseActive()
    {
        OnActive?.Invoke();
    }
    protected void RaiseUnActive()
    {
        OnUnActive?.Invoke();
    }
}