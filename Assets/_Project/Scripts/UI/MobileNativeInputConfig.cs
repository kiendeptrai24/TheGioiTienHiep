using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(TMP_InputField))]
public class MobileNativeInputConfig : MonoBehaviour
{
    [Header("Mobile Native Input")]
    [SerializeField] private bool showNativeMobileInput = true;

    [Header("Input Fix")]
    [SerializeField] private bool disableSelectAllOnFocus = true;
    [SerializeField] private int caretWidth = 2;
    [SerializeField] private float caretBlinkRate = 0.85f;

    private TMP_InputField inputField;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        Apply();
    }

    private void OnEnable()
    {
        Apply();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        inputField = GetComponent<TMP_InputField>();
        Apply();
    }
#endif

    private void Apply()
    {
        if (inputField == null) return;

        // false = hiện ô nhập native phía trên bàn phím
        // true  = ẩn ô native, chỉ nhập trong UI game
        inputField.shouldHideMobileInput = !showNativeMobileInput;

        if (disableSelectAllOnFocus)
        {
            inputField.onFocusSelectAll = false;
        }

        inputField.caretWidth = Mathf.Max(1, caretWidth);
        inputField.caretBlinkRate = caretBlinkRate;
    }

    public void SetShowNativeMobileInput(bool show)
    {
        showNativeMobileInput = show;
        Apply();
    }
}