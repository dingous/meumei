#if ANDROID
using Android.Gms.Common.Util.Concurrent;
using Android.OS;
using AndroidX.Credentials;
using AndroidX.Credentials.Exceptions;
using Java.Interop;
using MEIUtil.Services.Auth;
using Xamarin.GoogleAndroid.Libraries.Identity.GoogleId;
using JObject = Java.Lang.Object;

namespace MEIUtil.Platforms.Android.Services;

public sealed class AndroidGoogleNativeAuthService : JObject, IGoogleNativeAuthService, ICredentialManagerCallback
{
    private const string ServerClientId = "988830369120-953qsrti521ub2tki51cdb3m0hu707sm.apps.googleusercontent.com";
    private readonly object _gate = new();
    private TaskCompletionSource<GoogleNativeCredential>? _pending;
    private CancellationTokenRegistration _cancellationRegistration;

    public bool IsSupported => true;

    public Task<GoogleNativeCredential> SignInAsync(CancellationToken cancellationToken = default)
    {
        TaskCompletionSource<GoogleNativeCredential> pending;
        lock (_gate)
        {
            if (_pending is not null)
                throw new InvalidOperationException("Já existe uma tentativa de login em andamento.");
            pending = new TaskCompletionSource<GoogleNativeCredential>(TaskCreationOptions.RunContinuationsAsynchronously);
            _pending = pending;
            if (cancellationToken.CanBeCanceled)
                _cancellationRegistration = cancellationToken.Register(() => CompleteCanceled(cancellationToken));
        }

        try
        {
            var option = new GetSignInWithGoogleOption.Builder(ServerClientId).Build();
            var request = new GetCredentialRequest.Builder().AddCredentialOption(option).Build();
            var looper = Looper.MainLooper ?? throw new InvalidOperationException("Looper principal do Android indisponível.");
            var activity = Platform.CurrentActivity ?? throw new InvalidOperationException("Activity Android indisponível para abrir o login Google.");
            CredentialManager.Create(activity).GetCredentialAsync(activity, request, null, new HandlerExecutor(looper), this);
        }
        catch (System.Exception ex) { CompleteException(ex); }

        return pending.Task;
    }

    public void OnResult(JObject? result)
    {
        try
        {
            if (result is null || !result.TryJavaCast(out GetCredentialResponse? response) || response is null)
                throw new InvalidOperationException("O Google retornou uma credencial desconhecida.");
            var google = GoogleIdTokenCredential.CreateFrom(response.Credential.Data);
            if (string.IsNullOrWhiteSpace(google.IdToken))
                throw new InvalidOperationException("O Google não retornou um ID Token válido.");
            CompleteSuccess(new GoogleNativeCredential(google.IdToken, google.Id ?? string.Empty, google.DisplayName ?? string.Empty));
        }
        catch (System.Exception ex) { CompleteException(ex); }
    }

    public void OnError(JObject error)
    {
        try
        {
            if (error.TryJavaCast(out GetCredentialException? exception) && exception is not null)
            {
                switch (exception)
                {
                    case GetCredentialCancellationException:
                    case GetCredentialInterruptedException:
                        CompleteCanceled(CancellationToken.None); return;
                    case NoCredentialException:
                        CompleteException(new InvalidOperationException("Nenhuma conta Google disponível neste aparelho.")); return;
                    case GetCredentialProviderConfigurationException:
                        CompleteException(new InvalidOperationException("O login Google deste aplicativo ainda não está configurado corretamente.")); return;
                    default:
                        CompleteException(new InvalidOperationException("Não foi possível concluir o login Google.", exception)); return;
                }
            }
            CompleteException(new InvalidOperationException("Não foi possível concluir o login Google."));
        }
        catch (System.Exception ex) { CompleteException(ex); }
    }

    private void CompleteSuccess(GoogleNativeCredential credential)
    {
        TaskCompletionSource<GoogleNativeCredential>? pending;
        lock (_gate) { pending = _pending; _pending = null; _cancellationRegistration.Dispose(); }
        pending?.TrySetResult(credential);
    }

    private void CompleteException(System.Exception exception)
    {
        TaskCompletionSource<GoogleNativeCredential>? pending;
        lock (_gate) { pending = _pending; _pending = null; _cancellationRegistration.Dispose(); }
        pending?.TrySetException(exception);
    }

    private void CompleteCanceled(CancellationToken cancellationToken)
    {
        TaskCompletionSource<GoogleNativeCredential>? pending;
        lock (_gate) { pending = _pending; _pending = null; _cancellationRegistration.Dispose(); }
        if (cancellationToken.CanBeCanceled) pending?.TrySetCanceled(cancellationToken); else pending?.TrySetCanceled();
    }
}
#endif
