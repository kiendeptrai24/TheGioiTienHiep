

using System.Collections.Generic;
using UnityEngine;
using TGTH.Mobile;

public class SkillLoadGame : TGTHMonoBehaviour, ISaveable
{
    [SerializeField] private SkillPresenter skillPage;
    [SerializeField] private List<InventoryItem> listItemDatas;
    protected override void Awake()
    {
        skillPage?.SetInventoryData(listItemDatas);
    }
    public void LoadData(GameData _data)
    {
        foreach (var item in _data.itemDatas)
        {
            if (item is SkillData)
                listItemDatas.Add(new InventoryItem(item));
        }
        skillPage?.SetInventoryData(listItemDatas);
    }

    public void SaveGame(ref GameData _data)
    {

    }
}