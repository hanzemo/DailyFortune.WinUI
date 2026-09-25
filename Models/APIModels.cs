using System.Text.Json.Serialization;

namespace DailyFortune.WinUI.Models;

public class UserMeProfile
{
    [JsonPropertyName("id")] public string Id { get; set; } = "";
    [JsonPropertyName("email")] public string Email { get; set; } = "";
    [JsonPropertyName("role")] public string Role { get; set; } = "";
    [JsonPropertyName("language")] public string Language { get; set; } = "";
    [JsonPropertyName("timezone")] public string Timezone { get; set; } = "";
    [JsonPropertyName("username")] public string Username { get; set; } = "";
    [JsonPropertyName("display_name")] public string DisplayName { get; set; } = "";
    [JsonPropertyName("bio")] public string Bio { get; set; } = "";
    [JsonPropertyName("avatar_url")] public string AvatarUrl { get; set; } = "";
    [JsonPropertyName("background_url")] public string BackgroundUrl { get; set; } = "";
    [JsonPropertyName("registration_date")] public DateTime RegistrationDate { get; set; }
    [JsonPropertyName("last_active_date")] public DateTime LastActiveDate { get; set; }
    [JsonPropertyName("total_draws")] public int TotalDraws { get; set; }
    [JsonPropertyName("has_drawn_today")] public bool HasDrawnToday { get; set; }
    [JsonPropertyName("todays_fortune")] public string? TodaysFortune { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; } = "";
    [JsonPropertyName("is_hidden")] public bool IsHidden { get; set; }
    [JsonPropertyName("tags")] public List<string> Tags { get; set; } = new();
    [JsonPropertyName("qq")] public long? Qq { get; set; }
    [JsonPropertyName("use_qq_avatar")] public bool UseQqAvatar { get; set; }
    [JsonPropertyName("streak")] public int? Streak { get; set; }
    [JsonPropertyName("fortune_counts")] public Dictionary<string, int>? FortuneCounts { get; set; }

    [JsonIgnore] public bool IsAdmin => Role == "admin";
    [JsonIgnore] public string TagsText => Tags.Count == 0 ? "" : string.Join(",", Tags);

    public string? GetDisplayAvatarUrl()
    {
        if (UseQqAvatar && Qq.HasValue) return $"https://q.qlogo.cn/g?b=qq&nk={Qq}&s=640";
        return string.IsNullOrEmpty(AvatarUrl) ? null : AvatarUrl;
    }
}

public class AuthResponse
{
    [JsonPropertyName("access_token")] public string AccessToken { get; set; } = "";
    [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; } = "";
    [JsonPropertyName("token_type")] public string TokenType { get; set; } = "";
    [JsonPropertyName("user")] public UserMeProfile? User { get; set; }
}

public class MyProfileResponse
{
    [JsonPropertyName("user")] public UserMeProfile User { get; set; } = new();
    [JsonPropertyName("next_draw_at")] public DateTime? NextDrawAt { get; set; }
}

public class RegistrationStatusResponse
{
    [JsonPropertyName("is_open")] public bool IsOpen { get; set; }
}

public class FortuneDrawResponse
{
    [JsonPropertyName("fortune")] public string Fortune { get; set; } = "";
    [JsonPropertyName("next_draw_at")] public DateTime? NextDrawAt { get; set; }
}

public class FortuneHistoryItem
{
    [JsonPropertyName("created_at")] public DateTime CreatedAt { get; set; }
    [JsonPropertyName("value")] public string Value { get; set; } = "";
}

public class LeaderboardGroup
{
    [JsonPropertyName("fortune")] public string Fortune { get; set; } = "";
    [JsonPropertyName("users")] public List<LeaderboardUser> Users { get; set; } = new();
}

public class LeaderboardUser
{
    [JsonPropertyName("username")] public string Username { get; set; } = "";
    [JsonPropertyName("display_name")] public string DisplayName { get; set; } = "";
}

public class APIErrorResponse
{
    [JsonPropertyName("detail")] public string? Detail { get; set; }
}

public class UserUpdatePayload
{
    [JsonPropertyName("display_name")] public string? DisplayName { get; set; }
    [JsonPropertyName("email")] public string? Email { get; set; }
    [JsonPropertyName("bio")] public string? Bio { get; set; }
    [JsonPropertyName("avatar_url")] public string? AvatarUrl { get; set; }
    [JsonPropertyName("background_url")] public string? BackgroundUrl { get; set; }
    [JsonPropertyName("language")] public string? Language { get; set; }
    [JsonPropertyName("timezone")] public string? Timezone { get; set; }
    [JsonPropertyName("qq")] public long? Qq { get; set; }
    [JsonPropertyName("use_qq_avatar")] public bool? UseQqAvatar { get; set; }
}
