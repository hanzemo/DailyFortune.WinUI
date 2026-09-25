namespace DailyFortune.WinUI.Services;

public static class FortuneUtils
{
    private static readonly string[] Good = { "諭吉", "大吉", "吉", "中吉", "小吉" };
    private static readonly string[] Bad = { "凶", "大凶" };
    private static readonly Random Rng = new();

    public static string DrawLocally()
        => Rng.NextDouble() <= 0.8 ? Good[Rng.Next(Good.Length)] : Bad[Rng.Next(Bad.Length)];
}
