using System.Collections.Generic;
using Unity.Netcode;

public class MineRosterLinker : IRosterLinker
{
    private readonly PlayerBattleRoster battleRoster;

    public MineRosterLinker(
        PlayerBattleRoster roster)
    {
        battleRoster = roster;
    }

    public void Link(NetworkObject owner)
    {
        var roster =
            owner.GetComponent<PlayerBattleRoster>();

        if (roster != null)
        {
            roster.OnChampionPlayerChanged +=
                OnChampionPlayerChanged;
        }
    }

    public void UnLink(NetworkObject owner)
    {
        var roster =
            owner.GetComponent<PlayerBattleRoster>();

        if (roster != null)
        {
            roster.OnChampionPlayerChanged -=
                OnChampionPlayerChanged;
        }
    }
    private void OnChampionPlayerChanged(
        List<ItemData> list)
    {
        battleRoster.itemDatas = list;
    }
}