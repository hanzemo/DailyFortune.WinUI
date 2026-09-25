using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using DailyFortune.WinUI.Models;
using DailyFortune.WinUI.Services;

namespace DailyFortune.WinUI.ViewModels;

public record RoleChange(UserMeProfile User, string Role);
public record PasswordReset(UserMeProfile User, string NewPassword);
public record UserEdit(UserMeProfile User, UserUpdatePayload Payload);

public partial class AdminUserListViewModel : INotifyPropertyChanged
{
    private readonly ApiService _api;
    private readonly AuthManager _auth;
    public event PropertyChangedEventHandler? PropertyChanged;
    private void Raise([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new(n));

    private bool _isLoading;
    private string? _errorMessage;
    private string _searchText = "";

    public bool IsLoading { get => _isLoading; set { _isLoading = value; Raise(); } }
    public string? ErrorMessage { get => _errorMessage; set { _errorMessage = value; Raise(); } }
    public string SearchText { get => _searchText; set { _searchText = value; Raise(); ApplyFilter(); } }

    public ObservableCollection<UserMeProfile> Users { get; } = new();
    private List<UserMeProfile> _all = new();

    public bool IsAdmin => _auth.CurrentUser?.Role == "admin";

    public AdminUserListViewModel(ApiService api, AuthManager auth) { _api = api; _auth = auth; }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (!IsAdmin) { ErrorMessage = "无权限"; return; }
        IsLoading = true; ErrorMessage = null;
        try
        {
            _all = await _api.GetAllUsersAsync();
            ApplyFilter();
        }
        catch (Exception ex) { ErrorMessage = ex.Message; }
        finally { IsLoading = false; }
    }

    private void ApplyFilter()
    {
        Users.Clear();
        var q = SearchText?.Trim().ToLowerInvariant() ?? "";
        var src = string.IsNullOrEmpty(q) ? _all : _all.Where(u =>
            u.Username.ToLower().Contains(q) ||
            u.DisplayName.ToLower().Contains(q) ||
            u.Email.ToLower().Contains(q)).ToList();
        foreach (var u in src) Users.Add(u);
    }

    [RelayCommand]
    private async Task ToggleStatusAsync(UserMeProfile u)
        => await RunAsync(() => _api.UpdateUserStatusAsync(u.Id, u.Status == "active" ? "inactive" : "active"));

    [RelayCommand]
    private async Task ToggleVisibilityAsync(UserMeProfile u)
        => await RunAsync(() => _api.UpdateUserVisibilityAsync(u.Id, !u.IsHidden));

    [RelayCommand]
    private async Task SetRoleAsync(RoleChange args)
        => await RunAsync(() => _api.AdminUpdateRoleAsync(args.User.Id, args.Role));

    [RelayCommand]
    private async Task DeleteUserAsync(UserMeProfile u)
        => await RunAsync(() => _api.AdminDeleteUserAsync(u.Id));

    [RelayCommand]
    private async Task ResetPasswordAsync(PasswordReset args)
        => await RunAsync(() => _api.AdminResetPasswordAsync(args.User.Id, args.NewPassword));

    [RelayCommand]
    private async Task SaveUserAsync(UserEdit args)
        => await RunAsync(() => _api.AdminUpdateUserAsync(args.User.Id, args.Payload));

    private async Task RunAsync(Func<Task> action)
    {
        try { await action(); await LoadAsync(); }
        catch (Exception ex) { ErrorMessage = ex.Message; }
    }
}
