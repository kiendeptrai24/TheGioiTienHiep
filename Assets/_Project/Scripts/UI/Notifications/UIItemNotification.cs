using UnityEngine;
using UnityEngine.EventSystems;
public class UIItemNotification : TGTHMonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
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

    // Trường hợp kéo chuột ra ngoài rồi thả
    public void OnPointerExit(PointerEventData eventData)
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
    }

    public void OnHoldEnd()
    {
        isHolding = false;
    }
}
