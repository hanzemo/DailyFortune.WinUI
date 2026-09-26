using System.Collections.ObjectModel;

namespace DailyFortune.WinUI.Services;

public static class AppLog
{
    private static readonly object _lock = new();
    private const int MaxLines = 500;

    public static ObservableCollection<string> Lines { get; } = new();

    public static void Log(string msg)
    {
        var line = $"{DateTime.Now:HH:mm:ss.fff} {msg}";
        lock (_lock)
        {
            try
            {
                var path = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "DailyFortune.log");
                System.IO.File.AppendAllText(path, line + "\n");
            }
            catch { }

            try
            {
                if (Lines.Count > MaxLines) Lines.RemoveAt(0);
                Lines.Add(line);
            }
            catch { }
        }
    }

    public static string ReadAll()
    {
        try
        {
            var path = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DailyFortune.log");
            if (System.IO.File.Exists(path))
                return System.IO.File.ReadAllText(path);
        }
        catch (Exception ex) { return $"读取失败: {ex.Message}"; }
        return "(无日志)";
    }
}
