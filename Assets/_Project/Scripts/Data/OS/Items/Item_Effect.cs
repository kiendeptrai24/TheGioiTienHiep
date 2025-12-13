using UnityEngine;


[CreateAssetMenu(fileName = "NewItemEffect", menuName = "RPG/Items/Item Effect")]
public class Item_Effect : ScriptableObject
{
    [TextArea]
    public string effectDescription;
    public virtual void ExecuteEffect(Transform _enemyPosition)
    {
        Debug.Log("Effect execute");
    }
}