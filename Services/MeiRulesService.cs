using MEIUtil.Models;

namespace MEIUtil.Services;

public sealed class MeiRulesService
{
    public decimal GetApplicableAnnualLimit(MeiProfile profile, DateTime date)
    {
        if (profile.OpenedAt.Year != date.Year)
            return profile.AnnualRevenueLimit;

        var monthsActive = Math.Clamp(12 - profile.OpenedAt.Month + 1, 1, 12);
        return Math.Round(profile.AnnualRevenueLimit / 12m * monthsActive, 2);
    }

    public decimal GetProjection(decimal revenueToDate, MeiProfile profile, DateTime today)
    {
        var startMonth = profile.OpenedAt.Year == today.Year ? profile.OpenedAt.Month : 1;
        var elapsedMonths = Math.Max(1, today.Month - startMonth + 1);
        var activeMonthsInYear = 12 - startMonth + 1;
        return Math.Round(revenueToDate / elapsedMonths * activeMonthsInYear, 2);
    }

    public string GetRiskText(decimal percent)
        => percent switch
        {
            < 70m => "Dentro do limite",
            < 90m => "Atenção ao ritmo de faturamento",
            _ => "Próximo do limite anual"
        };
}
