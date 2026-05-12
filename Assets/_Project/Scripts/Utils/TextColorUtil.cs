using UnityEngine;

public static class TextColorUtil
{
    // ===== Basic =====

    public static string Color(string text, string hexColor)
    {
        return $"<color={hexColor}>{text}</color>";
    }

    public static string Bold(string text)
    {
        return $"<b>{text}</b>";
    }

    public static string Size(string text, int size)
    {
        return $"<size={size}%>{text}</size>";
    }

    // ===== Preset Colors =====

    public static string Gold(string text)
    {
        return Color(text, "#FFD700");
    }

    public static string Cyan(string text)
    {
        return Color(text, "#00FFFF");
    }

    public static string Green(string text)
    {
        return Color(text, "#00FF7F");
    }

    public static string Red(string text)
    {
        return Color(text, "#FF4444");
    }

    public static string Purple(string text)
    {
        return Color(text, "#B266FF");
    }

    public static string Orange(string text)
    {
        return Color(text, "#FF8C00");
    }

    // ===== Unity Color =====

    public static string Color(string text, Color color)
    {
        string hex = ColorUtility.ToHtmlStringRGB(color);
        return $"<color=#{hex}>{text}</color>";
    }
}