using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MEIUtil.Models;
using MEIUtil.Services;
using MEIUtil.Views;
using Microsoft.Maui.ApplicationModel.DataTransfer;

namespace MEIUtil.ViewModels;

public partial class QuotesViewModel(
    DatabaseService database) : ObservableObject
{
    public ObservableCollection<Quote> Items { get; } = [];

    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string errorMessage = string.Empty;

    private bool _isNavigating;

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var data =
                await database.GetQuotesAsync();

            Items.Clear();

            foreach (var item in data)
                Items.Add(item);
        }
        catch
        {
            ErrorMessage =
                "Não foi possível carregar os orçamentos.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddAsync()
    {
        if (_isNavigating)
            return;

        _isNavigating = true;
        ErrorMessage = string.Empty;

        try
        {
            var shell =
                Shell.Current
                ?? throw new InvalidOperationException();

            await shell.GoToAsync(
                nameof(QuoteFormPage));
        }
        catch
        {
            ErrorMessage =
                "Não foi possível abrir o novo orçamento agora.";
        }
        finally
        {
            _isNavigating = false;
        }
    }

    [RelayCommand]
    private async Task ShareAsync(
        Quote? quote)
    {
        if (quote is null ||
            IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var content =
                $"ORÇAMENTO {quote.Number}\n" +
                $"Cliente: {quote.ClientName}\n\n" +
                $"{quote.Description}\n\n" +
                $"Total: {quote.Total:C2}\n" +
                $"Validade: {quote.ValidUntil:dd/MM/yyyy}";

            await Share.Default.RequestAsync(
                new ShareTextRequest
                {
                    Title =
                        $"Orçamento {quote.Number}",
                    Text = content
                });
        }
        catch
        {
            ErrorMessage =
                "Não foi possível abrir o compartilhamento neste dispositivo.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
