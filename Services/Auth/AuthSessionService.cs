using System.Text.Json;

namespace MEIUtil.Services.Auth;

public sealed class AuthSessionService
{
    private const string SessionKey = "dingous_auth_session";

    private readonly SemaphoreSlim _storageGate = new(1, 1);
    private AuthSession? _cached;

    public async Task<AuthSession?> GetAsync()
    {
        await _storageGate.WaitAsync();

        try
        {
            if (_cached is not null)
            {
                if (!_cached.IsExpired)
                    return _cached;

                _cached = null;
                TryRemoveUnsafe();
                return null;
            }

            var raw =
                await SecureStorage.Default
                    .GetAsync(SessionKey);

            if (string.IsNullOrWhiteSpace(raw))
                return null;

            var session =
                JsonSerializer.Deserialize<AuthSession>(raw);

            if (session is null ||
                session.IsExpired)
            {
                _cached = null;
                TryRemoveUnsafe();
                return null;
            }

            _cached = session;
            return session;
        }
        catch
        {
            _cached = null;
            return null;
        }
        finally
        {
            _storageGate.Release();
        }
    }

    public async Task SaveAsync(
        AuthSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        if (session.IsExpired)
        {
            throw new InvalidOperationException(
                "Não é possível salvar uma sessão já expirada.");
        }

        await _storageGate.WaitAsync();

        try
        {
            await SecureStorage.Default.SetAsync(
                SessionKey,
                JsonSerializer.Serialize(session));

            _cached = session;
        }
        finally
        {
            _storageGate.Release();
        }
    }

    public async Task<bool> ClearAsync()
    {
        await _storageGate.WaitAsync();

        try
        {
            _cached = null;

            SecureStorage.Default.Remove(
                SessionKey);

            var remaining =
                await SecureStorage.Default
                    .GetAsync(SessionKey);

            return string.IsNullOrWhiteSpace(
                remaining);
        }
        catch
        {
            return false;
        }
        finally
        {
            _storageGate.Release();
        }
    }

    private static void TryRemoveUnsafe()
    {
        try
        {
            SecureStorage.Default.Remove(
                SessionKey);
        }
        catch
        {
            // A sessão expirada continua sendo rejeitada em memória.
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
    public bool IsExpired =>
        ExpiresAt <=
        DateTimeOffset.UtcNow.AddMinutes(1);
}
