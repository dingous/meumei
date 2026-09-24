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
        if (IsBusy)
            return;

        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            await LoadCoreAsync();
        }
        catch
        {
            ErrorMessage =
                "Não foi possível carregar o calendário de obrigações.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadCoreAsync()
    {
        await seed.EnsureAsync();

        var profile =
            await database.GetProfileAsync();

        var items =
            (await database.GetObligationsAsync())
            .Where(x =>
                rules.IsObligationApplicable(
                    x,
                    profile))
            .OrderBy(x => x.IsDone)
            .ThenBy(x => x.DueDate)
            .ThenBy(x => x.Id)
            .ToList();

        Items.Clear();

        foreach (var item in items)
            Items.Add(item);
    }

    [RelayCommand]
    private async Task ToggleDoneAsync(
        ObligationRecord? item)
    {
        if (item is null ||
            IsBusy)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = string.Empty;

        var previous =
            item.IsDone;

        item.IsDone =
            !previous;

        try
        {
            await database
                .SaveObligationAsync(item);

            await LoadCoreAsync();
        }
        catch
        {
            item.IsDone = previous;

            ErrorMessage =
                "Não foi possível atualizar esta obrigação.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
