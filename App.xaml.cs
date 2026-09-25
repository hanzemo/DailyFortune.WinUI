using DailyFortune.WinUI.Services;
using DailyFortune.WinUI.ViewModels;
using DailyFortune.WinUI.Views;
using DailyFortune.WinUI.Views.Admin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

namespace DailyFortune.WinUI;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;
    public static Window? MainWindow { get; private set; }

    public App()
    {
        InitializeComponent();
        Services = ConfigureServices();
    }

    private static IServiceProvider ConfigureServices()
    {
        var s = new ServiceCollection();
        s.AddSingleton<TokenStore>();
        s.AddSingleton<ApiService>();
        s.AddSingleton<AuthManager>();
        s.AddSingleton<MainWindow>();

        s.AddTransient<HomeViewModel>();
        s.AddTransient<LeaderboardViewModel>();
        s.AddTransient<ProfileViewModel>();
        s.AddTransient<SettingsViewModel>();
        s.AddTransient<LoginViewModel>();
        s.AddTransient<RegisterViewModel>();
        s.AddTransient<AdminUserListViewModel>();

        s.AddTransient<HomePage>();
        s.AddTransient<LeaderboardPage>();
        s.AddTransient<ProfilePage>();
        s.AddTransient<SettingsPage>();
        s.AddTransient<LoginPage>();
        s.AddTransient<RegisterPage>();
        s.AddTransient<AdminUserListPage>();
        return s.BuildServiceProvider();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = Services.GetRequiredService<MainWindow>();
        MainWindow.Activate();
    }
}
