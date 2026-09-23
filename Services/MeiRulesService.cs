using MEIUtil.Models;

namespace MEIUtil.Services;

public sealed class MeiRulesService
{
    public decimal GetApplicableAnnualLimit(MeiProfile profile, DateTime date)
    {
        var openedAt = profile.OpenedAt.Date;
        if (openedAt > date.Date)
            return 0m;

        if (openedAt.Year != date.Year)
            return profile.AnnualRevenueLimit;

        var monthsActive = Math.Clamp(12 - openedAt.Month + 1, 1, 12);
        return Math.Round(profile.AnnualRevenueLimit / 12m * monthsActive, 2);
    }

    public decimal GetProjection(decimal revenueToDate, MeiProfile profile, DateTime today)
    {
        if (revenueToDate <= 0)
            return 0m;

        var openedAt = profile.OpenedAt.Date;
        if (openedAt > today.Date)
            return 0m;

        var startMonth = openedAt.Year == today.Year ? openedAt.Month : 1;
        startMonth = Math.Clamp(startMonth, 1, today.Month);

        var elapsedMonths = Math.Max(1, today.Month - startMonth + 1);
        var activeMonthsInYear = 12 - startMonth + 1;
        return Math.Round(revenueToDate / elapsedMonths * activeMonthsInYear, 2);
    }

    public bool IsObligationApplicable(ObligationRecord obligation, MeiProfile profile)
    {
        var openedMonth = new DateTime(profile.OpenedAt.Year, profile.OpenedAt.Month, 1);

        if (obligation.Type.Equals("DAS", StringComparison.OrdinalIgnoreCase))
        {
            var reference = obligation.DueDate.AddMonths(-1);
            var referenceMonth = new DateTime(reference.Year, reference.Month, 1);
            return referenceMonth >= openedMonth;
        }

        if (obligation.Type.Equals("DASN", StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(obligation.Reference, out var referenceYear))
            return referenceYear >= profile.OpenedAt.Year;

        return true;
    }

    public string GetRiskText(decimal percent)
        => percent switch
        {
            < 70m => "Dentro do limite",
            < 90m => "Atenção ao ritmo de faturamento",
            _ => "Próximo do limite anual"
        };
}
