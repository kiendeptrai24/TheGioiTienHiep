using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyItemPopup : BasePopup<ShopSetupData, QuantityPopupData>
{
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Button showInfoBtn;
    [SerializeField] private Button minusBtn;
    [SerializeField] private Button addBtn;
    [SerializeField] private Image itemIconImge;
    [SerializeField] private TextMeshProUGUI titleTxt;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private TextMeshProUGUI quantityTxt;
    [SerializeField] private TextMeshProUGUI priceTxt;
    protected Action onShowInfoBtn;
    private int quantity = 1;
    private int priceTotal = 0;
    private int price = 0;


    public override void Show()
    {
        base.Show();
        //PopupAnimation.ShowPopup(rect, group, 0.5f);
    }
    protected override void SetupButtons()
    {
        base.SetupButtons();
        cancelBtn.onClick.AddListener(OnCancelClicked);
        showInfoBtn.onClick.AddListener(OnShowInfoClicked);
        minusBtn.onClick.AddListener(() =>
        {
            if (quantity <= 1) return;
            quantity--;
            UpdateQuantity();
        });
        addBtn.onClick.AddListener(() =>
        {
            quantity++;
            UpdateQuantity();
        });

    }
    private void UpdateQuantity()
    {
        int totelPrice = quantity * price;
        quantityTxt.text = quantity.ToString();
        priceTxt.text = "Tổng: " + totelPrice + "K";
    }
    private void OnShowInfoClicked()
    {
        onShowInfoBtn?.Invoke();
        PopupManager.Instance.HidePopup(this);
    }
    public override void Hide()
    {
        //PopupAnimation.HidePopup(rect, group, 0.5f);
        base.Hide();
    }
    public void ShowPopup(ShopSetupData data, Action<QuantityPopupData> onConfirm = null, Action onCancel = null, Action onShowInfo = null)
    {
        this.onConfirm = onConfirm;
        this.onCancel = onCancel;
        this.onShowInfoBtn = onShowInfo;
        SetupPopupData(data);
        PopupManager.Instance.ShowPopup<BuyItemPopup>(this);
    }
    protected override QuantityPopupData GetResult()
    {
        int itemCount = 0;
        try
        {
            itemCount = int.Parse(quantityTxt.text);
        }
        catch (System.Exception ex)
        {
            Debug.Log("error parse quanlity: " + ex.ToString());
        }
        return new QuantityPopupData(itemCount, priceTotal);
    }
    protected override void LoadComponent()
    {
        base.LoadComponent();
    }

    protected override void SetupPopupData(ShopSetupData data)
    {
        string description = "";
        titleTxt.text = data.data.title + "\n";
        itemIconImge.sprite = data.data.itemIcon;
        description += "Loại: " + data.data.type + "\n";
        description += "Cảnh giới: " + EnumTranslator.ToVietnamese(data.data.realm) + "\n";
        description += "Phẩm: " + EnumTranslator.ToVietnamese(data.data.quanlity) + "\n";
        priceTxt.text = "Tổng: " + data.data.price + "K";
        descriptionTxt.text = description;
        price = data.data.price;
        UpdateQuantity();
    }
}