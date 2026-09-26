using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DailyFortune.WinUI.Utilities;

public class FlexibleDateTimeConverter : JsonConverter<DateTime>
{
    private static readonly string[] Formats = {
        "yyyy-MM-dd'T'HH:mm:ss.FFFFFFK",
        "yyyy-MM-dd'T'HH:mm:ss.FFFFFFFZ",
        "yyyy-MM-dd'T'HH:mm:ssK",
        "yyyy-MM-dd'T'HH:mm:ss'Z'",
    };

    public override DateTime Read(ref Utf8JsonReader reader, Type t, JsonSerializerOptions o)
    {
        var s = reader.GetString();
        if (string.IsNullOrEmpty(s)) return default;

        // 带时区标记，标准解析
        foreach (var f in Formats)
        {
            if (DateTime.TryParseExact(s, f, CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var dt))
                return dt;
        }

        // 无时区 → 后端返回北京时间字面值 → 减去 8 小时得 UTC
        if (DateTime.TryParseExact(s, "yyyy-MM-dd'T'HH:mm:ss",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var local))
        {
            return DateTime.SpecifyKind(local, DateTimeKind.Utc).AddHours(-8);
        }

        if (DateTime.TryParse(s, CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var any))
            return any;

        throw new JsonException($"无法解析日期: {s}");
    }

    public override void Write(Utf8JsonWriter w, DateTime v, JsonSerializerOptions o)
        => w.WriteStringValue(v.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.FFFFFF'Z'"));
}

public class FlexibleNullableDateTimeConverter : JsonConverter<DateTime?>
{
    private readonly FlexibleDateTimeConverter _inner = new();
    public override DateTime? Read(ref Utf8JsonReader r, Type t, JsonSerializerOptions o)
    {
        if (r.TokenType == JsonTokenType.Null) return null;
        return _inner.Read(ref r, t, o);
    }
    public override void Write(Utf8JsonWriter w, DateTime? v, JsonSerializerOptions o)
    {
        if (v is null) { w.WriteNullValue(); return; }
        _inner.Write(w, v.Value, o);
    }
}
