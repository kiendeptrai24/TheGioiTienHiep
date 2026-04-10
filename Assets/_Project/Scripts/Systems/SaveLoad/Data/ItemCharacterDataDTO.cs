using System;
using System.Collections.Generic;

[Serializable]
public class ItemCharacterDataDTO
{
    public List<HeroData> inventoryItems = new List<HeroData>();
    public List<string> characterNames = new List<string>();
    public List<string> characterIds = new List<string>();

}
