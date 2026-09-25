using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using DailyFortune.WinUI.Models;
using DailyFortune.WinUI.Services;

namespace DailyFortune.WinUI.ViewModels;

public partial class LeaderboardViewModel : INotifyPropertyChanged
{
    private readonly ApiService _api;
    public event PropertyChangedEventHandler? PropertyChanged;
    private void Raise([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new(n));

    private bool _isLoading;
    private string? _errorMessage;
    private LeaderboardPeriod _period = LeaderboardPeriod.today;

    public bool IsLoading { get => _isLoading; set { _isLoading = value; Raise(); } }
    public string? ErrorMessage { get => _errorMessage; set { _errorMessage = value; Raise(); } }
    public LeaderboardPeriod Period
    {
        get => _period;
        set { if (_period == value) return; _period = value; Raise(); Raise(nameof(PeriodIndex)); }
    }

    public int PeriodIndex
    {
        get => (int)_period;
        set { Period = (LeaderboardPeriod)value; _ = LoadAsync(); }
    }

    public ObservableCollection<LeaderboardGroup> Groups { get; } = new();

    public LeaderboardViewModel(ApiService api) { _api = api; }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var list = await _api.GetLeaderboardAsync(Period);
            Groups.Clear();
            foreach (var g in list) Groups.Add(g);
        }
        catch (Exception ex) { ErrorMessage = ex.Message; }
        finally { IsLoading = false; }
    }
}
