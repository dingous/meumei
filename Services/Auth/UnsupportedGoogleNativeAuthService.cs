namespace MEIUtil.Services.Auth;

public sealed class UnsupportedGoogleNativeAuthService : IGoogleNativeAuthService
{
    public bool IsSupported => false;

    public Task<GoogleNativeCredential> SignInAsync(
        CancellationToken cancellationToken = default)
        => Task.FromException<GoogleNativeCredential>(
            new PlatformNotSupportedException(
                "O login Google nativo está disponível no Android."));

    public Task SignOutAsync(
        CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
