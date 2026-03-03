using System;
using UnityEngine;

public class ActiveGameObjectStateNotifier : ActiveStateNotifier
{
    private void OnEnable()
    {
        RaiseActive();
    }

    private void OnDisable()
    {
        RaiseUnActive();
    }
}