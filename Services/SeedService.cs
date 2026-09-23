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
            var existing = await database.GetObligationsAsync();
            var profile = await database.GetProfileAsync();
            var currentYear = DateTime.Today.Year;
            var keys = existing.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (profile.OpenedAt.Year <= currentYear)
            {
                var firstMonth = profile.OpenedAt.Year == currentYear
                    ? Math.Clamp(profile.OpenedAt.Month, 1, 12)
                    : 1;

                for (var month = firstMonth; month <= 12; month++)
                {
                    var key = $"DAS-{currentYear}-{month:00}";
                    if (!keys.Add(key))
                        continue;

                    var referenceDate = new DateTime(currentYear, month, 1);
                    var dueMonth = referenceDate.AddMonths(1);
                    await database.SaveObligationAsync(new ObligationRecord
                    {
                        Key = key,
                        Type = "DAS",
                        Title = "Pagamento mensal DAS",
                        Reference = referenceDate.ToString("MM/yyyy"),
                        DueDate = new DateTime(dueMonth.Year, dueMonth.Month, 20)
                    });
                }

                var dasnKey = $"DASN-{currentYear + 1}";
                if (keys.Add(dasnKey))
                {
                    await database.SaveObligationAsync(new ObligationRecord
                    {
                        Key = dasnKey,
                        Type = "DASN",
                        Title = "Declaração anual DASN-SIMEI",
                        Reference = currentYear.ToString(),
                        DueDate = new DateTime(currentYear + 1, 5, 31)
                    });
                }
            }
        }
        finally
        {
            _seedLock.Release();
        }
    }
}
