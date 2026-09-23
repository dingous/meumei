using System.Globalization;

namespace MEIUtil.Services;

public sealed class TrialService
{
    private const string TrialStartedKey = "trial_started_utc";
    private static readonly TimeSpan TrialDuration = TimeSpan.FromDays(365);

    public TrialStatus GetStatus()
    {
        var now = DateTimeOffset.UtcNow;
        var raw = Preferences.Default.Get(TrialStartedKey, string.Empty);

        if (!DateTimeOffset.TryParse(
                raw,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var started) ||
            started > now.AddMinutes(5))
        {
            started = now;
            Preferences.Default.Set(TrialStartedKey, started.ToString("O", CultureInfo.InvariantCulture));
        }

        var expires = started.Add(TrialDuration);
        var remaining = expires - now;
        var daysRemaining = remaining > TimeSpan.Zero
            ? Math.Clamp((int)Math.Ceiling(remaining.TotalDays), 0, 365)
            : 0;

        return new TrialStatus(started, expires, daysRemaining, remaining > TimeSpan.Zero);
    }
}

public sealed record TrialStatus(DateTimeOffset StartedAtUtc, DateTimeOffset ExpiresAtUtc, int DaysRemaining, bool IsActive);
