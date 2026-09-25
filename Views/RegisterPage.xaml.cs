using DailyFortune.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace DailyFortune.WinUI.Views;

public sealed partial class RegisterPage : Page
{
    public RegisterViewModel ViewModel { get; }
    public RegisterPage()
    {
        ViewModel = App.Services.GetRequiredService<RegisterViewModel>();
        InitializeComponent();
        _ = ViewModel.CheckRegistrationOpenAsync();
    }
    private void GoLogin_Click(object sender, RoutedEventArgs e)
        => Frame.Navigate(typeof(LoginPage));
}
