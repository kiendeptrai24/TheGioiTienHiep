

using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
public class UIItemResourse : TGTHMonoBehaviour, IPointerClickHandler
{
    public ItemData itemData;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private Image focus;
    #region Events
    public event Action<UIItemResourse> OnItemClicked;
    #endregion
    protected override void Awake()
    {
        base.Awake();
        UnSelect();
    }
    protected override void Start()
    {
        base.Start();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        OnItemClicked?.Invoke(this);
    }
    public void Select()
    {
        if (focus == null) return;
        focus.gameObject.SetActive(true);
    }
    public void UnSelect()
    {
        if (focus == null) return;
        focus.gameObject.SetActive(false);
    }
    public void SetData(ItemData data)
    {
        if (data == null) return;
        itemData = data;
        nameTxt.text = data.itemName;
    }
    public void ResetData()
    {
        itemData = null;
        nameTxt.text = "";
    }
}