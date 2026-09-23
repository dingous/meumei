using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MEIUtil.Models;
using MEIUtil.Services;

namespace MEIUtil.ViewModels;

public partial class ObligationsViewModel(DatabaseService database, SeedService seed) : ObservableObject
{
    public ObservableCollection<ObligationRecord> Items { get; } = [];
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
            await seed.EnsureAsync();
            Items.Clear();
            foreach (var item in await database.GetObligationsAsync()) Items.Add(item);
        }
        catch { ErrorMessage = "Não foi possível carregar o calendário de obrigações."; }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task ToggleDoneAsync(ObligationRecord? item)
    {
        if (item is null || IsBusy) return;
        ErrorMessage = string.Empty;
        try
        {
            item.IsDone = !item.IsDone;
            await database.SaveObligationAsync(item);
            await LoadAsync();
        }
        catch
        {
            item.IsDone = !item.IsDone;
            ErrorMessage = "Não foi possível atualizar esta obrigação.";
        }
    }
}
