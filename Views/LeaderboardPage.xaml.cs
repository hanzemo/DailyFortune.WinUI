using DailyFortune.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace DailyFortune.WinUI.Views;

public sealed partial class LeaderboardPage : Page
{
    public LeaderboardViewModel ViewModel { get; }
    private bool _loaded;

    public LeaderboardPage()
    {
        ViewModel = App.Services.GetRequiredService<LeaderboardViewModel>();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (_loaded) return;
        _loaded = true;
        await ViewModel.LoadAsync();
    }

    private void Period_Checked(object sender, RoutedEventArgs e)
    {
        if (!_loaded) return; // 初始化阶段忽略
        if (sender is RadioButton rb && rb.Tag is string tag && int.TryParse(tag, out var i))
            ViewModel.PeriodIndex = i;
    }
}
