using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class GameDataCenter
{
    public string version;
    [JsonIgnore]
    public List<ItemData> allItems;
    public List<EquipmentData> equipmentItems;
    public List<SkillData> skillDatas;
    public List<TechniqueData> techniqueDatas;
    public List<RaceData> raceItems;
    public List<EssenceData> essenceItems;
    public List<RealmData> realmDatas;
    public List<HeroData> championItems;
    public List<HeroData> characterDatas;
    public List<ItemData> shopItems;
    public List<SpiritStoneMineData> spiritStoneMineItems;
    public List<DemonBeastData> demonBeastItems;

    public GameDataCenter()
    {
        version = "1.0";
        allItems = new List<ItemData>();
        equipmentItems = new List<EquipmentData>();
        skillDatas = new List<SkillData>();
        techniqueDatas = new List<TechniqueData>();
        raceItems = new List<RaceData>();
        essenceItems = new List<EssenceData>();
        realmDatas = new List<RealmData>();
        championItems = new List<HeroData>();
        characterDatas = new List<HeroData>();
        shopItems = new List<ItemData>();
        spiritStoneMineItems = new List<SpiritStoneMineData>();
        demonBeastItems = new List<DemonBeastData>();
    }
}