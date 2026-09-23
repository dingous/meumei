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
            var currentYear = DateTime.Today.Year;
            var keys = existing.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);

            for (var month = 1; month <= 12; month++)
            {
                var key = $"DAS-{currentYear}-{month:00}";
                if (!keys.Add(key)) continue;

                var referenceDate = new DateTime(currentYear, month, 1);
                var due = new DateTime(referenceDate.AddMonths(1).Year, referenceDate.AddMonths(1).Month, 20);
                await database.SaveObligationAsync(new ObligationRecord
                {
                    Key = key,
                    Type = "DAS",
                    Title = "Pagamento mensal DAS",
                    Reference = referenceDate.ToString("MM/yyyy"),
                    DueDate = due
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
        finally
        {
            _seedLock.Release();
        }
    }
}
