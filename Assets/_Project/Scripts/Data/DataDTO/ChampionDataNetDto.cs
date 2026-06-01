

using System.Collections.Generic;
using UnityEngine;

public class ChampionDataNetDto : BaseDataNetDto
{
    public bool isCharacter;
    public string raceId;
    public string essenceId;
    public string realmId;
    public int physicalDamagePoint;
    public int magicalDamagePoint;
    public int spiritDamagePoint;
    public int physicalDefensePoint;
    public int magicalDefensePoint;
    public int spiritDefensePoint;
    public int healthPoint;
    public int manaPoint;
    public int spiritPoint;
    public int moveSpeedPoint;
    public int spititRangePoint;
    public Vector2Int championIndex = new Vector2Int();
    public List<string> equipmentIds = new();
    public List<string> skillIds = new();
    public List<string> techniqueIds = new();
}