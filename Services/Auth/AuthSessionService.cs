using System.Text.Json;

namespace MEIUtil.Services.Auth;

public sealed class AuthSessionService
{
    private const string SessionKey = "dingous_auth_session";
    private AuthSession? _cached;

    public async Task<AuthSession?> GetAsync()
    {
        if (_cached is not null)
            return _cached.IsExpired ? null : _cached;

        try
        {
            var raw = await SecureStorage.Default.GetAsync(SessionKey);
            if (string.IsNullOrWhiteSpace(raw))
                return null;

            var session = JsonSerializer.Deserialize<AuthSession>(raw);
            if (session is null || session.IsExpired)
            {
                SecureStorage.Default.Remove(SessionKey);
                return null;
            }

            _cached = session;
            return session;
        }
        catch
        {
            try { SecureStorage.Default.Remove(SessionKey); } catch { }
            _cached = null;
            return null;
        }
    }

    public async Task SaveAsync(AuthSession session)
    {
        _cached = session;
        await SecureStorage.Default.SetAsync(SessionKey, JsonSerializer.Serialize(session));
    }

    public void Clear()
    {
        _cached = null;
        SecureStorage.Default.Remove(SessionKey);
    }
}

public sealed record AuthSession(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    string Name,
    string Email,
    string PictureUrl)
{
    public bool IsExpired => ExpiresAt <= DateTimeOffset.UtcNow.AddMinutes(1);
}
