namespace MEIUtil.Services;

public sealed class TrialService
{
    private const string TrialStartedKey = "trial_started_utc";
    private static readonly TimeSpan TrialDuration = TimeSpan.FromDays(365);

    public TrialStatus GetStatus()
    {
        var raw = Preferences.Default.Get(TrialStartedKey, string.Empty);
        if (!DateTime.TryParse(raw, null, System.Globalization.DateTimeStyles.RoundtripKind, out var started))
        {
            started = DateTime.UtcNow;
            Preferences.Default.Set(TrialStartedKey, started.ToString("O"));
        }

        var expires = started.Add(TrialDuration);
        var remaining = expires - DateTime.UtcNow;
        return new TrialStatus(started, expires, Math.Max(0, (int)Math.Ceiling(remaining.TotalDays)), remaining > TimeSpan.Zero);
    }
}

public sealed record TrialStatus(DateTime StartedAtUtc, DateTime ExpiresAtUtc, int DaysRemaining, bool IsActive);
