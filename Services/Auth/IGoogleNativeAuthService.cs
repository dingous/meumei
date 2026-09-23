namespace MEIUtil.Services.Auth;

public interface IGoogleNativeAuthService
{
    bool IsSupported { get; }
    Task<GoogleNativeCredential> SignInAsync(CancellationToken cancellationToken = default);
}

public sealed record GoogleNativeCredential(string IdToken, string Email, string DisplayName);
