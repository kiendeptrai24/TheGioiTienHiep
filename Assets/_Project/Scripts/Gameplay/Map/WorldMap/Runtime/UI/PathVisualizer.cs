using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathVisualizer : Singleton<PathVisualizer>
{
    [SerializeField] private bool isVisible = true;

    private LineRenderer line;

    protected override void Awake()
    {
        line = GetComponent<LineRenderer>();

        line.positionCount = 0;
        line.widthMultiplier = 0.2f;
        line.useWorldSpace = true;

        ApplyVisible();
    }
    public void Draw(List<Vector3> corners)
    {
        if (!isVisible)
            return;

        if (corners == null || corners.Count == 0)
        {
            line.positionCount = 0;
            return;
        }

        line.positionCount = corners.Count;

        for (int i = 0; i < corners.Count; i++)
        {
            Vector3 pos = corners[i];
            pos.y += 0.1f;

            line.SetPosition(i, pos);
        }
    }

    public void Clear()
    {
        line.positionCount = 0;
    }

    public void SetVisible(bool value)
    {
        isVisible = value;

        ApplyVisible();

        if (!isVisible)
        {
            Clear();
        }
    }

    private void ApplyVisible()
    {
        line.enabled = isVisible;
    }
}