using DailyFortune.WinUI.Services;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace DailyFortune.WinUI.Utilities;

public static class Constants
{
    public static readonly Dictionary<string, string> FortuneColors = new()
    {
        ["諭吉"] = "#eec54b",
        ["大吉"] = "#C73E3A",
        ["吉"]   = "#9cca26",
        ["中吉"] = "#eaaa66",
        ["小吉"] = "#4cd3cf",
        ["凶"]   = "#67278F",
        ["大凶"] = "#1A297E",
    };

    public static readonly Dictionary<string, int> HeatmapLevels = new()
    {
        ["大凶"] = 1, ["凶"] = 2, ["小吉"] = 3, ["中吉"] = 4,
        ["吉"] = 5, ["大吉"] = 6, ["諭吉"] = 7,
    };

    public static readonly string[] HeatmapLight = {
        "", "#d32f2f", "#e57373", "#aceebb", "#78d593", "#4ac26b", "#2da44e", "#116329"
    };
    public static readonly string[] HeatmapDark = {
        "", "#ef9a9a", "#e57373", "#033a16", "#196c2e", "#2ea043", "#42bb53", "#56d364"
    };

    public static readonly string[] Timezones = {
        "UTC", "Asia/Shanghai", "Asia/Tokyo", "Europe/London",
        "Europe/Paris", "America/New_York", "America/Chicago", "America/Los_Angeles"
    };

    public static SolidColorBrush Brush(string? fortune, bool dark = false)
    {
        var key = fortune?.Trim() ?? "";
        AppLog.Log($"[Brush] key=[{key}] len={key.Length} found={FortuneColors.ContainsKey(key)}");
        if (!FortuneColors.TryGetValue(key, out var hex))
            return new SolidColorBrush(Colors.Gray);
        return new SolidColorBrush(FromHex(hex, dark ? 0.7 : 1.0));
    }

    public static Color FromHex(string hex, double scale = 1.0)
    {
        hex = hex.TrimStart('#');
        byte r = Convert.ToByte(hex.Substring(0, 2), 16);
        byte g = Convert.ToByte(hex.Substring(2, 2), 16);
        byte b = Convert.ToByte(hex.Substring(4, 2), 16);
        return Color.FromArgb(255,
            (byte)(r * scale), (byte)(g * scale), (byte)(b * scale));
    }
}
