using System.Collections.Generic;

public interface IUsable
{
    void UseItem(ulong playerId, UIItemSlotBase uiItem, int quantity = 1);
}
