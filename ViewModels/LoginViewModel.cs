using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;
using DailyFortune.WinUI.Services;

namespace DailyFortune.WinUI.ViewModels;

public partial class LoginViewModel : INotifyPropertyChanged
{
    private readonly ApiService _api;
    private readonly AuthManager _auth;
    public event PropertyChangedEventHandler? PropertyChanged;
    private void Raise([CallerMemberName] string? n = null) => PropertyChanged?.Invoke(this, new(n));

    private string _username = "";
    private string _password = "";
    private string? _error;
    private bool _isBusy;

    public string Username { get => _username; set { _username = value; Raise(); } }
    public string Password { get => _password; set { _password = value; Raise(); } }
    public string? Error { get => _error; set { _error = value; Raise(); } }
    public bool IsBusy { get => _isBusy; set { _isBusy = value; Raise(); } }

    public LoginViewModel(ApiService api, AuthManager auth) { _api = api; _auth = auth; }

    [RelayCommand]
    private async Task LoginAsync()
    {
        Error = null;
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrEmpty(Password))
        { Error = "请输入用户名和密码"; return; }

        IsBusy = true;
        try
        {
            var r = await _api.LoginAsync(Username, Password);
            if (r.User is null) { Error = "登录失败：响应缺少用户信息"; return; }

            _auth.Login(r.AccessToken, r.RefreshToken, r.User);
        }
        catch (Exception ex) { Error = ex.Message; }
        finally { IsBusy = false; }
    }
}
