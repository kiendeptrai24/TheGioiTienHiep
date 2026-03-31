using UnityEngine;

public class MinimapWorldIcon : TGTHMonoBehaviour
{
    private ItemMapWorld item;
    protected override void Awake()
    {
        base.Awake();
        LoadComponent();
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
        item = GetComponentInParent<ItemMapWorld>();
    }
}
