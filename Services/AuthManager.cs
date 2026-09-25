using System.ComponentModel;
using System.Runtime.CompilerServices;
using DailyFortune.WinUI.Models;

namespace DailyFortune.WinUI.Services;

public class AuthManager : INotifyPropertyChanged
{
    private readonly ApiService _api;
    private readonly TokenStore _tokens;

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _isLoading = true;
    private bool _isAuthenticated;
    private UserMeProfile? _currentUser;

    public bool IsLoading { get => _isLoading; private set => Set(ref _isLoading, value); }
    public bool IsAuthenticated { get => _isAuthenticated; private set => Set(ref _isAuthenticated, value); }
    public UserMeProfile? CurrentUser { get => _currentUser; private set => Set(ref _currentUser, value); }

    public AuthManager(ApiService api, TokenStore tokens)
    {
        _api = api;
        _tokens = tokens;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var token = _tokens.GetAccessToken();
            if (!string.IsNullOrEmpty(token))
                await FetchCurrentUserAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void Login(string access, string refresh, UserMeProfile user)
    {
        _tokens.SaveAccessToken(access);
        _tokens.SaveRefreshToken(refresh);
        CurrentUser = user;
        IsAuthenticated = true;
    }

    public void Logout()
    {
        _tokens.ClearAll();
        CurrentUser = null;
        IsAuthenticated = false;
    }

    public async Task FetchCurrentUserAsync()
    {
        try
        {
            var r = await _api.GetMyProfileAsync();
            CurrentUser = r.User;
            IsAuthenticated = true;
        }
        catch
        {
            if (await TryRefreshAsync())
            {
                try
                {
                    var r = await _api.GetMyProfileAsync();
                    CurrentUser = r.User;
                    IsAuthenticated = true;
                    return;
                }
                catch { }
            }
            Logout();
        }
    }

    private async Task<bool> TryRefreshAsync()
    {
        var refresh = _tokens.GetRefreshToken();
        if (string.IsNullOrEmpty(refresh)) return false;
        try
        {
            var (a, r) = await _api.RefreshTokenAsync(refresh);
            _tokens.SaveAccessToken(a);
            _tokens.SaveRefreshToken(r);
            return true;
        }
        catch { return false; }
    }

    public void UpdateUser(UserMeProfile u) => CurrentUser = u;

    private void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
