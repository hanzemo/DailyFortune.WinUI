using Windows.Security.Credentials;

namespace DailyFortune.WinUI.Services;

public class TokenStore
{
    private const string Resource = "DailyFortune";
    private readonly PasswordVault _vault = new();

    public void SaveAccessToken(string t) => Save("access", t);
    public void SaveRefreshToken(string t) => Save("refresh", t);
    public string? GetAccessToken() => Get("access");
    public string? GetRefreshToken() => Get("refresh");

    private void Save(string key, string value)
    {
        try { _vault.Remove(_vault.Retrieve(Resource, key)); } catch { }
        _vault.Add(new PasswordCredential(Resource, key, value));
    }

    private string? Get(string key)
    {
        try { return _vault.Retrieve(Resource, key).Password; }
        catch { return null; }
    }

    public void ClearAll()
    {
        try
        {
            foreach (var c in _vault.FindAllByResource(Resource))
                _vault.Remove(c);
        }
        catch { }
    }
}
