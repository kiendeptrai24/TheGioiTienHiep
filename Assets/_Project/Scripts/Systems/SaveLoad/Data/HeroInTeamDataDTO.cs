using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HeroInTeamDataDTO : HeroDataDTO
{
    public List<Vector2Int> championsIndex = new List<Vector2Int>();
}

[Serializable]
public class HeroDataDTO
{
    public List<HeroData> inventoryItems = new List<HeroData>();
}