using System;
using System.Collections.Generic;
using FeatureToggles;
using UnityEngine;
using UnityEngine.UI;

public class FeatureGateActiveBinder : TGTHMonoBehaviour
{
    [SerializeField] private List<FeatureId> dependentGates = new();
    private FeatureManager _mgr;
    [SerializeField] private List<ActiveStateNotifier> blockerObjects;
    private const string BLOCK_SRC = "ActionLockBtn_Blocker";
    protected override void Awake()
    {
        base.Awake();
        _mgr = FeatureManager.Instance;
        foreach (var blockerObject in blockerObjects)
        {
            blockerObject.OnActive += LockGate;
            blockerObject.OnUnActive += UnlockGate;
        }
    }

    private void UnlockGate()
    {
        foreach (var dependentGate in dependentGates)
        {
            _mgr.RemoveBlocker(dependentGate, BLOCK_SRC);
        }
    }

    private void LockGate()
    {
        foreach (var dependentGate in dependentGates)
        {
            _mgr.AddBlocker(dependentGate, BLOCK_SRC);
        }
    }
}
