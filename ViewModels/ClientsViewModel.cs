using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MEIUtil.Models;
using MEIUtil.Services;
using MEIUtil.Views;

namespace MEIUtil.ViewModels;

public partial class ClientsViewModel(
    DatabaseService database) : ObservableObject
{
    public ObservableCollection<Client> Items { get; } = [];

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
                await database.GetClientsAsync();

            Items.Clear();

            foreach (var item in data)
                Items.Add(item);
        }
        catch
        {
            ErrorMessage =
                "Não foi possível carregar os clientes.";
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
                nameof(ClientFormPage));
        }
        catch
        {
            ErrorMessage =
                "Não foi possível abrir o cadastro de cliente agora.";
        }
        finally
        {
            _isNavigating = false;
        }
    }
}
