using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MEIUtil.Models;
using MEIUtil.Services;
using MEIUtil.Views;

namespace MEIUtil.ViewModels;

public partial class ClientsViewModel(DatabaseService database) : ObservableObject
{
    public ObservableCollection<Client> Items { get; } = [];
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
            foreach (var item in await database.GetClientsAsync()) Items.Add(item);
        }
        catch { ErrorMessage = "Não foi possível carregar os clientes."; }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private Task AddAsync() => Shell.Current.GoToAsync(nameof(ClientFormPage));
}
