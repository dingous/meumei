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
        IsBusy = true;
        ErrorMessage = string.Empty;
        try
        {
            var start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var data = await database.GetTransactionsAsync(start, start.AddMonths(1));
            Items.Clear();
            foreach (var item in data) Items.Add(item);
            TotalRevenue = data.Where(x => x.Type == TransactionTypes.Revenue).Sum(x => x.Amount);
            TotalExpenses = data.Where(x => x.Type == TransactionTypes.Expense).Sum(x => x.Amount);
            Balance = TotalRevenue - TotalExpenses;
        }
        catch { ErrorMessage = "Não foi possível carregar os lançamentos."; }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private Task NewRevenueAsync() => Shell.Current.GoToAsync($"{nameof(TransactionFormPage)}?kind={Uri.EscapeDataString(TransactionTypes.Revenue)}");

    [RelayCommand]
    private Task NewExpenseAsync() => Shell.Current.GoToAsync($"{nameof(TransactionFormPage)}?kind={Uri.EscapeDataString(TransactionTypes.Expense)}");

    [RelayCommand]
    private async Task DeleteAsync(Transaction? item)
    {
        if (item is null || IsBusy) return;
        try
        {
            await database.DeleteTransactionAsync(item);
            await LoadAsync();
        }
        catch { ErrorMessage = "Não foi possível excluir este lançamento."; }
    }
}
