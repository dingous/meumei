using System.Text.Json;

namespace MEIUtil.Services.Auth;

public sealed class AuthSessionService
{
    private const string SessionKey = "dingous_auth_session";
    private AuthSession? _cached;

    public async Task<AuthSession?> GetAsync()
    {
        if (_cached is not null)
        {
            if (!_cached.IsExpired)
                return _cached;

            Clear();
            return null;
        }

        try
        {
            var raw = await SecureStorage.Default.GetAsync(SessionKey);
            if (string.IsNullOrWhiteSpace(raw))
                return null;

            var session = JsonSerializer.Deserialize<AuthSession>(raw);
            if (session is null || session.IsExpired)
            {
                Clear();
                return null;
            }

            _cached = session;
            return session;
        }
        catch
        {
            Clear();
            return null;
        }
    }

    public async Task SaveAsync(AuthSession session)
    {
        if (session.IsExpired)
            throw new InvalidOperationException("Não é possível salvar uma sessão já expirada.");

        await SecureStorage.Default.SetAsync(SessionKey, JsonSerializer.Serialize(session));
        _cached = session;
    }

    public void Clear()
    {
        _cached = null;
        try
        {
            SecureStorage.Default.Remove(SessionKey);
        }
        catch
        {
            // O cache em memória já foi invalidado. Falhas do cofre do SO não devem derrubar a UI.
        }
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
