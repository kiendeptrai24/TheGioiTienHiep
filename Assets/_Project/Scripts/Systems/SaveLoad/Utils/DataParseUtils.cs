using System.Globalization;
using System.Linq;
using UnityEngine;

public static class DataParseUtils
{
    public static float ParseNumberOrPercent(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0f;

        value = value.Trim();

        if (value.Contains("%"))
            return ParsePercent(value);

        return float.Parse(value, CultureInfo.InvariantCulture);
    }
    public static float ParsePercent(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0f;

        value = value.Replace("%", "").Trim();
        return float.Parse(value, CultureInfo.InvariantCulture) / 100f;
    }

    public static ulong ParseTimeToSeconds(string time)
    {
        if (string.IsNullOrWhiteSpace(time)) return 0;

        time = time.Trim().ToLower();

        string numberText = new string(time.Where(char.IsDigit).ToArray());

        if (string.IsNullOrEmpty(numberText)) return 0;

        ulong value = ulong.Parse(numberText);

        if (time.EndsWith("s")) return value;
        if (time.EndsWith("m")) return value * 60;
        if (time.EndsWith("h")) return value * 3600;

        return 0;
    }
}