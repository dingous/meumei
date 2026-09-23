using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MEIUtil.Models;
using MEIUtil.Services;
using MEIUtil.Views;

namespace MEIUtil.ViewModels;

public partial class TransactionsViewModel(DatabaseService database) : ObservableObject
{
    public ObservableCollection<Transaction> Items { get; } = [];
    [ObservableProperty] private decimal totalRevenue;
    [ObservableProperty] private decimal totalExpenses;
    [ObservableProperty] private decimal balance;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string errorMessage = string.Empty;

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy) return;
        await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var data = await database.GetTransactionsAsync(start, start.AddMonths(1));
            Items.Clear();
            foreach (var item in data)
                Items.Add(item);

            TotalRevenue = data
                .Where(x => x.Type == TransactionTypes.Revenue && x.IsPaid)
                .Sum(x => x.Amount);
            TotalExpenses = data
                .Where(x => x.Type == TransactionTypes.Expense && x.IsPaid)
                .Sum(x => x.Amount);
            Balance = TotalRevenue - TotalExpenses;
        }
        catch
        {
            ErrorMessage = "Não foi possível carregar os lançamentos.";
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
    private async Task DeleteAsync(Transaction? item)
    {
        if (item is null || IsBusy) return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            await database.DeleteTransactionAsync(item);
        }
        catch
        {
            ErrorMessage = "Não foi possível excluir este lançamento.";
            IsBusy = false;
            return;
        }

        IsBusy = false;
        await ReloadAsync();
    }
}
