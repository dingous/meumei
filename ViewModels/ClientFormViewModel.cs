using System.Net.Mail;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MEIUtil.Models;
using MEIUtil.Services;

namespace MEIUtil.ViewModels;

public partial class ClientFormViewModel(DatabaseService database) : ObservableObject
{
    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string document = string.Empty;
    [ObservableProperty] private string phone = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string notes = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy;

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy) return;

        ErrorMessage = string.Empty;
        Name = Name.Trim();
        Email = Email.Trim();
        Document = BrazilianDocumentValidator.Normalize(Document);

        if (Name.Length < 2)
        {
            ErrorMessage = "Informe o nome do cliente.";
            return;
        }

        if (!string.IsNullOrWhiteSpace(Document) &&
            !BrazilianDocumentValidator.IsValidCpfOrCnpj(Document))
        {
            ErrorMessage = "Informe um CPF ou CNPJ válido. CNPJ alfanumérico também é aceito.";
            return;
        }

        if (!string.IsNullOrWhiteSpace(Email) && !MailAddress.TryCreate(Email, out _))
        {
            ErrorMessage = "Informe um e-mail válido ou deixe o campo vazio.";
            return;
        }

        IsBusy = true;
        try
        {
            await database.SaveClientAsync(new Client
            {
                Name = Name,
                Document = Document,
                Phone = Phone.Trim(),
                Email = Email,
                Notes = Notes.Trim()
            });
            await Shell.Current.GoToAsync("..");
        }
        catch
        {
            ErrorMessage = "Não foi possível salvar o cliente. Tente novamente.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
