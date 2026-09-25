using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace DailyFortune.WinUI.Models;

public partial class UserPublicProfile : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private void Raise([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new(n));

    private string _username = "";
    private string _displayName = "";
    private string _bio = "";
    private string _avatarUrl = "";
    private string _backgroundUrl = "";
    private DateTime _registrationDate;
    private DateTime _lastActiveDate;
    private int _totalDraws;
    private bool _hasDrawnToday;
    private string? _todaysFortune;
    private string _status = "";
    private bool _isHidden;
    private List<string> _tags = new();
    private long? _qq;
    private bool _useQqAvatar;
    private int? _streak;
    private Dictionary<string, int>? _fortuneCounts;

    [JsonPropertyName("username")] public string Username { get => _username; set { _username = value; Raise(); } }
    [JsonPropertyName("display_name")] public string DisplayName { get => _displayName; set { _displayName = value; Raise(); } }
    [JsonPropertyName("bio")] public string Bio { get => _bio; set { _bio = value; Raise(); } }
    [JsonPropertyName("avatar_url")] public string AvatarUrl { get => _avatarUrl; set { _avatarUrl = value; Raise(); } }
    [JsonPropertyName("background_url")] public string BackgroundUrl { get => _backgroundUrl; set { _backgroundUrl = value; Raise(); } }
    [JsonPropertyName("registration_date")] public DateTime RegistrationDate { get => _registrationDate; set { _registrationDate = value; Raise(); } }
    [JsonPropertyName("last_active_date")] public DateTime LastActiveDate { get => _lastActiveDate; set { _lastActiveDate = value; Raise(); } }
    [JsonPropertyName("total_draws")] public int TotalDraws { get => _totalDraws; set { _totalDraws = value; Raise(); } }
    [JsonPropertyName("has_drawn_today")] public bool HasDrawnToday { get => _hasDrawnToday; set { _hasDrawnToday = value; Raise(); } }
    [JsonPropertyName("todays_fortune")] public string? TodaysFortune { get => _todaysFortune; set { _todaysFortune = value; Raise(); } }
    [JsonPropertyName("status")] public string Status { get => _status; set { _status = value; Raise(); } }
    [JsonPropertyName("is_hidden")] public bool IsHidden { get => _isHidden; set { _isHidden = value; Raise(); } }
    [JsonPropertyName("tags")] public List<string> Tags { get => _tags; set { _tags = value; Raise(); } }
    [JsonPropertyName("qq")] public long? Qq { get => _qq; set { _qq = value; Raise(); } }
    [JsonPropertyName("use_qq_avatar")] public bool UseQqAvatar { get => _useQqAvatar; set { _useQqAvatar = value; Raise(); } }
    [JsonPropertyName("streak")] public int? Streak { get => _streak; set { _streak = value; Raise(); } }
    [JsonPropertyName("fortune_counts")] public Dictionary<string, int>? FortuneCounts { get => _fortuneCounts; set { _fortuneCounts = value; Raise(); } }

    public string? GetDisplayAvatarUrl()
    {
        if (UseQqAvatar && Qq.HasValue) return $"https://q.qlogo.cn/g?b=qq&nk={Qq}&s=640";
        return string.IsNullOrEmpty(AvatarUrl) ? null : AvatarUrl;
    }
}
