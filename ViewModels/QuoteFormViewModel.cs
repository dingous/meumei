using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MEIUtil.Models;
using MEIUtil.Services;

namespace MEIUtil.ViewModels;

public partial class QuoteFormViewModel(DatabaseService database) : ObservableObject
{
    [ObservableProperty] private string clientName = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private decimal total;
    [ObservableProperty] private DateTime validUntil = DateTime.Today.AddDays(10);
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy;

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy) return;
        ErrorMessage = string.Empty;
        ClientName = ClientName.Trim();
        Description = Description.Trim();

        if (ClientName.Length < 2 || Description.Length < 2) { ErrorMessage = "Informe cliente e descrição do orçamento."; return; }
        if (Total <= 0) { ErrorMessage = "Informe um valor maior que zero."; return; }
        if (ValidUntil.Date < DateTime.Today) { ErrorMessage = "A validade não pode estar no passado."; return; }

        IsBusy = true;
        try
        {
            await database.SaveQuoteAsync(new Quote
            {
                ClientName = ClientName,
                Description = Description,
                Total = Math.Round(Total, 2),
                ValidUntil = ValidUntil.Date,
                CreatedAt = DateTime.Now
            });
            await Shell.Current.GoToAsync("..");
        }
        catch { ErrorMessage = "Não foi possível salvar o orçamento. Tente novamente."; }
        finally { IsBusy = false; }
    }
}
