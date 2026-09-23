using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MEIUtil.Models;
using MEIUtil.Services;

namespace MEIUtil.ViewModels;

public partial class TransactionFormViewModel(DatabaseService database) : ObservableObject
{
    public IReadOnlyList<string> Categories { get; } =
        ["Geral", "Serviços", "Produtos", "Materiais", "Transporte", "Marketing", "Taxas", "Outros"];

    [ObservableProperty] private string kind = TransactionTypes.Revenue;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private string category = "Geral";
    [ObservableProperty] private decimal amount;
    [ObservableProperty] private DateTime date = DateTime.Today;
    [ObservableProperty] private bool isPaid = true;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool isSaved;

    public string PageTitle => Kind == TransactionTypes.Expense ? "Nova despesa" : "Nova receita";
    public string PaymentLabel => Kind == TransactionTypes.Expense
        ? "Despesa já foi paga"
        : "Receita já foi recebida";

    partial void OnKindChanged(string value)
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(PaymentLabel));
    }

    public void ApplyQuery(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("kind", out var value))
            return;

        var requested = Uri.UnescapeDataString(value?.ToString() ?? TransactionTypes.Revenue);
        Kind = requested == TransactionTypes.Expense
            ? TransactionTypes.Expense
            : TransactionTypes.Revenue;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy || IsSaved)
            return;

        ErrorMessage = string.Empty;
        Description = Description.Trim();

        if (Description.Length < 2)
        {
            ErrorMessage = "Informe uma descrição válida.";
            return;
        }

        if (Amount <= 0)
        {
            ErrorMessage = "Informe um valor maior que zero.";
            return;
        }

        if (Date.Date > DateTime.Today)
        {
            ErrorMessage = "A data do lançamento não pode estar no futuro.";
            return;
        }

        if (!Categories.Contains(Category))
            Category = "Geral";

        IsBusy = true;
        try
        {
            await database.SaveTransactionAsync(new Transaction
            {
                Type = Kind,
                Description = Description,
                Category = Category,
                Amount = Math.Round(Amount, 2),
                Date = Date.Date,
                IsPaid = IsPaid
            });
            IsSaved = true;
        }
        catch
        {
            ErrorMessage = "Não foi possível salvar o lançamento. Tente novamente.";
            return;
        }
        finally
        {
            IsBusy = false;
        }

        try
        {
            await Shell.Current.GoToAsync("..");
        }
        catch
        {
            ErrorMessage = "Lançamento salvo. Não foi possível voltar automaticamente; use o botão Voltar.";
        }
    }
}
