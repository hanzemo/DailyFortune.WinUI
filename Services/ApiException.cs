namespace DailyFortune.WinUI.Services;

public class ApiException : Exception
{
    public int StatusCode { get; }
    public ApiException(int code, string message) : base(message) => StatusCode = code;
}
