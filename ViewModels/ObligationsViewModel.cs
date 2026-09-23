using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MEIUtil.Models;
using MEIUtil.Services;

namespace MEIUtil.ViewModels;

public partial class ObligationsViewModel(
    DatabaseService database,
    SeedService seed,
    MeiRulesService rules) : ObservableObject
{
    public ObservableCollection<ObligationRecord> Items { get; } = [];
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
            await seed.EnsureAsync();
            var profile = await database.GetProfileAsync();
            var items = (await database.GetObligationsAsync())
                .Where(x => rules.IsObligationApplicable(x, profile))
                .OrderBy(x => x.IsDone)
                .ThenBy(x => x.DueDate);

            Items.Clear();
            foreach (var item in items)
                Items.Add(item);
        }
        catch
        {
            ErrorMessage = "Não foi possível carregar o calendário de obrigações.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ToggleDoneAsync(ObligationRecord? item)
    {
        if (item is null || IsBusy) return;

        IsBusy = true;
        ErrorMessage = string.Empty;
        var previous = item.IsDone;
        item.IsDone = !previous;

        try
        {
            await database.SaveObligationAsync(item);
        }
        catch
        {
            item.IsDone = previous;
            ErrorMessage = "Não foi possível atualizar esta obrigação.";
            IsBusy = false;
            return;
        }

        IsBusy = false;
        await ReloadAsync();
    }
}
