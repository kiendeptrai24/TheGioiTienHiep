using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryDescription : MonoBehaviour
{
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private TMP_Text title;
    [SerializeField]
    private TMP_Text description;
    [SerializeField] private Button cancleBtn;
    [SerializeField] private Button acceptBtn;
    private InventoryItem inventoryItem;
    public void Awake()
    {
        ResetDescription();
    }
    public void SetInvenotoryItem(InventoryItem item)
    {
        inventoryItem = item;
    }
    public void ResetDescription()
    {
        itemImage.gameObject.SetActive(false);
        title.text = "";
        description.text = "";
        cancleBtn.gameObject.SetActive(false);
        acceptBtn.gameObject.SetActive(false);
    }

    public void SetDescription(Sprite sprite, string itemName,
        string itemDescription)
    {
        itemImage.gameObject.SetActive(true);
        itemImage.sprite = sprite;
        title.text = itemName;
        description.text = itemDescription;
    }
    public void SetButtonDescriptionInventory()
    {
        if(inventoryItem == null) return;

        if (inventoryItem.data is ItemEquitmentData)
        {
            cancleBtn.gameObject.SetActive(true);
            acceptBtn.gameObject.SetActive(true);
            acceptBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
        }
        else if (inventoryItem.data is ItemData)
        {
            cancleBtn.gameObject.SetActive(true);
            acceptBtn.gameObject.SetActive(true);
            acceptBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Use";
        }
    }
    public void SetButtonDescriptionEquipment()
    {
        if(inventoryItem == null || inventoryItem.data is not ItemEquitmentData) return;
        
        cancleBtn.gameObject.SetActive(true);
        acceptBtn.gameObject.SetActive(true);
        acceptBtn.GetComponentInChildren<TextMeshProUGUI>().text = "UnEquip";
    }
}