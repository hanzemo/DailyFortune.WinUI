using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DailyFortune.WinUI.Models;
using DailyFortune.WinUI.Services;
using DailyFortune.WinUI.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

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
        AppLog.Log("[Heatmap] 控件构造");
        Rebuild();
    }

    private static void OnHistoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FortuneHeatmapControl c) return;

        // 取消旧集合订阅
        if (e.OldValue is INotifyCollectionChanged oldColl)
            oldColl.CollectionChanged -= c.OnCollectionChanged;

        // 订阅新集合
        if (e.NewValue is INotifyCollectionChanged newColl)
            newColl.CollectionChanged += c.OnCollectionChanged;

        var count = (e.NewValue as IEnumerable<FortuneHistoryItem>)?.Count() ?? 0;
        AppLog.Log($"[Heatmap] History 属性变化, count={count}");
        c.Rebuild();
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var count = History?.Count() ?? 0;
        AppLog.Log($"[Heatmap] 集合内容变化 {e.Action}, count={count}");
        Rebuild();
    }

    private void Rebuild()
    {
        Cells.Clear();

        var historyCount = History?.Count() ?? 0;
        AppLog.Log($"[Heatmap] Rebuild 开始, History.Count={historyCount}");

        var byDate = new Dictionary<DateTime, string>();
        if (History != null)
        {
            foreach (var h in History)
            {
                var beijing = h.CreatedAt.AddHours(8);
                var key = beijing.Date;
                var val = (h.Value ?? "").Trim();
                byDate[key] = val;

                var found = Constants.HeatmapLevels.TryGetValue(val, out var lv);
                AppLog.Log($"[Heatmap] 数据: {key:yyyy-MM-dd} = [{val}] len={val.Length} 匹配={found} level={(found ? lv : 0)}");
            }
        }

        var today = DateTime.Now.AddHours(8).Date;
        var start = today.AddDays(-364);

        int colored = 0;
        for (var d = start; d <= today; d = d.AddDays(1))
        {
            var key = d.Date;
            var has = byDate.TryGetValue(key, out var value);
            var trimmed = (value ?? "").Trim();

            var level = 0;
            if (has && Constants.HeatmapLevels.TryGetValue(trimmed, out var lv))
                level = lv;

            SolidColorBrush brush;
            if (level == 0)
            {
                brush = new SolidColorBrush(Microsoft.UI.Colors.LightGray);
            }
            else
            {
                brush = Constants.Brush(trimmed);
                colored++;
            }

            Cells.Add(new HeatCell
            {
                Brush = brush,
                Tooltip = has ? $"{key:yyyy-MM-dd} {trimmed}" : $"{key:yyyy-MM-dd}"
            });
        }

        AppLog.Log($"[Heatmap] Rebuild 完成: 总={Cells.Count}, 有色={colored}");
    }
}
