using UnityEngine;
using TMPro;

public class ReplaceTMPFontInScene : MonoBehaviour
{
    public TMP_FontAsset newFont;

    [ContextMenu("Replace All TMP Fonts")]
    void ReplaceFonts()
    {
        var texts = FindObjectsOfType<TextMeshProUGUI>(true);
        foreach (var t in texts)
        {
            t.font = newFont;
        }
        Debug.Log($"Replaced {texts.Length} TMP texts");
    }
}
