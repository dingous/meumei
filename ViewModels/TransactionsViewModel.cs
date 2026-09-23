using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MEIUtil.Models;
using MEIUtil.Services;
using MEIUtil.Views;

namespace MEIUtil.ViewModels;

public partial class TransactionsViewModel(
    DatabaseService database) : ObservableObject
{
    public ObservableCollection<Transaction> Items { get; } = [];

    [ObservableProperty] private decimal totalRevenue;
    [ObservableProperty] private decimal totalExpenses;
    [ObservableProperty] private decimal balance;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string errorMessage = string.Empty;

    private bool _deletePromptOpen;

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            await LoadCoreAsync();
        }
        catch
        {
            ErrorMessage =
                "Não foi possível carregar os lançamentos.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadCoreAsync()
    {
        var today =
            DateTime.Today;

        var start =
            new DateTime(
                today.Year,
                today.Month,
                1);

        var data =
            await database.GetTransactionsAsync(
                start,
                start.AddMonths(1));

        Items.Clear();

        foreach (var item in data)
            Items.Add(item);

        TotalRevenue = data
            .Where(x =>
                x.Type == TransactionTypes.Revenue &&
                x.IsPaid &&
                x.Date.Date <= today)
            .Sum(x => x.Amount);

        TotalExpenses = data
            .Where(x =>
                x.Type == TransactionTypes.Expense &&
                x.IsPaid &&
                x.Date.Date <= today)
            .Sum(x => x.Amount);

        Balance =
            TotalRevenue - TotalExpenses;
    }

    [RelayCommand]
    private Task NewRevenueAsync()
        => Shell.Current.GoToAsync(
            $"{nameof(TransactionFormPage)}?kind={Uri.EscapeDataString(TransactionTypes.Revenue)}");

    [RelayCommand]
    private Task NewExpenseAsync()
        => Shell.Current.GoToAsync(
            $"{nameof(TransactionFormPage)}?kind={Uri.EscapeDataString(TransactionTypes.Expense)}");

    [RelayCommand]
    private async Task DeleteAsync(
        Transaction? item)
    {
        if (item is null ||
            IsBusy ||
            _deletePromptOpen)
        {
            return;
        }

        _deletePromptOpen = true;
        ErrorMessage = string.Empty;

        try
        {
            var shell =
                Shell.Current;

            if (shell is null)
            {
                ErrorMessage =
                    "Não foi possível abrir a confirmação de exclusão.";
                return;
            }

            var confirmed =
                await shell.DisplayAlertAsync(
                    "Excluir lançamento",
                    $"Deseja excluir “{item.Description}” no valor de {item.Amount:C2}?",
                    "Excluir",
                    "Cancelar");

            if (!confirmed)
                return;

            IsBusy = true;

            await database
                .DeleteTransactionAsync(item);

            await LoadCoreAsync();
        }
        catch
        {
            ErrorMessage =
                "Não foi possível excluir este lançamento.";
        }
        finally
        {
            IsBusy = false;
            _deletePromptOpen = false;
        }
    }
}
