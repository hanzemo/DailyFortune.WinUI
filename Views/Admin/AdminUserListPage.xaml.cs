using DailyFortune.WinUI.Models;
using DailyFortune.WinUI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace DailyFortune.WinUI.Views.Admin;

public sealed partial class AdminUserListPage : Page
{
    public AdminUserListViewModel ViewModel { get; }

    public AdminUserListPage()
    {
        ViewModel = App.Services.GetRequiredService<AdminUserListViewModel>();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await ViewModel.LoadAsync();
    }

    private void User_Click(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not UserMeProfile user) return;
        var flyout = BuildFlyout(user);
        flyout.ShowAt((FrameworkElement)sender);
    }

    private MenuFlyout BuildFlyout(UserMeProfile user)
    {
        var f = new MenuFlyout();

        var status = new MenuFlyoutItem { Text = user.Status == "active" ? "禁用账号" : "启用账号" };
        status.Click += async (_, _) => await ViewModel.ToggleStatusCommand.ExecuteAsync(user);
        f.Items.Add(status);

        var hide = new MenuFlyoutItem { Text = user.IsHidden ? "取消隐藏" : "隐藏用户" };
        hide.Click += async (_, _) => await ViewModel.ToggleVisibilityCommand.ExecuteAsync(user);
        f.Items.Add(hide);

        var edit = new MenuFlyoutItem { Text = "编辑资料" };
        edit.Click += async (_, _) => await ShowEditDialogAsync(user);
        f.Items.Add(edit);

        if (user.Role != "admin")
        {
            var p = new MenuFlyoutItem { Text = "设为管理员" };
            p.Click += async (_, _) => await ViewModel.SetRoleCommand.ExecuteAsync(new RoleChange(user, "admin"));
            f.Items.Add(p);
        }
        else
        {
            var d = new MenuFlyoutItem { Text = "设为普通用户" };
            d.Click += async (_, _) => await ViewModel.SetRoleCommand.ExecuteAsync(new RoleChange(user, "user"));
            f.Items.Add(d);
        }

        var reset = new MenuFlyoutItem { Text = "重置密码" };
        reset.Click += async (_, _) => await ShowResetPasswordDialogAsync(user);
        f.Items.Add(reset);

        f.Items.Add(new MenuFlyoutSeparator());

        var del = new MenuFlyoutItem { Text = "删除用户" };
        del.Click += async (_, _) => await ConfirmAndDeleteAsync(user);
        f.Items.Add(del);

        return f;
    }

    private async Task ShowEditDialogAsync(UserMeProfile user)
    {
        var dialog = new AdminEditUserDialog(user) { XamlRoot = XamlRoot };
        if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            await ViewModel.SaveUserCommand.ExecuteAsync(new UserEdit(user, dialog.Payload));
    }

    private async Task ShowResetPasswordDialogAsync(UserMeProfile user)
    {
        var input = new PasswordBox { Header = "新密码", Width = 300 };
        var dlg = new ContentDialog
        {
            Title = $"重置 {user.Username} 的密码",
            Content = input,
            PrimaryButtonText = "确定",
            CloseButtonText = "取消",
            XamlRoot = XamlRoot
        };
        if (await dlg.ShowAsync() == ContentDialogResult.Primary)
            await ViewModel.ResetPasswordCommand.ExecuteAsync(new PasswordReset(user, input.Password));
    }

    private async Task ConfirmAndDeleteAsync(UserMeProfile user)
    {
        var dlg = new ContentDialog
        {
            Title = "确认删除",
            Content = $"确定删除用户 {user.Username} 吗？此操作不可撤销。",
            PrimaryButtonText = "删除",
            CloseButtonText = "取消",
            XamlRoot = XamlRoot
        };
        if (await dlg.ShowAsync() == ContentDialogResult.Primary)
            await ViewModel.DeleteUserCommand.ExecuteAsync(user);
    }
}
