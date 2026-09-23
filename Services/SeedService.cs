using MEIUtil.Models;

namespace MEIUtil.Services;

public sealed class SeedService(DatabaseService database)
{
    private readonly SemaphoreSlim _seedLock = new(1, 1);

    public async Task EnsureAsync()
    {
        await _seedLock.WaitAsync();

        try
        {
            var existing =
                await database.GetObligationsAsync();

            var profile =
                await database.GetProfileAsync();

            var today =
                DateTime.Today;

            var currentYear =
                today.Year;

            var keys = existing
                .Select(x => x.Key)
                .ToHashSet(
                    StringComparer.OrdinalIgnoreCase);

            if (profile.OpenedAt.Year > currentYear)
                return;

            var firstMonth =
                profile.OpenedAt.Year == currentYear
                    ? Math.Clamp(
                        profile.OpenedAt.Month,
                        1,
                        12)
                    : 1;

            for (var month = firstMonth;
                 month <= 12;
                 month++)
            {
                var referenceDate =
                    new DateTime(
                        currentYear,
                        month,
                        1);

                var dueMonth =
                    referenceDate.AddMonths(1);

                var dueDate =
                    new DateTime(
                        dueMonth.Year,
                        dueMonth.Month,
                        20);

                if (dueDate.Date < today)
                    continue;

                var key =
                    $"DAS-{currentYear}-{month:00}";

                if (!keys.Add(key))
                    continue;

                await database.SaveObligationAsync(
                    new ObligationRecord
                    {
                        Key = key,
                        Type = "DAS",
                        Title = "Pagamento mensal DAS",
                        Reference =
                            referenceDate.ToString("MM/yyyy"),
                        DueDate = dueDate
                    });
            }

            var previousReferenceYear =
                currentYear - 1;

            if (previousReferenceYear >=
                profile.OpenedAt.Year)
            {
                await EnsureDasnAsync(
                    previousReferenceYear,
                    keys,
                    today);
            }

            await EnsureDasnAsync(
                currentYear,
                keys,
                today);
        }
        finally
        {
            _seedLock.Release();
        }
    }

    private async Task EnsureDasnAsync(
        int referenceYear,
        HashSet<string> keys,
        DateTime today)
    {
        var dueYear =
            referenceYear + 1;

        var dueDate =
            new DateTime(
                dueYear,
                5,
                31);

        if (dueDate.Date < today)
            return;

        var key =
            $"DASN-{dueYear}";

        if (!keys.Add(key))
            return;

        await database.SaveObligationAsync(
            new ObligationRecord
            {
                Key = key,
                Type = "DASN",
                Title =
                    "Declaração anual DASN-SIMEI",
                Reference =
                    referenceYear.ToString(),
                DueDate = dueDate
            });
    }
}
