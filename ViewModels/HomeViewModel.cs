using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using DailyFortune.WinUI.Services;
using Microsoft.UI.Xaml;

namespace DailyFortune.WinUI.ViewModels;

public partial class HomeViewModel : INotifyPropertyChanged
{
    private readonly ApiService _api;
    private readonly AuthManager _auth;
    private DispatcherTimer? _timer;
    private DateTime? _nextDrawAt;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void Raise([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new(n));

    private string? _fortune;
    private string _countdown = "";
    private bool _isLoading;
    private string? _errorMessage;

    public string? Fortune { get => _fortune; private set { _fortune = value; Raise(); Raise(nameof(HasFortune)); Raise(nameof(CanDraw)); } }
    public string Countdown { get => _countdown; private set { _countdown = value; Raise(); Raise(nameof(HasCountdown)); } }
    public bool IsLoading { get => _isLoading; private set { _isLoading = value; Raise(); Raise(nameof(CanDraw)); } }
    public string? ErrorMessage { get => _errorMessage; private set { _errorMessage = value; Raise(); } }

    public bool HasFortune => !string.IsNullOrEmpty(Fortune);
    public bool HasCountdown => !string.IsNullOrEmpty(Countdown);
    public bool CanDraw => !IsLoading && !HasFortune;

    public HomeViewModel(ApiService api, AuthManager auth)
    {
        _api = api;
        _auth = auth;
    }

    public async Task LoadAsync()
    {
        if (!_auth.IsAuthenticated) { Fortune = null; Countdown = ""; return; }
        var user = _auth.CurrentUser;
        if (user is null) return;

        if (user.HasDrawnToday)
        {
            Fortune = user.TodaysFortune;
            try
            {
                var r = await _api.GetMyProfileAsync();
                _nextDrawAt = r.NextDrawAt;
                StartCountdown();
            }
            catch { }
        }
        else
        {
            Fortune = null;
            _nextDrawAt = null;
            Countdown = "";
            StopTimer();
        }
    }

    [RelayCommand]
    private async Task DrawAsync()
    {
        ErrorMessage = null;

        if (!_auth.IsAuthenticated)
        {
            Fortune = FortuneUtils.DrawLocally();
            return;
        }

        IsLoading = true;
        try
        {
            var r = await _api.DrawFortuneAsync();
            Fortune = r.Fortune;
            _nextDrawAt = r.NextDrawAt;
            StartCountdown();
            await _auth.FetchCurrentUserAsync();
        }
        catch (Exception ex) { ErrorMessage = ex.Message; }
        finally { IsLoading = false; }
    }

    private void StopTimer()
    {
        if (_timer is not null)
        {
            _timer.Stop();
            _timer.Tick -= OnTick;
            _timer = null;
        }
    }

    private void StartCountdown()
    {
        StopTimer();
        if (_nextDrawAt is null) return;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += OnTick;
        _timer.Start();
        OnTick(null, EventArgs.Empty);
    }

    private async void OnTick(object? sender, object e)
    {
        var target = _nextDrawAt;
        if (target is null) { StopTimer(); return; }

        var diff = target.Value - DateTime.UtcNow;
        if (diff <= TimeSpan.Zero)
        {
            StopTimer();
            Fortune = null;
            _nextDrawAt = null;
            Countdown = "";
            await _auth.FetchCurrentUserAsync();
            return;
        }
        Countdown = $"{(int)diff.TotalHours:D2}:{diff.Minutes:D2}:{diff.Seconds:D2}";
    }
}
