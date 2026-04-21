

using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
public class UIItemResourceType : TGTHMonoBehaviour, IPointerClickHandler
{
    public ResourceSourceType resourceSourceType;
    [SerializeField] private Image focusImage;
    #region Events
    public event Action<UIItemResourceType> OnItemClicked;
    #endregion
    public void OnPointerClick(PointerEventData eventData)
    {
        OnItemClicked?.Invoke(this);
    }
    public void FocusItem()
    {
        focusImage.gameObject.SetActive(true);
    }
    public void UnFocusItem()
    {
        focusImage.gameObject.SetActive(false);
    }
}