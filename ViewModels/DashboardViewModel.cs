using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MEIUtil.Models;
using MEIUtil.Services;
using MEIUtil.Services.Auth;
using MEIUtil.Views;

namespace MEIUtil.ViewModels;

public partial class DashboardViewModel(
    DatabaseService database,
    MeiRulesService rules,
    TrialService trial,
    SeedService seed,
    AuthSessionService authSession) : ObservableObject
{
    [ObservableProperty] private string greeting = "Olá!";
    [ObservableProperty] private string subtitle = "Seu MEI organizado em um só lugar.";
    [ObservableProperty] private decimal monthRevenue;
    [ObservableProperty] private decimal monthExpenses;
    [ObservableProperty] private decimal monthBalance;
    [ObservableProperty] private decimal annualRevenue;
    [ObservableProperty] private decimal annualLimit;
    [ObservableProperty] private decimal limitRemaining;
    [ObservableProperty] private decimal limitPercent;
    [ObservableProperty] private double limitProgress;
    [ObservableProperty] private decimal annualProjection;
    [ObservableProperty] private string riskText = "Dentro do limite";
    [ObservableProperty] private string nextObligation = "Nenhuma pendência encontrada";
    [ObservableProperty] private string nextObligationDate = string.Empty;
    [ObservableProperty] private string trialText = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy;

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            await seed.EnsureAsync();

            var today = DateTime.Today;
            var profile = await database.GetProfileAsync();
            var session = await authSession.GetAsync();
            var displayName = !string.IsNullOrWhiteSpace(session?.Name)
                ? session.Name
                : profile.OwnerName;

            Greeting = string.IsNullOrWhiteSpace(displayName)
                ? "Olá!"
                : $"Olá, {displayName.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]}!";

            Subtitle = session is null
                ? "Seu MEI organizado em um só lugar."
                : "Sua conta Dingous está conectada e protegida.";

            var monthStart = new DateTime(today.Year, today.Month, 1);
            var monthItems = await database.GetTransactionsAsync(monthStart, monthStart.AddMonths(1));

            MonthRevenue = monthItems
                .Where(x => x.Type == TransactionTypes.Revenue && x.IsPaid)
                .Sum(x => x.Amount);

            MonthExpenses = monthItems
                .Where(x => x.Type == TransactionTypes.Expense && x.IsPaid)
                .Sum(x => x.Amount);

            MonthBalance = MonthRevenue - MonthExpenses;

            var yearStart = new DateTime(today.Year, 1, 1);
            var yearItems = await database.GetTransactionsAsync(yearStart, yearStart.AddYears(1));

            AnnualRevenue = yearItems
                .Where(x => x.Type == TransactionTypes.Revenue && x.IsPaid)
                .Sum(x => x.Amount);

            AnnualLimit = rules.GetApplicableAnnualLimit(profile, today);
            LimitRemaining = Math.Max(0, AnnualLimit - AnnualRevenue);
            LimitPercent = AnnualLimit <= 0
                ? 0
                : Math.Round(AnnualRevenue / AnnualLimit * 100m, 1);

            LimitProgress = Math.Clamp((double)(LimitPercent / 100m), 0d, 1d);
            AnnualProjection = rules.GetProjection(AnnualRevenue, profile, today);
            RiskText = rules.GetRiskText(LimitPercent);

            NextObligation = "Nenhuma pendência encontrada";
            NextObligationDate = "Você está em dia no calendário local.";

            var obligations = await database.GetObligationsAsync();
            var next = obligations
                .Where(x => !x.IsDone && rules.IsObligationApplicable(x, profile))
                .OrderBy(x => x.DueDate)
                .FirstOrDefault();

            if (next is not null)
            {
                NextObligation = $"{next.Title} • {next.Reference}";
                NextObligationDate = next.DueDate.Date < today
                    ? $"Vencido em {next.DueDate:dd/MM/yyyy}"
                    : $"Vence em {next.DueDate:dd/MM/yyyy}";
            }

            var status = trial.GetStatus();
            TrialText = status.IsActive
                ? $"1º ano grátis • {status.DaysRemaining} dias restantes"
                : "Período gratuito encerrado • seus dados continuam disponíveis";
        }
        catch
        {
            ErrorMessage = "Não foi possível atualizar o painel agora. Seus dados locais continuam preservados.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task NewRevenueAsync()
        => Shell.Current.GoToAsync($"{nameof(TransactionFormPage)}?kind={Uri.EscapeDataString(TransactionTypes.Revenue)}");

    [RelayCommand]
    private Task NewExpenseAsync()
        => Shell.Current.GoToAsync($"{nameof(TransactionFormPage)}?kind={Uri.EscapeDataString(TransactionTypes.Expense)}");

    [RelayCommand]
    private Task NewQuoteAsync()
        => Shell.Current.GoToAsync(nameof(QuoteFormPage));
}
