using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HeroDataDTO : ItemDataDTO
{
    public List<Vector2Int> championsIndex = new List<Vector2Int>();
}