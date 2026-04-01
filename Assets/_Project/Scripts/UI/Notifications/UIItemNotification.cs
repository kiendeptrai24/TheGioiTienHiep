using UnityEngine;
using UnityEngine.EventSystems;
public class UIItemNotification : TGTHMonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private bool isHolding;
    public void OnPointerDown(PointerEventData eventData)
    {
        OnHoldStart();
        Debug.Log("Down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnHoldEnd();
        Debug.Log("Up");
    }

    // Trường hợp kéo chuột ra ngoài rồi thả
    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoldEnd();
        Debug.Log("Exit");
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
