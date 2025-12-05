using UnityEngine;

public abstract class TGTHBehaviour : MonoBehaviour {
    protected abstract void Awake();
    protected abstract void Start();
    protected abstract void LoadComponent();
    private void Reset() => LoadComponent();
}