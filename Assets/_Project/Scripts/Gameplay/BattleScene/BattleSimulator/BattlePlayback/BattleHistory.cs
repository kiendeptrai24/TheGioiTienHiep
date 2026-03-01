

using System;
using System.Collections.Generic;
[Serializable]
public class BattleHistory 
{
    public string name;
    public string namePlayer;
    public string nameEnemy;
    public string winner;
    public float duration;
    public DateTime dateTime;
    public List<BattleEvent> battleEvents; 
}