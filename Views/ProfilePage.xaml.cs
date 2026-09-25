using DailyFortune.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace DailyFortune.WinUI.Views;

public sealed partial class ProfilePage : Page
{
    public ProfileViewModel ViewModel { get; }
    public ProfilePage()
    {
        ViewModel = App.Services.GetRequiredService<ProfileViewModel>();
        InitializeComponent();
    }
    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is string username) ViewModel.SetTarget(username);
        else ViewModel.SetTarget(null);
        await ViewModel.LoadAsync();
    }
}
