using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using DailyFortune.WinUI.Services;

namespace DailyFortune.WinUI.ViewModels;

public partial class RegisterViewModel : INotifyPropertyChanged
{
    private readonly ApiService _api;
    private readonly AuthManager _auth;
    public event PropertyChangedEventHandler? PropertyChanged;
    private void Raise([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new(n));

    private string _username = "";
    private string _email = "";
    private string _password = "";
    private string? _error;
    private bool _isBusy;
    private bool _isOpen = true;

    public string Username { get => _username; set { _username = value; Raise(); } }
    public string Email { get => _email; set { _email = value; Raise(); } }
    public string Password { get => _password; set { _password = value; Raise(); } }
    public string? Error { get => _error; set { _error = value; Raise(); } }
    public bool IsBusy { get => _isBusy; set { _isBusy = value; Raise(); } }
    public bool IsOpen { get => _isOpen; set { _isOpen = value; Raise(); } }

    public RegisterViewModel(ApiService api, AuthManager auth) { _api = api; _auth = auth; }

    public async Task CheckRegistrationOpenAsync()
    {
        try { var s = await _api.GetRegistrationStatusAsync(); IsOpen = s.IsOpen; }
        catch { IsOpen = true; }
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        Error = null;
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrEmpty(Password))
        { Error = "请填写完整信息"; return; }

        IsBusy = true;
        try
        {
            var r = await _api.RegisterAsync(Username, Email, Password);
            if (r.User is null) { Error = "注册失败：响应缺少用户信息"; return; }

            _auth.Login(r.AccessToken, r.RefreshToken, r.User);
        }
        catch (Exception ex) { Error = ex.Message; }
        finally { IsBusy = false; }
    }
}
