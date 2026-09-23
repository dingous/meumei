using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MEIUtil.Models;
using MEIUtil.Services;
using MEIUtil.Views;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace MEIUtil.ViewModels;

public partial class QuotesViewModel(DatabaseService database) : ObservableObject
{
    public ObservableCollection<Quote> Items { get; } = [];
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
            Items.Clear();
            foreach (var item in await database.GetQuotesAsync()) Items.Add(item);
        }
        catch { ErrorMessage = "Não foi possível carregar os orçamentos."; }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private Task AddAsync() => Shell.Current.GoToAsync(nameof(QuoteFormPage));

    [RelayCommand]
    private async Task ShareAsync(Quote? quote)
    {
        if (quote is null) return;
        try
        {
            var content = $"ORÇAMENTO {quote.Number}\nCliente: {quote.ClientName}\n\n{quote.Description}\n\nTotal: {quote.Total:C2}\nValidade: {quote.ValidUntil:dd/MM/yyyy}";
            await Share.Default.RequestAsync(new ShareTextRequest { Title = $"Orçamento {quote.Number}", Text = content });
        }
        catch { ErrorMessage = "Não foi possível abrir o compartilhamento neste dispositivo."; }
    }
}
