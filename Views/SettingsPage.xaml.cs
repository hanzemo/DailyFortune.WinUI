using DailyFortune.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace DailyFortune.WinUI.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel { get; }
    public SettingsPage()
    {
        ViewModel = App.Services.GetRequiredService<SettingsViewModel>();
        InitializeComponent();
    }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        ViewModel.Load();
    }
    private void ChangePassword_Click(object sender, RoutedEventArgs e)
        => ViewModel.ChangePasswordCommand.Execute(new PasswordChange(CurPwd.Password, NewPwd.Password));
    private void Logout_Click(object sender, RoutedEventArgs e) => ViewModel.Logout();
    private async void DeleteAccount_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new ContentDialog
        {
            Title = "确认注销",
            Content = "此操作不可撤销，确定要注销账号吗？",
            PrimaryButtonText = "注销",
            CloseButtonText = "取消",
            XamlRoot = XamlRoot
        };
        if (await dlg.ShowAsync() == ContentDialogResult.Primary)
            await ViewModel.DeleteAccountCommand.ExecuteAsync(null);
    }
}
