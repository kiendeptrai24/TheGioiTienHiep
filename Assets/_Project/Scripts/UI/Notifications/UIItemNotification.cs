using UnityEngine;
using UnityEngine.EventSystems;

// 🛠️ Bỏ IPointerExitHandler đi để khi di chuột ra ngoài vẫn không bị mất trạng thái hold
public class UIItemNotification : TGTHMonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isHolding;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnHoldStart();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnHoldEnd();
    }

    public bool IsHolding()
    {
        return isHolding;
    }

    public void OnHoldStart()
    {
        isHolding = true;
        // Debug.Log("Bắt đầu giữ thông báo");
    }

    public void OnHoldEnd()
    {
        isHolding = false;
        // Debug.Log("Đã buông tay hoàn toàn");
    }
}