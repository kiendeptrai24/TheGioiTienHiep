using UnityEngine;

[System.Serializable]
public class SpawnSettings
{
    public int count = 10;
    public float spacing = 2f;
    public Vector3 originOffset = Vector3.zero;

    [Header("Random")]
    public int maxAttemptsPerPoint = 30;

    [Header("Line")]
    public Vector3 direction = Vector3.right;

    [Header("Circle")]
    public float radius = 5f;

    [Header("Grid")]
    public int columns = 5;
}