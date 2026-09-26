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

    private async void Diagnostics_Click(object sender, RoutedEventArgs e)
    {
        var logText = DailyFortune.WinUI.Services.AppLog.ReadAll();
        if (logText.Length > 10000)
            logText = logText.Substring(logText.Length - 10000);

        var scroll = new ScrollViewer
        {
            Content = new TextBlock
            {
                Text = logText,
                FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Consolas"),
                FontSize = 11,
                TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap,
                IsTextSelectionEnabled = true
            },
            Height = 500,
            Width = 700
        };

        var dlg = new ContentDialog
        {
            Title = "诊断日志",
            Content = scroll,
            PrimaryButtonText = "清空日志",
            CloseButtonText = "关闭",
            XamlRoot = XamlRoot
        };

        var result = await dlg.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            try
            {
                var path = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "DailyFortune.log");
                System.IO.File.WriteAllText(path, "");
            }
            catch { }
        }
    }
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
