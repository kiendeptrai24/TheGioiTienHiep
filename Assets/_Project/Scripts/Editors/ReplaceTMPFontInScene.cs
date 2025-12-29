using UnityEngine;
using TMPro;

public class ReplaceTMPFontInScene : MonoBehaviour
{
    public TMP_FontAsset newFont;

    [ContextMenu("Replace All TMP Fonts")]
    void ReplaceFonts()
    {
        var texts = FindObjectsByType<TextMeshProUGUI>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );
        foreach (var t in texts)
        {
            t.font = newFont;
        }
        Debug.Log($"Replaced {texts.Length} TMP texts");
    }
}
