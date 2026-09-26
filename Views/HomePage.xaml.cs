using DailyFortune.WinUI.Utilities;
using DailyFortune.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.ComponentModel;

namespace DailyFortune.WinUI.Views;

public sealed partial class HomePage : Page
{
    public HomeViewModel ViewModel { get; }

    public HomePage()
    {
        ViewModel = App.Services.GetRequiredService<HomeViewModel>();
        InitializeComponent();
        ViewModel.PropertyChanged += OnViewModelChanged;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadAsync();
        UpdateFortuneColor();
    }

    private void OnViewModelChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.Fortune))
            UpdateFortuneColor();
    }

    private void UpdateFortuneColor()
    {
        var fortune = ViewModel.Fortune?.Trim();

        if (string.IsNullOrEmpty(fortune))
        {
            FortuneBackground.Visibility = Visibility.Collapsed;
            var black = new SolidColorBrush(Colors.Black);
            FortuneText.Foreground = black;
            FortuneLabel.Foreground = black;
            CountdownText.Foreground = black;
            return;
        }

        FortuneBackground.Background = Constants.Brush(fortune);
        FortuneBackground.Visibility = Visibility.Visible;

        var white = new SolidColorBrush(Colors.White);
        FortuneText.Foreground = white;
        FortuneLabel.Foreground = white;
        CountdownText.Foreground = white;
    }
}
