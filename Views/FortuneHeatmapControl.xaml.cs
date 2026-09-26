using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.Collections.ObjectModel;
using DailyFortune.WinUI.Models;
using DailyFortune.WinUI.Services;
using DailyFortune.WinUI.Utilities;

namespace DailyFortune.WinUI.Views;

public class HeatCell
{
    public SolidColorBrush Brush { get; set; } = new(Microsoft.UI.Colors.LightGray);
    public string Tooltip { get; set; } = "";
}

public sealed partial class FortuneHeatmapControl : UserControl
{
    public ObservableCollection<HeatCell> Cells { get; } = new();

    public static readonly DependencyProperty HistoryProperty =
        DependencyProperty.Register(nameof(History), typeof(IEnumerable<FortuneHistoryItem>),
            typeof(FortuneHeatmapControl), new PropertyMetadata(null, OnHistoryChanged));

    public IEnumerable<FortuneHistoryItem>? History
    {
        get => (IEnumerable<FortuneHistoryItem>?)GetValue(HistoryProperty);
        set => SetValue(HistoryProperty, value);
    }

    public FortuneHeatmapControl()
    {
        InitializeComponent();
        Rebuild();
    }

    private static void OnHistoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FortuneHeatmapControl c) c.Rebuild();
    }

    private void Rebuild()
    {
        Cells.Clear();
        var dict = new Dictionary<DateTime, string>();
        if (History != null)
        {
            foreach (var h in History)
            {
                var key = h.CreatedAt.Date;
                if (!dict.ContainsKey(key) || dict[key].Length == 0)
                    dict[key] = h.Value ?? "";
            }
        }

        var start = DateTime.Today.AddDays(-364);
        for (var d = start; d <= DateTime.Today; d = d.AddDays(1))
        {
            var key = d.Date;
            var has = dict.TryGetValue(key, out var value);
            var trimmed = (value ?? "").Trim();
            var level = has && Constants.HeatmapLevels.TryGetValue(trimmed, out var lv) ? lv : 0;
            var brush = level == 0
                ? new SolidColorBrush(Microsoft.UI.Colors.LightGray)
                : Constants.Brush(trimmed);

            Cells.Add(new HeatCell
            {
                Brush = brush,
                Tooltip = has ? $"{key:yyyy-MM-dd} {trimmed}" : $"{key:yyyy-MM-dd}"
            });
        }
    }
}
