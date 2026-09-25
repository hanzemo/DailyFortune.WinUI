using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DailyFortune.WinUI.Models;
using DailyFortune.WinUI.Utilities;

namespace DailyFortune.WinUI.Services;

public enum LeaderboardPeriod { today, week, month, year }

public class ApiService
{
    private const string BaseUrl = "http://186.241.81.212:8000";
    private readonly HttpClient _http;
    private readonly TokenStore _tokens;
    private readonly JsonSerializerOptions _json;

    public ApiService(TokenStore tokens)
    {
        _tokens = tokens;
        _http = new HttpClient { BaseAddress = new Uri(BaseUrl), Timeout = TimeSpan.FromSeconds(30) };
        _json = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new FlexibleDateTimeConverter(),
                new FlexibleNullableDateTimeConverter(),
                new JsonStringEnumConverter()
            }
        };
    }

    private async Task<T> RequestAsync<T>(string endpoint, HttpMethod method, object? body = null, bool auth = true)
    {
        var req = new HttpRequestMessage(method, endpoint);
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        if (auth && _tokens.GetAccessToken() is { } token)
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body != null)
            req.Content = new StringContent(JsonSerializer.Serialize(body, _json), Encoding.UTF8, "application/json");

        var resp = await _http.SendAsync(req);
        var data = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
            throw new ApiException((int)resp.StatusCode, ExtractError(data, resp.StatusCode));
        return JsonSerializer.Deserialize<T>(data, _json) ?? throw new ApiException(0, "空响应");
    }

    private async Task RequestNoContentAsync(string endpoint, HttpMethod method, object? body = null, bool auth = true)
    {
        var req = new HttpRequestMessage(method, endpoint);
        if (auth && _tokens.GetAccessToken() is { } token)
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        if (body != null)
            req.Content = new StringContent(JsonSerializer.Serialize(body, _json), Encoding.UTF8, "application/json");

        var resp = await _http.SendAsync(req);
        if (!resp.IsSuccessStatusCode)
        {
            var data = await resp.Content.ReadAsStringAsync();
            throw new ApiException((int)resp.StatusCode, ExtractError(data, resp.StatusCode));
        }
    }

    private string ExtractError(string data, int code)
    {
        var msg = $"服务器错误: {code}";
        try
        {
            var e = JsonSerializer.Deserialize<APIErrorResponse>(data, _json);
            if (!string.IsNullOrEmpty(e?.Detail)) msg = e!.Detail!;
        }
        catch { }
        return msg;
    }

    // ---- Auth ----
    public Task<RegistrationStatusResponse> GetRegistrationStatusAsync()
        => RequestAsync<RegistrationStatusResponse>("/config/registration-status", HttpMethod.Get, auth: false);

    public async Task<AuthResponse> LoginAsync(string username, string password)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["username"] = username,
            ["password"] = password
        });
        var resp = await _http.PostAsync("/auth/login", content);
        var data = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
            throw new ApiException((int)resp.StatusCode, ExtractError(data, (int)resp.StatusCode));
        return JsonSerializer.Deserialize<AuthResponse>(data, _json)!;
    }

    public Task<AuthResponse> RegisterAsync(string username, string email, string password)
        => RequestAsync<AuthResponse>("/auth/register", HttpMethod.Post,
            new { username, email, password }, auth: false);

    public async Task<(string access, string refresh)> RefreshTokenAsync(string refreshToken)
    {
        var r = await RequestAsync<AuthResponse>("/auth/refresh", HttpMethod.Post,
            new { refresh_token = refreshToken }, auth: false);
        return (r.AccessToken, r.RefreshToken);
    }

    // ---- Users ----
    public Task<MyProfileResponse> GetMyProfileAsync() => RequestAsync<MyProfileResponse>("/users/me", HttpMethod.Get);
    public Task<UserPublicProfile> GetUserProfileAsync(string username) => RequestAsync<UserPublicProfile>($"/users/u/{username}", HttpMethod.Get);
    public Task<MyProfileResponse> UpdateMyProfileAsync(UserUpdatePayload p) => RequestAsync<MyProfileResponse>("/users/me", HttpMethod.Patch, p);
    public Task ChangePasswordAsync(string cur, string nw) => RequestNoContentAsync("/users/me/password", HttpMethod.Patch, new { current_password = cur, new_password = nw });
    public Task<List<FortuneHistoryItem>> GetUserFortuneHistoryAsync(string username) => RequestAsync<List<FortuneHistoryItem>>($"/users/u/{username}/fortune-history", HttpMethod.Get);
    public Task DeleteMyAccountAsync() => RequestNoContentAsync("/users/me", HttpMethod.Delete);

    // ---- Fortune ----
    public Task<FortuneDrawResponse> DrawFortuneAsync() => RequestAsync<FortuneDrawResponse>("/fortune/draw", HttpMethod.Post);
    public Task<List<LeaderboardGroup>> GetLeaderboardAsync(LeaderboardPeriod p = LeaderboardPeriod.today)
        => RequestAsync<List<LeaderboardGroup>>($"/fortune/leaderboard?period={p}", HttpMethod.Get);

    // ---- Admin ----
    public Task<List<UserMeProfile>> GetAllUsersAsync() => RequestAsync<List<UserMeProfile>>("/admin/users", HttpMethod.Get);
    public Task AdminUpdateUserAsync(string id, UserUpdatePayload p) => RequestNoContentAsync($"/admin/users/{id}", HttpMethod.Patch, p);
    public Task AdminUpdateRoleAsync(string id, string role) => RequestNoContentAsync($"/admin/users/{id}/role", HttpMethod.Post, new { role });
    public Task AdminDeleteUserAsync(string id) => RequestNoContentAsync($"/admin/users/{id}", HttpMethod.Delete);
    public Task AdminResetPasswordAsync(string id, string nw) => RequestNoContentAsync($"/admin/users/{id}/reset-password", HttpMethod.Post, new { new_password = nw });
    public Task UpdateUserStatusAsync(string id, string status) => RequestNoContentAsync($"/admin/users/{id}/status", HttpMethod.Post, new { status });
    public Task UpdateUserVisibilityAsync(string id, bool isHidden) => RequestNoContentAsync($"/admin/users/{id}/visibility", HttpMethod.Post, new { is_hidden = isHidden });
    public Task UpdateUserTagsAsync(string id, List<string> tags) => RequestNoContentAsync($"/admin/users/{id}/tags", HttpMethod.Post, new { tags });
}
