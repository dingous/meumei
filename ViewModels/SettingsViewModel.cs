using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MEIUtil.Models;
using MEIUtil.Services;
using MEIUtil.Services.Auth;

namespace MEIUtil.ViewModels;

public partial class SettingsViewModel(
    DatabaseService database,
    TrialService trial,
    IGoogleNativeAuthService googleAuth,
    DingousAuthApi dingousAuth,
    AuthSessionService authSession) : ObservableObject
{
    [ObservableProperty] private MeiProfile profile = new();
    [ObservableProperty] private string trialTitle = "Primeiro ano grátis";
    [ObservableProperty] private string trialText = string.Empty;
    [ObservableProperty] private string message = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool isGoogleLoginAvailable;
    [ObservableProperty] private bool isLoggedIn;

    public bool IsLoggedOut => !IsLoggedIn;

    [ObservableProperty] private string accountName = string.Empty;
    [ObservableProperty] private string accountEmail = string.Empty;
    [ObservableProperty] private string accountInitials = "ME";

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            Profile =
                await database.GetProfileAsync();

            var status =
                trial.GetStatus();

            TrialTitle =
                status.IsActive
                    ? "Primeiro ano grátis"
                    : "Período gratuito encerrado";

            TrialText =
                status.IsActive
                    ? $"Grátis até {status.ExpiresAtUtc.ToLocalTime():dd/MM/yyyy} • {status.DaysRemaining} dias restantes"
                    : "Seus dados locais continuam disponíveis.";

            IsGoogleLoginAvailable =
                googleAuth.IsSupported;

            await RefreshSessionAsync();
        }
        catch
        {
            ErrorMessage =
                "Não foi possível carregar seus dados agora.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy)
            return;

        Message = string.Empty;
        ErrorMessage = string.Empty;

        Profile.OwnerName =
            Profile.OwnerName.Trim();

        Profile.BusinessName =
            Profile.BusinessName.Trim();

        Profile.Cnpj =
            BrazilianDocumentValidator.Normalize(
                Profile.Cnpj);

        if (Profile.OpenedAt.Date >
            DateTime.Today)
        {
            ErrorMessage =
                "A data de abertura não pode estar no futuro.";
            return;
        }

        if (Profile.AnnualRevenueLimit <= 0)
        {
            ErrorMessage =
                "Informe um limite anual maior que zero.";
            return;
        }

        if (!string.IsNullOrWhiteSpace(Profile.Cnpj) &&
            !BrazilianDocumentValidator.IsValidCnpj(
                Profile.Cnpj))
        {
            ErrorMessage =
                "Informe um CNPJ válido. O formato alfanumérico de 14 posições também é aceito.";
            return;
        }

        IsBusy = true;

        try
        {
            await database.SaveProfileAsync(
                Profile);

            Message =
                "Dados salvos com segurança.";
        }
        catch
        {
            ErrorMessage =
                "Não foi possível salvar. Tente novamente.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SignInWithGoogleAsync()
    {
        if (IsBusy)
            return;

        ErrorMessage = string.Empty;
        Message = string.Empty;
        IsBusy = true;

        try
        {
            var google =
                await googleAuth.SignInAsync();

            var session =
                await dingousAuth
                    .ExchangeGoogleTokenAsync(
                        google.IdToken);

            await authSession.SaveAsync(
                session);

            await RefreshSessionAsync();

            Message =
                "Conta Google conectada ao Dingous com sucesso.";

            if (string.IsNullOrWhiteSpace(Profile.OwnerName) &&
                !string.IsNullOrWhiteSpace(session.Name))
            {
                Profile.OwnerName =
                    session.Name;

                try
                {
                    await database.SaveProfileAsync(
                        Profile);
                }
                catch
                {
                    // Login concluído; falha ao preencher nome não invalida a sessão.
                }
            }
        }
        catch (OperationCanceledException)
        {
            Message =
                "Login cancelado.";
        }
        catch (Exception ex)
        {
            ErrorMessage =
                FriendlyAuthError(
                    ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SignOutAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        Message = string.Empty;
        ErrorMessage = string.Empty;

        try
        {
            var localStateCleared =
                await authSession.ClearAsync();

            var nativeStateCleared =
                true;

            try
            {
                await googleAuth.SignOutAsync();
            }
            catch
            {
                nativeStateCleared = false;
            }

            await RefreshSessionAsync();

            if (IsLoggedIn ||
                !localStateCleared)
            {
                ErrorMessage =
                    "Não foi possível encerrar a sessão local com segurança. Tente sair novamente.";
                return;
            }

            Message =
                nativeStateCleared
                    ? "Você saiu da conta Dingous neste aparelho."
                    : "Sessão Dingous encerrada. O Google pode manter a conta anterior sugerida no próximo login.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RefreshSessionAsync()
    {
        var session =
            await authSession.GetAsync();

        IsLoggedIn =
            session is not null;

        AccountName =
            session?.Name ??
            string.Empty;

        AccountEmail =
            session?.Email ??
            string.Empty;

        AccountInitials =
            Initials(
                session?.Name,
                session?.Email);
    }

    partial void OnIsLoggedInChanged(
        bool value)
        => OnPropertyChanged(
            nameof(IsLoggedOut));

    private static string FriendlyAuthError(
        string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "Não foi possível entrar com o Google.";

        if (raw.Contains(
                "503",
                StringComparison.OrdinalIgnoreCase) ||
            raw.Contains(
                "não foi configurado",
                StringComparison.OrdinalIgnoreCase))
        {
            return "O login Google ainda não está habilitado no DingousChatTrade de produção.";
        }

        if (raw.Contains(
                "network",
                StringComparison.OrdinalIgnoreCase) ||
            raw.Contains(
                "internet",
                StringComparison.OrdinalIgnoreCase) ||
            raw.Contains(
                "conectar",
                StringComparison.OrdinalIgnoreCase))
        {
            return "Sem conexão com o DingousChatTrade. Verifique sua internet.";
        }

        return raw.Length <= 180
            ? raw
            : "Não foi possível concluir o login Google agora.";
    }

    private static string Initials(
        string? name,
        string? email)
    {
        var source =
            string.IsNullOrWhiteSpace(name)
                ? email ?? string.Empty
                : name;

        var parts =
            source.Split(
                new[] { ' ', '@', '.', '_', '-' },
                StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
            return "ME";

        if (parts.Length == 1)
        {
            return parts[0]
                [..Math.Min(
                    2,
                    parts[0].Length)]
                .ToUpperInvariant();
        }

        return
            $"{parts[0][0]}{parts[^1][0]}"
            .ToUpperInvariant();
    }
}
