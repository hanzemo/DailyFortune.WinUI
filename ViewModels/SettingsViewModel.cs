using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using DailyFortune.WinUI.Models;
using DailyFortune.WinUI.Services;

namespace DailyFortune.WinUI.ViewModels;

public record PasswordChange(string Current, string New);

public class SettingsViewModel : INotifyPropertyChanged
{
    private readonly ApiService _api;
    private readonly AuthManager _auth;
    public event PropertyChangedEventHandler? PropertyChanged;
    private void Raise([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new(n));

    private string _displayName = "";
    private string _bio = "";
    private string _email = "";
    private string _language = "zh-CN";
    private string _timezone = "Asia/Shanghai";
    private string? _message;
    private bool _isSaving;

    public string DisplayName { get => _displayName; set { _displayName = value; Raise(); } }
    public string Bio { get => _bio; set { _bio = value; Raise(); } }
    public string Email { get => _email; set { _email = value; Raise(); } }
    public string Language { get => _language; set { _language = value; Raise(); } }
    public string Timezone { get => _timezone; set { _timezone = value; Raise(); } }
    public string? Message { get => _message; set { _message = value; Raise(); } }
    public bool IsSaving { get => _isSaving; set { _isSaving = value; Raise(); } }

    public string[] Timezones => Utilities.Constants.Timezones;

    public SettingsViewModel(ApiService api, AuthManager auth)
    {
        _api = api; _auth = auth;
    }

    public void Load()
    {
        var u = _auth.CurrentUser;
        if (u == null) return;
        DisplayName = u.DisplayName;
        Bio = u.Bio;
        Email = u.Email;
        Language = u.Language;
        Timezone = u.Timezone;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var u = _auth.CurrentUser;
        if (u == null) return;
        IsSaving = true; Message = null;
        try
        {
            var p = new UserUpdatePayload();
            if (DisplayName != u.DisplayName) p.DisplayName = DisplayName;
            if (Bio != u.Bio) p.Bio = Bio;
            if (Email != u.Email) p.Email = Email;
            if (Language != u.Language) p.Language = Language;
            if (Timezone != u.Timezone) p.Timezone = Timezone;

            var r = await _api.UpdateMyProfileAsync(p);
            _auth.UpdateUser(r.User);
            Message = "已保存";
        }
        catch (Exception ex) { Message = ex.Message; }
        finally { IsSaving = false; }
    }

    [RelayCommand]
    private async Task ChangePasswordAsync(PasswordChange args)
    {
        Message = null;
        try
        {
            await _api.ChangePasswordAsync(args.Current, args.New);
            Message = "密码已修改";
        }
        catch (Exception ex) { Message = ex.Message; }
    }

    [RelayCommand]
    private async Task DeleteAccountAsync()
    {
        try { await _api.DeleteMyAccountAsync(); _auth.Logout(); }
        catch (Exception ex) { Message = ex.Message; }
    }

    public void Logout() => _auth.Logout();
}
