

using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using System;
public class UIItemResourse : TGTHMonoBehaviour, IPointerClickHandler
{
    public ItemData itemData;
    [SerializeField] private TextMeshProUGUI nameTxt;
    #region Events
    public event Action<UIItemResourse> OnItemClicked;
    #endregion
    public void OnPointerClick(PointerEventData eventData)
    {
        OnItemClicked?.Invoke(this);
    }
    public void SetData(ItemData data)
    {
        itemData = data;
        nameTxt.text = data.itemName;
    }
    public void ResetData()
    {
        itemData = null;
        nameTxt.text = "";
    }
}