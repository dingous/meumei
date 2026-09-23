using System.Net.Http.Json;
using System.Text.Json;

namespace MEIUtil.Services.Auth;

public sealed class DingousAuthApi(HttpClient httpClient)
{
    private const long IdentityOnlyCompanyId = 0;

    public async Task<AuthSession> ExchangeGoogleTokenAsync(
        string idToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(idToken))
            throw new InvalidOperationException("O Google não retornou uma credencial válida.");

        try
        {
            using var response = await httpClient.PostAsJsonAsync(
                "api/auth/google-game",
                new GoogleGameLoginRequest(idToken, IdentityOnlyCompanyId),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var detail = await ReadSafeErrorAsync(response, cancellationToken);
                throw new InvalidOperationException(detail);
            }

            var payload = await response.Content.ReadFromJsonAsync<GoogleGameLoginResponse>(
                cancellationToken: cancellationToken)
                ?? throw new InvalidOperationException(
                    "O DingousChatTrade retornou uma resposta de login vazia.");

            if (string.IsNullOrWhiteSpace(payload.AccessToken))
                throw new InvalidOperationException(
                    "O DingousChatTrade não retornou um token de acesso válido.");

            return new AuthSession(
                payload.AccessToken,
                payload.ExpiresAt,
                payload.PlayerName ?? string.Empty,
                payload.Email ?? string.Empty,
                payload.PictureUrl ?? string.Empty);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new InvalidOperationException(
                "O DingousChatTrade demorou demais para responder. Tente novamente.");
        }
        catch (HttpRequestException)
        {
            throw new InvalidOperationException(
                "Não foi possível conectar ao DingousChatTrade. Verifique sua internet.");
        }
    }

    private static async Task<string> ReadSafeErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        try
        {
            var raw = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(raw))
                return $"Não foi possível entrar. O servidor respondeu HTTP {(int)response.StatusCode}.";

            using var json = JsonDocument.Parse(raw);
            var root = json.RootElement;

            foreach (var key in new[] { "error", "detail", "title" })
            {
                if (root.TryGetProperty(key, out var value) &&
                    value.ValueKind == JsonValueKind.String)
                {
                    var message = value.GetString();
                    if (!string.IsNullOrWhiteSpace(message))
                        return message;
                }
            }
        }
        catch
        {
        }

        return $"Não foi possível entrar. O servidor respondeu HTTP {(int)response.StatusCode}.";
    }

    private sealed record GoogleGameLoginRequest(string IdToken, long CompanyId);

    private sealed record GoogleGameLoginResponse(
        string AccessToken,
        DateTimeOffset ExpiresAt,
        string? PlayerName,
        string? Email,
        string? PictureUrl);
}
