using DailyFortune.WinUI.Models;
using Microsoft.UI.Xaml.Controls;

namespace DailyFortune.WinUI.Views.Admin;

public sealed partial class AdminEditUserDialog : ContentDialog
{
    private readonly UserMeProfile _user;
    public UserUpdatePayload Payload { get; private set; } = new();

    public AdminEditUserDialog(UserMeProfile user)
    {
        _user = user;
        InitializeComponent();
        DisplayNameBox.Text = user.DisplayName;
        BioBox.Text = user.Bio;
        AvatarBox.Text = user.AvatarUrl;
        QqBox.Text = user.Qq?.ToString() ?? "";
    }

    private void OnSave(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var p = new UserUpdatePayload();
        if (DisplayNameBox.Text != _user.DisplayName) p.DisplayName = DisplayNameBox.Text;
        if (BioBox.Text != _user.Bio) p.Bio = BioBox.Text;
        if (AvatarBox.Text != _user.AvatarUrl) p.AvatarUrl = AvatarBox.Text;
        var qq = long.TryParse(QqBox.Text, out var v) ? v : (long?)null;
        if (qq != _user.Qq) p.Qq = qq;
        Payload = p;
    }
}
