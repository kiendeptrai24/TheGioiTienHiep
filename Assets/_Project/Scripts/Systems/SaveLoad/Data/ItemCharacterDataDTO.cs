using System;
using System.Collections.Generic;

[Serializable]
public class ItemCharacterDataDTO : ItemDataDTO
{
    public List<string> characterNames = new List<string>();
    public List<string> characterIds = new List<string>();

}
