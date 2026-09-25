using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using DailyFortune.WinUI.Models;
using DailyFortune.WinUI.Services;
using Microsoft.UI.Xaml.Media;

namespace DailyFortune.WinUI.ViewModels;

public class ProfileViewModel : INotifyPropertyChanged
{
    private readonly ApiService _api;
    private readonly AuthManager _auth;
    public event PropertyChangedEventHandler? PropertyChanged;
    private void Raise([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new(n));

    private UserPublicProfile? _profile;
    private bool _isLoading;
    private string? _errorMessage;
    private string? _targetUsername;

    public UserPublicProfile? Profile { get => _profile; set { _profile = value; RaiseAll(); } }
    public bool IsLoading { get => _isLoading; set { _isLoading = value; Raise(); } }
    public string? ErrorMessage { get => _errorMessage; set { _errorMessage = value; Raise(); } }
    public bool HasProfile => Profile != null;
    public bool IsMe => _targetUsername == null;

    // 扁平化属性，方便 x:Bind
    public string DisplayName => Profile?.DisplayName ?? "";
    public string UsernameDisplay => Profile == null ? "" : "@" + Profile.Username;
    public string Bio => Profile?.Bio ?? "";
    public string? TodaysFortune => Profile?.TodaysFortune;
    public bool HasDrawnToday => Profile?.HasDrawnToday ?? false;
    public int TotalDraws => Profile?.TotalDraws ?? 0;
    public int Streak => Profile?.Streak ?? 0;
    public string RegistrationDate => Profile?.RegistrationDate.ToString("yyyy-MM-dd") ?? "";
    public string LastActiveDate => Profile?.LastActiveDate.ToString("yyyy-MM-dd") ?? "";
    public ImageSource? AvatarImage => TryImage(Profile?.GetDisplayAvatarUrl());
    public ImageSource? BackgroundImage => TryImage(Profile?.BackgroundUrl);
    public SolidColorBrush FortuneBrush => Utilities.Constants.Brush(Profile?.TodaysFortune);

    public ObservableCollection<FortuneHistoryItem> HistoryItems { get; } = new();

    public ProfileViewModel(ApiService api, AuthManager auth) { _api = api; _auth = auth; }

    public void SetTarget(string? username) => _targetUsername = username;

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            string? username = _targetUsername ?? _auth.CurrentUser?.Username;
            if (string.IsNullOrEmpty(username)) { ErrorMessage = "未指定用户"; return; }

            if (_targetUsername == null)
            {
                var r = await _api.GetMyProfileAsync();
                Profile = ToPublic(r.User);
            }
            else
            {
                Profile = await _api.GetUserProfileAsync(username);
            }

            var hist = await _api.GetUserFortuneHistoryAsync(username);
            HistoryItems.Clear();
            foreach (var h in hist) HistoryItems.Add(h);
        }
        catch (Exception ex) { ErrorMessage = ex.Message; }
        finally { IsLoading = false; }
    }

    private void RaiseAll()
    {
        Raise(nameof(Profile));
        Raise(nameof(HasProfile));
        Raise(nameof(DisplayName));
        Raise(nameof(UsernameDisplay));
        Raise(nameof(Bio));
        Raise(nameof(TodaysFortune));
        Raise(nameof(HasDrawnToday));
        Raise(nameof(TotalDraws));
        Raise(nameof(Streak));
        Raise(nameof(RegistrationDate));
        Raise(nameof(LastActiveDate));
        Raise(nameof(AvatarImage));
        Raise(nameof(BackgroundImage));
        Raise(nameof(FortuneBrush));
    }

    private static ImageSource? TryImage(string? url)
    {
        if (string.IsNullOrEmpty(url)) return null;
        return new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(url));
    }

    private static UserPublicProfile ToPublic(UserMeProfile p) => new()
    {
        Username = p.Username, DisplayName = p.DisplayName, Bio = p.Bio,
        AvatarUrl = p.AvatarUrl, BackgroundUrl = p.BackgroundUrl,
        RegistrationDate = p.RegistrationDate, LastActiveDate = p.LastActiveDate,
        TotalDraws = p.TotalDraws, HasDrawnToday = p.HasDrawnToday,
        TodaysFortune = p.TodaysFortune, Status = p.Status, IsHidden = p.IsHidden,
        Tags = p.Tags, Qq = p.Qq, UseQqAvatar = p.UseQqAvatar,
        Streak = p.Streak, FortuneCounts = p.FortuneCounts
    };
}
