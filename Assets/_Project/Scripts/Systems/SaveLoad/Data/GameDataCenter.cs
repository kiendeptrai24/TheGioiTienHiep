using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class GameDataCenter
{
    public string version;
    [JsonIgnore]
    public List<ItemData> allItems;
    public List<EquitmentData> equipmentItems;
    public List<SkillData> skillItems;
    public List<TechniqueData> techniqueDatasItems;

    public List<RaceData> raceItems;
    public List<EssenceData> essenceItems;
    public List<RealmData> realmItems;
    public List<HeroData> championItems;
    public List<HeroData> characterDatas;
    public List<ItemShop> shopItems;

    public GameDataCenter()
    {
        version = "1.0";
        allItems = new List<ItemData>();
        equipmentItems = new List<EquitmentData>();
        skillItems = new List<SkillData>();
        techniqueDatasItems = new List<TechniqueData>();
        raceItems = new List<RaceData>();
        essenceItems = new List<EssenceData>();
        realmItems = new List<RealmData>();
        championItems = new List<HeroData>();
        characterDatas = new List<HeroData>();
        shopItems = new List<ItemShop>();
    }
}