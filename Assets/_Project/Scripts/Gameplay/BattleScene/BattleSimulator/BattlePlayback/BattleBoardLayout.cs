using UnityEngine;

public static class BattleBoardLayout
{
    public const int TotalRows = 9;
    public const int MaxWidth = 5;

    public static int GetRowWidth(int row)
    {
        if (row < 0 || row >= TotalRows) return 0;
        return (row % 2 == 0) ? 4 : 5; // 4,5,4,5,4...
    }

    public static Vector3 CellToWorld(Vector2Int cell, float cellWidth, float rowHeight, Vector3 origin)
    {
        float rowOffsetX = GetRowWidth(cell.y) == 4 ? cellWidth * 0.5f : 0f;

        float x = origin.x + rowOffsetX + cell.x * cellWidth;
        float z = origin.z + cell.y * rowHeight;

        return new Vector3(x, origin.y, z);
    }
}