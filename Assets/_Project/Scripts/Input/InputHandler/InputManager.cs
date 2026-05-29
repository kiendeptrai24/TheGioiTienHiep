

using System;
using System.Collections.Generic;
using FeatureToggles;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.InputSystem.InputAction;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UnityEngine.InputSystem.EnhancedTouch;

public enum InputType
{
    Player,
    Verhicle,
    UI,

}
public class InputManager : MonoBehaviour
{
    public InputHandler inputHandler;
    public InputType inputType;
    public Action OnEnterClick;
    private bool isPressed;
    public event Action<Vector2> OnPointerPositionClick;
    private FeatureManager _mgr;
    private PointerEventData pointerEventData;
    private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();
    public void Awake()
    {
        inputHandler = new InputHandler();

        inputHandler.UI.Enter.started += (CallbackContext context) => { OnEnterClick?.Invoke(); };

        inputHandler.Player.PointerPress.started += (CallbackContext context) =>
        {
            if (isPressed) return;
            isPressed = true;

            Vector2 pointerPos = GetPointerPosition();
            if (IsPointerOverUI(pointerPos) == false)
            {
                OnPointerPositionClick?.Invoke(pointerPos);
            }
        };

        inputHandler.Player.PointerPress.canceled += (CallbackContext context) =>
        {
            isPressed = false;
        };

        _mgr = FeatureManager.Instance;
        _mgr.OnFeatureEffectiveChanged += OnChanged;
    }
    private void OnChanged(FeatureId id, bool unlockInput)
    {
        switch (id)
        {

            case FeatureId.WorldClick_Enabled:
                if (unlockInput)
                {
                    TurnOnPlayerInput();
                }
                else
                {
                    TurnOffPlayerInput();
                }
                break;
            case FeatureId.BattleScene_Enabled:
                if (unlockInput)
                {
                    TurnOnAllInput();
                }
                else
                {
                    TurnOffAllInput();
                }
                break;
            default:
                break;
        }
    }

    #region Player
    public Vector2 GetInputDirection()
    {
        if (!inputHandler.Player.enabled) return Vector2.zero;
        return inputHandler.Player.Move.ReadValue<Vector2>();
    }
    private bool IsPointerOverUI(Vector2 screenPos)
    {
        if (EventSystem.current == null) return true;

        if (pointerEventData == null)
            pointerEventData = new PointerEventData(EventSystem.current);

        pointerEventData.position = screenPos;

        raycastResults.Clear();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        foreach (var ray in raycastResults)
        {
            if (ray.gameObject.CompareTag("IgnoreUI") == false)
                return true;
        }

        return false;
    }

    public Vector2 GetPointerPosition()
    {
        if (!inputHandler.Player.enabled) return Vector2.zero;
        return inputHandler.Player.PointerPosition.ReadValue<Vector2>();
    }
    #endregion

    #region UI
    public float GetInputScrollWheel()
    {
        return GetZoomInput();
    }
    public bool IsZoom() => Mathf.Abs(GetZoomInput()) > 0.001f;
    public float GetZoomInput()
    {
        Vector2 scroll = inputHandler.UI.Scroll.ReadValue<Vector2>();
        if (scroll.y != 0)
            return scroll.y;

        if (Touch.activeTouches.Count >= 2)
        {
            var t0 = Touch.activeTouches[0];
            var t1 = Touch.activeTouches[1];

            float prev = Vector2.Distance(
                t0.screenPosition - t0.delta,
                t1.screenPosition - t1.delta
            );

            float curr = Vector2.Distance(
                t0.screenPosition,
                t1.screenPosition
            );
            return (curr - prev) * 0.01f; // giá trị zoom thật (mượt)
        }

        return 0;
    }
    public Vector2 GetUIPrimavePointerPosition()
    {
        return inputHandler.UI.PrimaryFingerPosition.ReadValue<Vector2>();
    }
    public Vector2 GetUISecondaryPointerPosition()
    {
        return inputHandler.UI.SecondaryFingerPosition.ReadValue<Vector2>();
    }
    public Vector2 GetUIPointerDelta()
    {
        return inputHandler.UI.PointerDelta.ReadValue<Vector2>();
    }
    public bool IsUIPointerPressed()
    {
        return inputHandler.UI.PointerPress.IsPressed();
    }

    public Vector2 GetUIPointerPosition()
    {
        return inputHandler.UI.PointerPosition.ReadValue<Vector2>();
    }
    #endregion

    #region Toggle Player

    public void TurnOnPlayerInput()
    {
        inputHandler.Player.Enable();
    }
    public void TurnOffPlayerInput()
    {
        inputHandler.Player.Disable();
    }
    #endregion

    #region Toggle UI
    public void TurnOnUIInput()
    {
        inputHandler.UI.Enable();
    }
    public void TurnOffUIInput()
    {
        inputHandler.UI.Disable();
    }

    #endregion
    public void TurnOffAllInput()
    {
        inputHandler.Disable();
    }
    public void TurnOnAllInput()
    {
        inputHandler.Enable();
    }
    private void OnEnable()
    {
        inputHandler.Enable();
        EnhancedTouchSupport.Enable();
    }
    private void OnDisable()
    {
        inputHandler.Disable();
        EnhancedTouchSupport.Disable();
    }
}