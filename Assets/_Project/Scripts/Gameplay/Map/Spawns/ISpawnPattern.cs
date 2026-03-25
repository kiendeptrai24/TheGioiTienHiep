using System.Collections.Generic;
using UnityEngine;

public interface ISpawnPattern
{
    List<Vector3> GeneratePoints(ISpawnArea area, SpawnSettings settings);
}