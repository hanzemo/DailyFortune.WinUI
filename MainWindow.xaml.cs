using DailyFortune.WinUI.Services;
using DailyFortune.WinUI.Views;
using DailyFortune.WinUI.Views.Admin;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DailyFortune.WinUI;

public sealed partial class MainWindow : Window
{
    private readonly AuthManager _auth;
    private bool _initialized;

    public MainWindow(AuthManager auth)
    {
        _auth = auth;
        InitializeComponent();
        // InitializeComponent 之后再订阅，确保 DispatcherQueue 可用
        _auth.PropertyChanged += OnAuthChanged;
        Activated += OnFirstActivated;
    }

    private async void OnFirstActivated(object sender, WindowActivatedEventArgs e)
    {
        if (_initialized) return;
        _initialized = true;
        Activated -= OnFirstActivated;
        await _auth.InitializeAsync();
        UpdateRoot();
    }

    private void OnAuthChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var dq = DispatcherQueue;
        if (dq is null) return;
        dq.TryEnqueue(UpdateRoot);
    }

    private void UpdateRoot()
    {
        if (_auth.IsLoading)
        {
            SplashPanel.Visibility = Visibility.Visible;
            AuthFrame.Visibility = Visibility.Collapsed;
            NavView.Visibility = Visibility.Collapsed;
            return;
        }
        SplashPanel.Visibility = Visibility.Collapsed;

        if (!_auth.IsAuthenticated)
        {
            AuthFrame.Visibility = Visibility.Visible;
            NavView.Visibility = Visibility.Collapsed;
            if (AuthFrame.Content is null)
                AuthFrame.Navigate(typeof(LoginPage));
            return;
        }

        AuthFrame.Visibility = Visibility.Collapsed;
        NavView.Visibility = Visibility.Visible;
        AdminNavItem.Visibility = _auth.CurrentUser?.Role == "admin"
            ? Visibility.Visible : Visibility.Collapsed;

        if (ContentFrame.Content is null)
        {
            NavView.SelectedItem = NavView.MenuItems[0];
            ContentFrame.Navigate(typeof(HomePage));
        }
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is not NavigationViewItem item) return;
        Type? page = item.Tag?.ToString() switch
        {
            "home" => typeof(HomePage),
            "leaderboard" => typeof(LeaderboardPage),
            "profile" => typeof(ProfilePage),
            "settings" => typeof(SettingsPage),
            "admin" => typeof(AdminUserListPage),
            _ => null
        };
        if (page != null && ContentFrame.CurrentSourcePageType != page)
            ContentFrame.Navigate(page);
    }
}
